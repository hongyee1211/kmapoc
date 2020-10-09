using System;
using System.Collections;

namespace CognitiveSearch.UI.Models
{
    public class SearchResultViewModel
    {
        public DocumentResult documentResult { get; set; }

        public string query { get; set; }

        public SearchFacet[] selectedFacets { get; set; }

        public int currentPage { get; set; }

        public string searchId { get; set; }

        public int searchFbId { get; set; }

        public string applicationInstrumentationKey { get; set; }

        public string[] facetableFields { get; set; }

        public Boolean subscribed { get; set; }

        public string[] standards { get; set; }
    }
}