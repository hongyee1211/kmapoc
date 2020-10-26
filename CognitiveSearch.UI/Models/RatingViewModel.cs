using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Models.Data;
using System.Collections.Generic;

namespace CognitiveSearch.UI.Models
{
    public class RatingViewModel
    {
        public List<RatingRow> feedbacks { get; set; }
    }

    public class RatingRow
    {
        public int searchId { get; set; }
        public int rating { get; set; }
        public string document { get; set; }
        public string query { get; set; }
        public string user { get; set; }
        public int id { get; set; }
    }
}