using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Models
{
    public class DirectLineToken
    {
        public string conversationId { get; set; }
        public string token { get; set; }
        public int expires_in { get; set; }
    }
    public class ChatViewModel
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string[] facetableFields { get; set; }

        public Dictionary<string, Filter[]> Filters { get; set; }

        public ChatViewModel()
        {
            string[] ecList = new string[] { "AIR EXPANDER", "ANALYSIS", "AUXILLIARY CABLE", "BLOWER", "CABLE", "COLUMN", "COMPRESSOR", "COMPRESSOR PISTON", "COMPRESSOR ROTARY", "CONTROL SYSTEM" };
            List<Filter> equipmentClass = CreateFilters(ecList);
            string[] cList = new string[] { "Accessory Drive", "Actuating Devices", "Actuating device", "Air Cooling", "Air Intake", "Anti Icing", "Anti Icing Valve", "Anti-Surge Valve", "Antisurge system", "Bearing", "Bearings" };
            List<Filter> component = CreateFilters(cList);
            //string[] fmList = new string[] { "Corrosion", "Health", "Safety", "Air Cooling", "Vibration" };
            //List<Filter> failureMode = CreateFilters(fmList);
            string[] mList = new string[] { "ABB", "ALFA LAVAL", "BAKER HUGHES", "BRAN & LUEBBE", "GE", "NUOVO PIGNONE", "PATTERSON", "RELIANCE" };
            List<Filter> manufacturer = CreateFilters(mList);
            string[] pcList = new string[] { "ABF", "AMSB", "DFP", "EPOMS", "GDCH", "PGB", "MLNG" };
            List<Filter> plantCode = CreateFilters(pcList);

            this.Filters = new Dictionary<string, Filter[]>
            {
                { "PlantCode", plantCode.ToArray()},
                { "Manufacturer", manufacturer.ToArray()},
                { "EquipmentClass", equipmentClass.ToArray()},
                { "Model", component.ToArray()},
            };
        }

        private List<Filter> CreateFilters(string[] values)
        {
            List<Filter> filters = new List<Filter>();
            foreach(string value in values)
            {
                int childLevel = 0;
                string[] children = new string[] { };
                string parent = null;
                if(value == "GE")
                {
                    children = new string[] { "NUOVO PIGNONE" };
                }
                else if (value == "NUOVO PIGNONE")
                {
                    parent = "GE";
                    childLevel = 1;
                }
                Filter filter = new Filter { value = value, children = children, childLevel = childLevel, parent = parent };
                filters.Add(filter);
            }
            return filters;
        }
    }

    public class Filter
    {
        public string value { get; set; }
        public string[] children { get; set; }
        public string? parent { get; set; }
        public int childLevel { get; set; }
    }
}
