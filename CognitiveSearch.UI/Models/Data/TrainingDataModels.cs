using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Models.Data
{
    public class TrainingAbbreviationModel
    {
        [Key]
        public int identity { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
    }

    public class TrainingEquipmentClassModel
    {
        [Key]
        public int identity { get; set; }
        public string EquipmentClass { get; set; }
    }

    public class TrainingEquipmentComponentModel
    {
        [Key]
        public int identity { get; set; }
        public string EquipmentComponent { get; set; }
    }

    public class TrainingFailureModeModel
    {
        [Key]
        public int identity { get; set; }
        public string FailureMode { get; set; }
    }

    public class TrainingManufacturerModel
    {
        [Key]
        public int identity { get; set; }
        public string Manufacturer { get; set; }
    }

    public class TrainingPlantCodeModel
    {
        [Key]
        public int identity { get; set; }
        public string PlantCode { get; set; }
        public string PlantName { get; set; }
    }
}
