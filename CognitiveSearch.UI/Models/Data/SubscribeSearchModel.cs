using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Models.Data
{
    public class SubscribeSearchModel
    {
        public int documentCount { get; set; }
        public string userId { get; set; }
        public string givenName { get; set; }
        public string userType { get; set; }
        public string query { get; set; }

    }
}
