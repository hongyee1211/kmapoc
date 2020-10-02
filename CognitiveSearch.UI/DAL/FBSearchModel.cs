using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.DAL
{
    public class FBSearchModel
    {
        [Key]
        public int searchId { get; set; }
        public string userId { get; set; }
        public string givenName { get; set; }
        public string userType { get; set; }
        public string query { get; set; }
    }
}
