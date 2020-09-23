using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;

namespace CognitiveSearch.UI.Models
{
    public class FeedbackModel
    {
        public string feedbackID { get; set; }
        public string feedbackName { get; set; }
        public string feedbackAction { get; set; }
    }
}
