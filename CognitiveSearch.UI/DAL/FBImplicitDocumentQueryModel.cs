using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.DAL
{
    public class FBImplicitDocumentQueryModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDQueryFeedbackId { get; set; }
        public int searchId { get; set; }
        public string documentName { get; set; }
        public string query { get; set; }
    }
}
