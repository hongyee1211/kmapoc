using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Models.Data
{
    public class FBImplicitDocumentResultModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDResultFeedbackId { get; set; }
        public int searchId { get; set; }
        public string documentName { get; set; }
        public string tagSelected { get; set; }
    }
}
