using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.DAL
{
    public class FBReviewDocumentModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int revFeedbackId { get; set; }
        public int searchId { get; set; }
        public string documentName { get; set; }
        public string comment { get; set; }
        public int rating { get; set; }
    }
}
