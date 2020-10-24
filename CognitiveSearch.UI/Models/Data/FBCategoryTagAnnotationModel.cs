using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Models.Data
{
    public class FBCategoryTagAnnotationModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int categoryAnnotationFeedbackId { get; set; }
        public int searchId { get; set; }
        public string annotation { get; set; }
        public string tag { get; set; }
        public string current { get; set; }
        public int allow { get; set; }
    }
}
