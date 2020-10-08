using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Models.Data
{    public class FBTagAnnotationModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int annotationFeedbackId { get; set; }
        public int searchId { get; set; }
        public string documentName { get; set; }
        public string annotation { get; set; }
        public string tag { get; set; }
    }
}
