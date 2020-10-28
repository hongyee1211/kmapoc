// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.ApplicationInsights;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using CognitiveSearch.UI.Controllers;
using CognitiveSearch.UI.Models;
using Azure;
using Microsoft.AspNetCore.Http;

namespace CognitiveSearch.UI
{
    public class DocumentSearchClient
    {
        private IConfiguration _configuration { get; set; }
        private SearchServiceClient _searchClient;
        private ISearchIndexClient _indexClient;
        private string searchServiceName { get; set; }
        private string apiKey { get; set; }
        private string IndexName { get; set; }
        private string IndexerName { get; set; }

        private string idField { get; set; }

        // Client logs all searches in Application Insights
        private static TelemetryClient telemetryClient = new TelemetryClient();
        public static string _searchId;

        public SearchSchema Schema { get; set; }
        public SearchModel Model { get; set; }

        public static string errorMessage;

        bool _isPathBase64Encoded { get; set; }

        // data source information. Currently supporting 3 data sources indexed by different indexers
        private static string[] containerAddresses = null;
        private static string[] tokens = null;

        // this should match the default value used in appsettings.json.  
        private static string defaultContainerUriValue = "https://{storage-account-name}.blob.core.windows.net/{container-name}";


        public DocumentSearchClient(IConfiguration configuration)
        {
            try
            {
                _configuration = configuration;
                searchServiceName = configuration.GetSection("SearchServiceName")?.Value;
                apiKey = configuration.GetSection("SearchApiKey")?.Value;
                IndexName = configuration.GetSection("SearchIndexName")?.Value;
                IndexerName = configuration.GetSection("SearchIndexerName")?.Value;
                idField = configuration.GetSection("KeyField")?.Value;
                telemetryClient.InstrumentationKey = configuration.GetSection("InstrumentationKey")?.Value;

                // Create an HTTP reference to the catalog index
                _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
                _indexClient = _searchClient.Indexes.GetClient(IndexName);

                Schema = new SearchSchema().AddFields(_searchClient.Indexes.Get(IndexName).Fields);
                Model = new SearchModel(Schema);

                _isPathBase64Encoded = (configuration.GetSection("IsPathBase64Encoded")?.Value == "True");

            }
            catch (Exception e)
            {
                // If you get an exceptio here, most likely you have not set your
                // credentials correctly in appsettings.json
                throw new ArgumentException("DocumentSearchClient" + e.Message.ToString());
            }
        }

        public DocumentSearchResult<Document> Search(string searchText, SearchFacet[] searchFacets = null, string[] selectFilter = null, int currentPage = 1, string polygonString = null, string documentGroupId = null)
        {
            try
            {
                SearchParameters sp = GenerateSearchParameters(searchFacets, selectFilter, currentPage, polygonString, documentGroupId);

                if (!string.IsNullOrEmpty(telemetryClient.InstrumentationKey))
                {
                    var s = GenerateSearchId(searchText, sp);
                    _searchId = s.Result;
                }

                return _indexClient.Documents.Search(searchText, sp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }
            return null;
        }

        public SearchParameters GenerateSearchParameters(SearchFacet[] searchFacets = null, string[] selectFilter = null, int currentPage = 1, string polygonString = null, string documentGroupId = null)
        {
            // For more information on search parameters visit: 
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.search.models.searchparameters?view=azure-dotnet
            SearchParameters sp = new SearchParameters()
            {
                SearchMode = SearchMode.All,
                Top = 100,
                Skip = ((int)Math.Ceiling((decimal)currentPage /10) - 1) * 100,
                IncludeTotalResultCount = true,
                QueryType = QueryType.Full,
                Select = selectFilter,
                Facets = Model.Facets.Select(f => f.Name).ToList(),
                HighlightFields = Model.SearchableFields,
                HighlightPreTag = "<b>",
                HighlightPostTag = "</b>"
            };

            string filter = null;
            var filterStr = string.Empty;

            if (searchFacets != null)
            {
                foreach (var item in searchFacets)
                {
                    var facet = Model.Facets.Where(f => f.Name == item.Key).FirstOrDefault();

                    filterStr = string.Join(",", item.Value);

                    // Construct Collection(string) facet query
                    if (facet.Type == typeof(string[]))
                    {
                        if (string.IsNullOrEmpty(filter))
                            filter = $"{item.Key}/any(t: search.in(t, '{filterStr}', ','))";
                        else
                            filter += $" and {item.Key}/any(t: search.in(t, '{filterStr}', ','))";
                    }
                    // Construct string facet query
                    else if (facet.Type == typeof(string))
                    {
                        if (string.IsNullOrEmpty(filter))
                            filter = $"{item.Key} eq '{filterStr}'";
                        else
                            filter += $" and {item.Key} eq '{filterStr}'";
                    }
                    // Construct DateTime facet query
                    else if (facet.Type == typeof(DateTime))
                    {
                        // TODO: Date filters
                    }
                }

                //code to add filter condition for the query
                switch (HomeController.MyGlobalVariables.UserDept)
                {
                    case "Compressor":
                        documentGroupId = "89b7db00-f092-48d0-8edb-ec3bb02f565e";
                        break;
                    case "Electrical":
                        documentGroupId = "2fc63317-295a-46b8-99eb-d563c7d1505f";
                        break;
                    case "Gas Turbine":
                        documentGroupId = "5bbc43d8-8cd2-4504-9450-95d018e86509";
                        break;
                    case "Instrument":
                        documentGroupId = "e84e11b5-19ba-4d55-962f-855b1f1eb096";
                        break;
                    case "Mechanical":
                        documentGroupId = "f7b826b6-f0eb-4474-bbe0-fdc17db339f7";
                        break;
                    case "MLNG":
                        documentGroupId = "11abfa5e-4b45-45ef-bebb-c13691190b42";
                        break;
                    case "MRCSB":
                        documentGroupId = "b5bbdc71-b6ae-430c-bd86-67ed64c85183";
                        break;
                    case "Process":
                        documentGroupId = "e46c7dfb-c25a-4f1f-8034-16338f03a324";
                        break;
                    default:
                        documentGroupId = "";
                        break;
                }

                if (documentGroupId != "")
                {
                    if (string.IsNullOrEmpty(filter))
                        filter = String.Format("document_group/any(p:search.in(p, '{0}'))", string.Join(",", documentGroupId));
                    else
                        filter += String.Format(" and document_group/any(p:search.in(p, '{0}'))", string.Join(",", documentGroupId));

                    SearchParameters parameters = new SearchParameters()
                    {
                        Filter = filter,
                        Select = new[] { "application essays" }
                    };
                }
                //else
                //{
                //    filter = null;
                //}
            }

            sp.Filter = filter;

            // Add Filter based on geographic polygon if it is set.
            if (polygonString != null && polygonString.Length > 0)
            {
                string geoQuery = "geo.intersects(geoLocation, geography'POLYGON((" + polygonString + "))')";
                
                if (sp.Filter != null && sp.Filter.Length > 0)
                { 
                    sp.Filter += " and " + geoQuery; 
                }
                else
                { 
                    sp.Filter = geoQuery; 
                }
            }

            return sp;
        }

        public DocumentSuggestResult<Document> Suggest(string searchText, bool fuzzy)
        {
            // Execute search based on query string
            try
            {
                SuggestParameters sp = new SuggestParameters()
                {
                    UseFuzzyMatching = fuzzy,
                    Top = 8
                };

                return _indexClient.Documents.Suggest(searchText, "sg", sp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }
            return null;
        }

        public AutocompleteResult Autocomplete(string searchText, bool fuzzy)
        {
            // Execute search based on query string
            try
            {
                AutocompleteParameters ap = new AutocompleteParameters()
                {
                    AutocompleteMode = AutocompleteMode.OneTermWithContext,
                    UseFuzzyMatching = fuzzy,
                    Top = 8
                };

                return _indexClient.Documents.Autocomplete(searchText, "sg", ap);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }
            return null;
        }


        public Document LookUp(string id)
        {
            // Execute geo search based on query string
            try
            {
                return _indexClient.Documents.Get(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }
            return null;
        }


        private async Task<string> GenerateSearchId(string searchText, SearchParameters sp)
        {
            var client = new SearchIndexClient(searchServiceName, IndexName, new SearchCredentials(apiKey));
            var headers = new Dictionary<string, List<string>>() { { "x-ms-azs-return-searchid", new List<string>() { "true" } } };
            var response = await client.Documents.SearchWithHttpMessagesAsync(searchText: searchText, searchParameters: sp, customHeaders: headers);
            IEnumerable<string> headerValues;
            string searchId = string.Empty;
            if (response.Response.Headers.TryGetValues("x-ms-azs-searchid", out headerValues))
            {
                searchId = headerValues.FirstOrDefault();
            }
            return searchId;
        }

        public string GetSearchId()
        {
            if (_searchId != null) { return _searchId; }
            return string.Empty;
        }

        public DocumentSearchResult<Document> GetFacets(string searchText, List<string> facetNames, int maxCount = 30)
        {
            var facets = new List<String>();

            foreach (var facet in facetNames) 
            {
                 facets.Add($"{facet}, count:{maxCount}");
            }

            // Execute search based on query string
            try
            {
                SearchParameters sp = new SearchParameters()
                {
                    SearchMode = SearchMode.Any,
                    Top = 10,
                    Select = new List<String>() { idField },
                    Facets = facets, 
                    QueryType = QueryType.Full
                };

                return _searchClient.Indexes.GetClient(IndexName).Documents.Search(searchText, sp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DocumentSearchClient: Error querying index: {0}\r\n", ex.Message.ToString());
            }
            return null;
        }

        //**TODO refactor feedback dataset parameter in the future
        public DocumentResult GetDocuments(string q, SearchFacet[] searchFacets, int currentPage, string polygonString = null, IQueryable<FeedbackModel> feedbacks = null)
        {
            var tokens = GetContainerSasUris();

            var selectFilter = Model.SelectFilter;

            if (!string.IsNullOrEmpty(q))
            {
                q = q.Replace("?", "");
            }

            var response = Search(q, searchFacets, selectFilter, currentPage, polygonString);
            var searchId = GetSearchId().ToString();
            var facetResults = new List<object>();
            var tagsResults = new List<object>();
            int i = 0;

            if (response != null && response.Facets != null)
            {
                // Return only the selected facets from the Search Model
                foreach (var facetResult in response.Facets.Where(f => Model.Facets.Where(x => x.Name == f.Key).Any()))
                {
                    var cleanValues = GetCleanFacetValues(facetResult);

                    facetResults.Add(new
                    {
                        key = facetResult.Key,
                        value = cleanValues
                    });
                }

                foreach (var tagResult in response.Facets.Where(t => Model.Tags.Where(x => x.Name == t.Key).Any()))
                {
                    var cleanValues = GetCleanFacetValues(tagResult);

                    tagsResults.Add(new
                    {
                        key = tagResult.Key,
                        value = cleanValues
                    });
                }
            }

            
            var result = new DocumentResult
            {
                Results = (response == null ? null : response.Results),
                Facets = facetResults,
                Tags = tagsResults,
                Count = (response == null ? 0 : Convert.ToInt32(response.Count)),
                SearchId = searchId,
                IdField = idField,
                Token = tokens[0],
                IsPathBase64Encoded = _isPathBase64Encoded
            };
            return result;
            
        }

        /// <summary>
        /// Initiates a run of the search indexer.
        /// </summary>
        public async Task RunIndexer()
        {
            var indexStatus = await _searchClient.Indexers.GetStatusAsync(IndexerName);
            if (indexStatus.LastResult.Status != IndexerExecutionStatus.InProgress)
            {
                _searchClient.Indexers.Run(IndexerName);
            }
        }

        private string GetToken(string decodedPath, out int storageIndex)
        {
            // Initialize tokens and containers if not already initialized
            GetContainerSasUris();

            // Determine which token to use.
            string tokenToUse;
            if (decodedPath.ToLower().Contains(containerAddresses[1])) { tokenToUse = tokens[1]; storageIndex = 1; }
            else if (decodedPath.ToLower().Contains(containerAddresses[2])) { tokenToUse = tokens[2]; storageIndex = 2; }
            else { tokenToUse = tokens[0]; storageIndex = 0; }

            return tokenToUse;
        }

        /// <summary>
        /// This will return up to 3 tokens for the storage accounts
        /// </summary>
        /// <returns></returns>
        private string[] GetContainerSasUris()
        {
            // We need to refresh the tokens every time or they will become invalid.
            tokens = new string[3];
            containerAddresses = new string[3];

            string accountName = _configuration.GetSection("StorageAccountName")?.Value;
            string accountKey = _configuration.GetSection("StorageAccountKey")?.Value;

            SharedAccessBlobPolicy adHocPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Read
            };

            containerAddresses[0] = _configuration.GetSection("StorageContainerAddress")?.Value.ToLower();
            CloudBlobContainer container = new CloudBlobContainer(new Uri(containerAddresses[0]), new StorageCredentials(accountName, accountKey));
            tokens[0] = container.GetSharedAccessSignature(adHocPolicy, null);

            // Get token for second indexer data source
            containerAddresses[1] = _configuration.GetSection("StorageContainerAddress2")?.Value.ToLower();
            if (!String.Equals(containerAddresses[1], defaultContainerUriValue))
            {
                CloudBlobContainer container2 = new CloudBlobContainer(new Uri(containerAddresses[1]), new StorageCredentials(accountName, accountKey));
                tokens[1] = container2.GetSharedAccessSignature(adHocPolicy, null);
            }

            // Get token for third indexer data source
            containerAddresses[2] = _configuration.GetSection("StorageContainerAddress3")?.Value.ToLower();
            if (!String.Equals(containerAddresses[2], defaultContainerUriValue))
            {
                CloudBlobContainer container3 = new CloudBlobContainer(new Uri(containerAddresses[2]), new StorageCredentials(accountName, accountKey));
                tokens[2] = container3.GetSharedAccessSignature(adHocPolicy, null);
            }

            return tokens;
        }

        public DocumentResult GetDocumentById(string id)
        {
            var decodedPath = id;

            var response = LookUp(id);

            if (_isPathBase64Encoded)
            {
                decodedPath = Base64Decode(id);
            }

            int storageIndex;
            string tokenToUse = GetToken(decodedPath, out storageIndex);

            var result = new DocumentResult
            {
                Result = response,
                Token = tokenToUse,
                StorageIndex = storageIndex,
                DecodedPath = decodedPath,
                IdField = idField,
                IsPathBase64Encoded = _isPathBase64Encoded
            };
            return result;
        }

        private static string Base64Decode(string input)
        {
            if (input == null) throw new ArgumentNullException("input");
            int inputLength = input.Length;
            if (inputLength < 1) return null;

            // Get padding chars
            int numPadChars = (int)input[inputLength - 1] - (int)'0';
            if (numPadChars < 0 || numPadChars > 10)
            {
                return null;
            }

            // replace '-' and '_'
            char[] base64Chars = new char[inputLength - 1 + numPadChars];
            for (int iter = 0; iter < inputLength - 1; iter++)
            {
                char c = input[iter];

                switch (c)
                {
                    case '-':
                        base64Chars[iter] = '+';
                        break;

                    case '_':
                        base64Chars[iter] = '/';
                        break;

                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }

            // Add padding chars
            for (int iter = inputLength - 1; iter < base64Chars.Length; iter++)
            {
                base64Chars[iter] = '=';
            }

            var charArray = Convert.FromBase64CharArray(base64Chars, 0, base64Chars.Length);
            return System.Text.Encoding.Default.GetString(charArray);
        }

        /// <summary>
        /// In some situations you may want to restrict the facets that are displayed in the
        /// UI. This allows you to add some heuristics to remove facets that you may consider unnecessary.
        /// </summary>
        /// <param name="facetResult"></param>
        /// <returns></returns>
        private static IList<FacetResult> GetCleanFacetValues(KeyValuePair<string, IList<FacetResult>> facetResult)
        {
            IList<FacetResult> cleanValues = new List<FacetResult>();

            if (facetResult.Key == "people")
            {
                // only add names that are long enough 
                foreach (var element in facetResult.Value)
                {
                    //excluding incorrect people facets
                    switch (element.Value.ToString().ToLower())
                    {
                        case "appendix":
                        case "untuk dalaman":
                        case "c. the":
                        case "lo":
                        case "max":
                        case "ij":
                        case "categoris":
                        case "v. gtg":
                        case "ordine":
                        case "commessa":
                        case "cliente":
                        case "commesso":
                        case "cogen":
                        case "train":
                        case "solar":
                        case "bunga kertas":
                        case "bekok":
                        case "baronia":
                        case "inlet":
                        case "f. macroscopic":
                        case "dia":
                        case "amine":
                        case "doc":
                        case "monitor":
                        case "disabled comm":
                        case "achieved sil":
                        case "rotation":
                        case "pitch":
                        case "wall":
                        case "c. equipment":
                        case "max switch":
                        case "a. perimiter":
                        case "grade c.":
                        case "micropack":
                        case "fac":
                        case "major":
                        case "reboiler":
                        case "unclassified":
                        case "pam":
                        case "a. bill":
                        case "b. contractor":
                        case "c. contractor":
                        case "d. contractor":
                        case "e. contractor":
                        case "f. contractor":
                        case "g. contractor":
                        case "k. contractor":
                        case "target sil":
                        case "average normal max":
                        case "norm":
                        case "norm.max":
                        case "aux boiler":
                        case "period":
                        case "page":
                        case "analsis":
                            break;
                        default:
                            if (element.Value.ToString().Length >= 4)
                            {
                                cleanValues.Add(element);
                            }
                            break;
                    }
                }

                return cleanValues;
            }
            else
            {
                return facetResult.Value;
            }
        }
    }
}
