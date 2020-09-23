using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Search
{
    public class SearchFeedback
    {
        //Key is for thumbs down value (0). If its thumbs up, we will not record it
        public string Key { get; set; }
        //Value is to store the metadata_storage_name 
        public string Value { get; set; }
    }
}
