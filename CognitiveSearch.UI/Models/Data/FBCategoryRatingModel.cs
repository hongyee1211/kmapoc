using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.DAL
{
    public class FBCategoryRatingModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int categoryRatingId { get; set; }
        public int searchId { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public int rating { get; set; }
    }
}
