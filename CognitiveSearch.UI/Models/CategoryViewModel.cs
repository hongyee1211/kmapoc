using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Models
{
    public class CategoryViewModel
    {
        public List<CategoryRow> categories { get; set; }
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
}
