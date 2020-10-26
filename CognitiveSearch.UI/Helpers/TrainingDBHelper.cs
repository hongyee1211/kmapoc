using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Helpers
{
    public class TrainingDBHelper
    {
        public TrainingContext _context;


        public TrainingDBHelper(TrainingContext context)
        {
            this._context = context;
        }

        public void UpdateDBIndex(FBCategoryTagAnnotationModel annotation)
        {
            Delete(annotation.tag, annotation.current);
            Add(annotation.tag, annotation.annotation);
            this._context.SaveChanges();
        }

        private void Delete(string tag, string from)
        {
            switch (from)
            {
                case "EquipmentClass":
                    {
                        var found = this._context.EquipmentClass.FirstOrDefault(x => x.EquipmentClass == tag);
                        if (found != null)
                        {
                            this._context.EquipmentClass.Remove(found);
                        }
                    }
                    break;
                case "PlantCode":
                    {
                        var found = this._context.PlantCode.FirstOrDefault(x => x.PlantName == tag);
                        if (found != null)
                        {
                            this._context.PlantCode.Remove(found);
                        }
                    }
                    break;
                case "FailureMode":
                    {
                        var found = this._context.FailureMode.FirstOrDefault(x => x.FailureMode == tag);
                        if (found != null)
                        {
                            this._context.FailureMode.Remove(found);
                        }
                    }
                    break;
                case "Manufacturer":
                    {
                        var found = this._context.Manufacturer.FirstOrDefault(x => x.Manufacturer == tag);
                        if (found != null)
                        {
                            this._context.Manufacturer.Remove(found);
                        }
                    }
                    break;
                case "Component":
                    {
                        var found = this._context.Component.FirstOrDefault(x => x.EquipmentComponent == tag);
                        if (found != null)
                        {
                            this._context.Component.Remove(found);
                        }
                    }
                    break;
                case "Abbreviation":
                    {
                        var found = this._context.Abbreviation.FirstOrDefault(x => x.Abbreviation == tag);
                        if (found != null)
                        {
                            this._context.Abbreviation.Remove(found);
                        }
                    }
                    break;
            }
        }

        private void Add(string tag, string to)
        {
            switch (to)
            {
                case "EquipmentClass":
                    {
                        var found = this._context.EquipmentClass.FirstOrDefault(x => x.EquipmentClass == tag);
                        if (found == null)
                        {
                            this._context.EquipmentClass.Add(new TrainingEquipmentClassModel() { EquipmentClass=tag} );
                        }
                    }
                    break;
                case "PlantCode":
                    {
                        var found = this._context.PlantCode.FirstOrDefault(x => x.PlantName == tag);
                        if (found == null)
                        {
                            this._context.PlantCode.Add(new TrainingPlantCodeModel() { PlantName = tag, PlantCode = null });
                        }
                    }
                    break;
                case "FailureMode":
                    {
                        var found = this._context.FailureMode.FirstOrDefault(x => x.FailureMode == tag);
                        if (found == null)
                        {
                            this._context.FailureMode.Add(new TrainingFailureModeModel() { FailureMode = tag });
                        }
                    }
                    break;
                case "Manufacturer":
                    {
                        var found = this._context.Manufacturer.FirstOrDefault(x => x.Manufacturer == tag);
                        if (found == null)
                        {
                            this._context.Manufacturer.Add(new TrainingManufacturerModel() { Manufacturer = tag });
                        }
                    }
                    break;
                case "Component":
                    {
                        var found = this._context.Component.FirstOrDefault(x => x.EquipmentComponent == tag);
                        if (found == null)
                        {
                            this._context.Component.Add(new TrainingEquipmentComponentModel() { EquipmentComponent = tag });
                        }
                    }
                    break;
                case "Abbreviation":
                    {
                        var found = this._context.Abbreviation.FirstOrDefault(x => x.Abbreviation == tag);
                        if (found == null)
                        {
                            this._context.Abbreviation.Add(new TrainingAbbreviationModel() { Abbreviation = tag, Description = null});
                        }
                    }
                    break;
            }
        }

    }
}
