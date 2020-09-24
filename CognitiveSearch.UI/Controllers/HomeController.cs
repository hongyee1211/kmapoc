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

namespace CognitiveSearch.UI.Controllers
{
    public class HomeController : Controller
    {

        private readonly ITokenAcquisition tokenAcquisition;
        private readonly IGraphApiOperations graphApiOperations;
        private readonly IArmOperations armOperations;

        private IConfiguration _configuration { get; set; }
        private DocumentSearchClient _docSearch { get; set; }
        private string _configurationError { get; set; }

        public HomeController(
            IConfiguration configuration,
            ITokenAcquisition tokenAcquisition,
            IGraphApiOperations graphApiOperations,
            IArmOperations armOperations)
        {
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
                if (child.Name == "givenName")
                {
                    Response.Cookies.Append(child.Name.ToString(), child.Value.ToString(), options);
                }
                if (child.Name == "displayName")
                {
                    Response.Cookies.Append(child.Name.ToString(), child.Value.ToString(), options);
                }
                if (child.Name == "userType")
                {
                    Response.Cookies.Append(child.Name.ToString(), child.Value.ToString(), options);
                }
                //if (child.Name == "groups")
                //{
                //    Response.Cookies.Append(child.Name.ToString(), child.Value.ToString(), options);
                //}
            }

            return View();
        }

        public static List<FeedbackModel> feedbackModels = new List<FeedbackModel>();

        [HttpPost]
        public void getFeedbackVal(FeedbackModel feedbackVal)
        {
            var feedbackModel = new FeedbackModel();
            feedbackModel.feedbackID = feedbackVal.feedbackID;
            feedbackModel.feedbackName = feedbackVal.feedbackName;
            feedbackModel.feedbackAction = feedbackVal.feedbackAction;

            feedbackModels.Add(feedbackModel);

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
            public static string[] thumbsDownDocuments { get; set; }
        }

        public IActionResult Search([FromQuery]string q, [FromQuery]string facets = "", [FromQuery]int page = 1)
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
            MyGlobalVariables.MyGlobalString = strText;

            var arr = Regex.Matches(strText, @"[A-Z]+(?=[A-Z][a-z]+)|\d|[A-Z][a-z]+")
             .Cast<Match>()
             .Select(m => m.Value)
             .ToArray();

            var viewModel = SearchView(new SearchParameters
            {
                q = String.Join(" ", arr) + ", " + q,
                searchFacets = searchFacets,
                currentPage = page
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
        }

        [HttpPost]
        public SearchResultViewModel SearchView([FromForm]SearchParameters searchParams)
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

            var viewModel = new SearchResultViewModel
            {
                documentResult = _docSearch.GetDocuments(searchParams.q, searchParams.searchFacets, searchParams.currentPage, searchParams.polygonString),
                query = searchParams.q,
                selectedFacets = searchParams.searchFacets,
                currentPage = searchParams.currentPage,
                searchId = searchidId ?? null,
                applicationInstrumentationKey = _configuration.GetSection("InstrumentationKey")?.Value,
                facetableFields = _docSearch.Model.Facets.Select(k => k.Name).ToArray()
            };
            return viewModel;
        }

        [HttpPost]
        public IActionResult GetDocumentById(string id = "")
        {
            var result = _docSearch.GetDocumentById(id);

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
    }
}
