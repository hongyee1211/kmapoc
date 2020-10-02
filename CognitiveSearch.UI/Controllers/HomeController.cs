// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using CognitiveSearch.UI.Models;
using CognitiveSearch.UI.Services.GraphOperations;
using CognitiveSearch.UI.Services.ARM;
using CognitiveSearch.UI.Search;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CognitiveSearch.UI.DAL;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.Search;
using CognitiveSearch.UI.Helpers;

namespace CognitiveSearch.UI.Controllers
{
    public class HomeController : Controller
    {

        private readonly ITokenAcquisition tokenAcquisition;
        private readonly IGraphApiOperations graphApiOperations;
        private readonly IArmOperations armOperations;
        private readonly FeedbackContext _context;
        private readonly FeedbackDBHelper feedbackHandler;

        private IConfiguration _configuration { get; set; }
        private DocumentSearchClient _docSearch { get; set; }
        private string _configurationError { get; set; }
        private static ISearchIndexClient _indexClient;

        public HomeController(
            IConfiguration configuration,
            ITokenAcquisition tokenAcquisition,
            IGraphApiOperations graphApiOperations,
            IArmOperations armOperations,
            FeedbackContext context)
        {
            this._context = context;
            this.feedbackHandler = new FeedbackDBHelper(context);
            this.tokenAcquisition = tokenAcquisition;
            this.graphApiOperations = graphApiOperations;
            this.armOperations = armOperations;

            _configuration = configuration;
            InitializeDocSearch();
        }

        private void InitializeDocSearch()
        {
            try
            {
                _docSearch = new DocumentSearchClient(_configuration);
            }
            catch (Exception e)
            {
                _configurationError = $"The application settings are possibly incorrect. The server responded with this message: " + e.Message.ToString();
            }
        }

        /// <summary>
        /// Checks that the search client was intiailized successfully.
        /// If not, it will add the error reason to the ViewBag alert.
        /// </summary>
        /// <returns>A value indicating whether the search client was initialized succesfully.</returns>
        public bool CheckDocSearchInitialized()
        {
            if (_docSearch == null)
            {
                ViewBag.Style = "alert-warning";
                ViewBag.Message = _configurationError;
                return false;
            }

            return true;
        }

        public async Task<IActionResult> Index()
        {
            CheckDocSearchInitialized();

            var accessToken =
                await tokenAcquisition.GetAccessTokenForUserAsync(new[] { Infrastructure.Constants.ScopeUserRead });

            var me = await graphApiOperations.GetUserInformation(accessToken);

            ViewData["Me"] = me;

            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(60),
                IsEssential = true
            };

            var children = me.Properties();
            foreach (var child in children)
            {
                if (child.Name == "id")
                {
                    Response.Cookies.Append("userId", child.Value.ToString(), options);
                }
                else if (child.Name == "givenName")
                {
                    Response.Cookies.Append(child.Name.ToString(), child.Value.ToString(), options);
                }
                else if (child.Name == "displayName")
                {
                    Response.Cookies.Append(child.Name.ToString(), child.Value.ToString(), options);
                }
                else if (child.Name == "userType")
                {
                    Response.Cookies.Append(child.Name.ToString(), child.Value.ToString(), options);
                }
                else if (child.Name == "department")
                {
                    Response.Cookies.Append(child.Name.ToString(), child.Value.ToString(), options);
                }
            }

            return View();
        }

        public static List<FeedbackModel> feedbackModels = new List<FeedbackModel>();


        [HttpPost]
        public void getRating(FBRatingDocumentModel feedbackVal)
        {
            feedbackHandler.SaveRatingFeedback(feedbackVal.searchId, feedbackVal.documentName, feedbackVal.rating);
        }


        [HttpPost]
        public void getReview(FBReviewDocumentModel feedbackVal)
        {
            feedbackHandler.SaveReviewFeedback(feedbackVal.searchId, feedbackVal.documentName, feedbackVal.rating, feedbackVal.comment);

            //var feedbackModel = new FeedbackModel();
            //feedbackModel.userID = Request.Cookies["userId"];
            //feedbackModel.userType = Request.Cookies["userType"];
            //feedbackModel.givenName = Request.Cookies["givenName"];
            //feedbackModel.query = feedbackVal.query;
            //feedbackModel.documentName = feedbackVal.documentName;
            //feedbackModel.feedbackRating = feedbackVal.feedbackRating;
            //feedbackModel.comment = feedbackVal.comment;
            //var existing = this._context.Feedbacks.FirstOrDefault(f => f.userID.Equals(feedbackModel.userID) && f.documentName.Equals(feedbackModel.documentName) && f.query.Equals(feedbackModel.query));

            //if (existing == null)
            //{
            //    this._context.Feedbacks.Add(feedbackModel);
            //}
            //else
            //{
            //    this._context.Entry(existing).CurrentValues.SetValues(feedbackModel);
            //    //this._context.Feedbacks.Update(feedbackModel);
            //}
            //this._context.SaveChanges();

            //if (feedbackModels.Count() > 0) {
            //    foreach (FeedbackModel feedback in feedbackModels.ToList())
            //    {
            //        if (feedbackVal.feedbackID == feedback.feedbackID)
            //        {
            //            feedbackModels.Remove(feedback);
            //        }
            //    }
            //}

            //feedbackModels.Add(feedbackModel);

            //Console.WriteLine(feedbackVal.feedbackID);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //global variables for cookies
        public static class MyGlobalVariables
        {
            public static string MyGlobalString { get; set; }
            public static string DisplayName { get; set; }
            public static string UserDept { get; set; }
        }

        public IActionResult Search([FromQuery] string q, [FromQuery] string facets = "", [FromQuery] int page = 1)
        {
            // Split the facets.
            //  Expected format: &facets=key1_val1,key1_val2,key2_val1
            var searchFacets = facets
                // Split by individual keys
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                // Split key/values
                .Select(f => f.Split("_", StringSplitOptions.RemoveEmptyEntries))
                // Group by keys
                .GroupBy(f => f[0])
                // Select grouped key/values into SearchFacet array
                .Select(g => new SearchFacet { Key = g.Key, Value = g.Select(f => f[1]).ToArray() })
                .ToArray();

            string strText = Request.Cookies["givenName"];
            string strdisplayName = Request.Cookies["displayName"];
            string strUserDept = Request.Cookies["department"];
            MyGlobalVariables.MyGlobalString = strText;
            MyGlobalVariables.DisplayName = strdisplayName;
            MyGlobalVariables.UserDept = strUserDept;

            var arr = Regex.Matches(strText, @"[A-Z]+(?=[A-Z][a-z]+)|\d|[A-Z][a-z]+")
             .Cast<Match>()
             .Select(m => m.Value)
             .ToArray();

            string searchString = String.Join(" ", strText) + ", " + q;

            var viewModel = SearchView(new SearchParameters
            {
                q = searchString,
                searchFacets = searchFacets,
                currentPage = page,
            });

            return View(viewModel);
        }

        public class SearchParameters
        {
            public string q { get; set; }
            public SearchFacet[] searchFacets { get; set; }
            public int currentPage { get; set; }
            public string polygonString { get; set; }
            public string groupFilter { get; set; }
            public int searchId { get; set; }
        }

        [HttpPost]
        public SearchResultViewModel SearchView([FromForm] SearchParameters searchParams)
        {
            if (searchParams.q == null)
                searchParams.q = "*";
            if (searchParams.searchFacets == null)
                searchParams.searchFacets = new SearchFacet[0];
            if (searchParams.currentPage == 0)
                searchParams.currentPage = 1;

            string searchidId = null;

            if (CheckDocSearchInitialized())
                searchidId = _docSearch.GetSearchId().ToString();

            var searchQuery = feedbackHandler.AddSearchQuery(Request.Cookies["userId"], Request.Cookies["userType"], Request.Cookies["givenName"], searchParams.q);

            var feedbacks = this._context.Feedbacks.Where(feedback => feedback.query.Equals(searchParams.q) && feedback.userID.Equals(Request.Cookies["userId"]));

            var viewModel = new SearchResultViewModel
            {
                documentResult = _docSearch.GetDocuments(searchParams.q, searchParams.searchFacets, searchParams.currentPage, searchParams.polygonString, feedbacks),
                query = searchParams.q,
                selectedFacets = searchParams.searchFacets,
                currentPage = searchParams.currentPage,
                searchId = searchidId ?? null,
                searchFbId = searchQuery.searchId,
                applicationInstrumentationKey = _configuration.GetSection("InstrumentationKey")?.Value,
                facetableFields = _docSearch.Model.Facets.Select(k => k.Name).ToArray()
            };
            return viewModel;
        }

        [HttpPost]
        public IActionResult GetDocumentById(string id = "", int searchFbId = -1)
        {
            var result = _docSearch.GetDocumentById(id);
            string documentName = result.Result["metadata_storage_name"].ToString();

            if (searchFbId != -1 && documentName != null)
                feedbackHandler.AddImplicitDocumentQuery(searchFbId, documentName);

            return new JsonResult(result);
        }


        public class MapCredentials
        {
            public string MapKey { get; set; }
        }


        [HttpPost]
        public IActionResult GetMapCredentials()
        {
            string mapKey = _configuration.GetSection("AzureMapsSubscriptionKey")?.Value;

            return new JsonResult(
                new MapCredentials
                {
                    MapKey = mapKey
                });
        }

        [HttpPost]
        public ActionResult GetGraphData(string query, string[] fields, int maxLevels, int maxNodes)
        {
            string[] facetNames = fields;

            if (facetNames == null || facetNames.Length == 0)
            {
                string facetsList = _configuration.GetSection("GraphFacet")?.Value;

                facetNames = facetsList.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (query == null)
            {
                query = "*";
            }

            FacetGraphGenerator graphGenerator = new FacetGraphGenerator(_docSearch);
            var graphJson = graphGenerator.GetFacetGraphNodes(query, facetNames.ToList<string>(), maxLevels, maxNodes);

            return Content(graphJson.ToString(), "application/json");
        }

        [HttpPost, HttpGet]
        public ActionResult Suggest(string term, bool fuzzy = true)
        {
            // Change to _docSearch.Suggest if you would prefer to have suggestions instead of auto-completion
            var response = _docSearch.Autocomplete(term, fuzzy);

            List<string> suggestions = new List<string>();
            if (response != null)
            {
                foreach (var result in response.Results)
                {
                    suggestions.Add(result.Text);
                }
            }

            // Get unique items
            List<string> uniqueItems = suggestions.Distinct().ToList();

            return new JsonResult
            (
                uniqueItems
            );

        }

        public async Task<ActionResult> AutoComplete(string term)
        {
            InitializeDocSearch();

            // Setup the autocomplete parameters.
            var ap = new AutocompleteParameters()
            {
                AutocompleteMode = AutocompleteMode.OneTermWithContext,
                Top = 6
            };
            AutocompleteResult autocompleteResult = await _indexClient.Documents.AutocompleteAsync(term, "ka-suggestor-03", ap);

            // Convert the results to a list that can be displayed in the client.
            List<string> autocomplete = autocompleteResult.Results.Select(x => x.Text).ToList();

            // Return the list.
            return new JsonResult(autocomplete);
        }

        [HttpPost]
        public void MonitorDocumentQuery(int searchFbId, string documentName, string query)
        {
            feedbackHandler.AddImplicitDocumentQuery(searchFbId, documentName, query);
        }

        [HttpPost]
        public void MonitorDocumentResultTags(int searchFbId, string documentName, string tag)
        {
            feedbackHandler.AddImplicitDocumentResult(searchFbId, documentName, tag);
        }

        [HttpPost]
        public void MonitorCategoryTags(int searchFbId, string category, string tag, int rating)
        {
            feedbackHandler.AddCategoryFeedback(searchFbId, category, tag, rating);
        }
    }
}
