using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Models
{
    public class AdminViewModel
    {
        public List<CategoryRow> categories { get; set; }
        public List<ReviewRow> reviews { get; set; }
    }

    public class CategoryRow
    {
        public int id { get; set; }
        public int searchId { get; set; }
        public string category { get; set; }
        public string annotation { get; set; }
        public string name { get; set; }
        public string current { get; set; }
    }

    public class ReviewRow
    {
        public int searchId { get; set; }
        public int rating { get; set; }
        public string document { get; set; }
        public string comment { get; set; }
        public string query { get; set; }
        public string user { get; set; }
        public int id { get; set; }
    }
}
