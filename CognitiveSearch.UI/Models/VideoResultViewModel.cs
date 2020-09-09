using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;

namespace CognitiveSearch.UI.Models
{
    public class VideoResultViewModel
    {
        public VideoResult videoResult { get; set; }

        public string query { get; set; }

        public SearchFacet[] selectedFacets { get; set; }

        public int currentPage { get; set; }

        public string searchId { get; set; }

        public string applicationInstrumentationKey { get; set; }

        public string[] facetableFields { get; set; }
    }
}
