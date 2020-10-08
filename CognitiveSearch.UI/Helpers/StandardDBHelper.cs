using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Helpers
{
    public class StandardDBHelper
    {
        public StandardContext _context;


        public StandardDBHelper(StandardContext context)
        {
            this._context = context;
        }

        public string[] FindAllStandard(IEnumerable<string> stringFacets, SearchFacet[] facets)
        {
            IEnumerable<string> components, equipmentClass, failureMode;
            components = equipmentClass = failureMode = stringFacets;

            foreach (SearchFacet facet in facets)
            {
                switch (facet.Key)
                {
                    case "EquipmentClass":
                        equipmentClass = equipmentClass.Union(facet.Value);
                        break;
                    case "FailureMode":
                        failureMode = failureMode.Union(facet.Value);
                        break;
                    case "Components":
                        components = components.Union(facet.Value);
                        break;
                    default:
                        break;
                }
            }

            return this._context.Standards
                .Where(x => components.Contains(x.Component.ToLower()) || equipmentClass.Contains(x.EquipmentClass.ToLower()) || failureMode.Contains(x.FailureMode.ToLower()))
                .Select(x => x.DocumentName).Distinct().ToArray<string>();
        }

        
    }
}
