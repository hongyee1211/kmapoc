using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;

namespace CognitiveSearch.UI.Models
{
    public class FeedbackModel
    {
        public string userID { get; set; }
        public string documentName { get; set; }
        public string givenName { get; set; }
        public string userType { get; set; }
        public string comment { get; set; }
        public string query { get; set; }
        public int feedbackRating { get; set; }
    }
}
