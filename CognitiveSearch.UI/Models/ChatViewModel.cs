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
            string[] ecList = new string[] { "AUXILLIARY CABLE", "CABLE", "EQUIPMENT POWER", "EQUIPMENT", "GENERATOR", "MOTOR", "MOTOR", "PANEL",
                "POWER RELAY", "TRANSFORMER", "ANALYSIS", "CONTROL SYSTEM", "FILTER", "FREON GAS", "PRESSURE GAUGE", "SENSOR", "TRANSMISSION", "BLOWER",
                "COLUMN", "UTILITY EJECTOR", "AIR EXPANDER", "EXPANDER", "TURBO EXPANDER", "STEAM EXPANDER", "FAN", "FILTER", "PUMP CENTRIFUGAL",
                "PUMP RECIPOCRATING", "SAFETY VALVE", "ROTARY", "STATE", "SUBMERSIBLE PUMP", "STEAM TURBINE", "TANK", "VALVE", "VESSEL", "GAS TURBINE",
                "COMPRESSOR", "COMPRESSOR ROTARY", "COMPRESSOR PISTON", "ENGINE", "GEAR", "PUMP CENTRIFUGAL", "PUMP RECIPOCRATING", "ROTARY VALVE", "PUMP" };
            List<Filter> equipmentClass = CreateFilters(ecList);
            string[] cList = new string[] { "Accessory Drive", "Actuating Devices", "Actuating device", "Air Cooling", "Air Intake", "Anti Icing", "Anti Icing Valve", "Anti-Surge Valve", "Antisurge system", "Bearing", "Bearings" };
            List<Filter> component = CreateFilters(cList);
            //string[] fmList = new string[] { "Corrosion", "Health", "Safety", "Air Cooling", "Vibration" };
            //List<Filter> failureMode = CreateFilters(fmList);
            string[] mList = new string[] { "ABB","DRESSER","DRESSER-RAND","INGERSOLL-RAND","ROLLS ROYCE","BROOK CROMPTON","BROOK HANSEN","ALLWEILER",
                "ALFA LAVAL","ALSTOM","MITSUBISHI HEAVY INDUSTRIES","HYUNDAI HEAVY INDUSTRIES","ATLAS COPCO","SIEMENS","BAKER HUGHES","BALDOR","BAUER",
                "BAYLEY","BEGEMANN","BLACKMER","BLAGDON","BOSCH","BRAN & LUEBBE","BROWN & ROOT","BUGATTI","BURGESS","CARRIER","CATERPILLAR","CLYDE UNION",
                "COOPER ENERGY SERVICES","CROMPTON","CUMMINS","DAVID BROWN","DAWSON & DOWNIE","DELAVAL","DEMAG","DETROIT DIESEL","DOOSAN","DONGYANG",
                "DONGHWA","DRESSER-ROOTS","EBARA","EDWARDS","ELLIOT","FAVCO","FLAKT","FLENDER","FLOWSERVE","FLOWGUARD","FMC","FUJI","GARDNER-DENVER",
                "GE","NUOVO PIGNONE","GENERAL MOTORS","GOULDS","GRAFFENSTADEN","GRUNDFOS","HALBERG","HALIFAX","HAMILTON","HANSEN","HARBOUR MARINE",
                "HASKEL","HITACHI","HITACHI HEAVY INDUSTRIES","TOSHIBA","MITSUBISHI","HOFFMAN","HOLTEC","HONDA","HOWDEN","HSL ENGINEERING","HYUNDAI",
                "INGERSOLL-DRESSER","JAPAN STEEL","JAPAN AIRCRAFT","JAPAN MACHINERY COMPANY","JOHN DEERE","KAERCHER","KAESER","KAJI IRON WORKS","KATO",
                "KOBELCO","KOBE STEEL","KONE","KRUGER","KRUPP","KUBOTA","LIGHTNIN","LINDE","LOHER","LUFKIN","MILTON ROY","MAN TURBO","MANNESMANN",
                "MARELLI MOTORI","MARUSHICHI","MITSUBISHI ELECTRIC COMPANY","MITSUI ENGINEERING","MITSUI","MONROE","NIIGATA","NIKKISO","NISSAN DIESEL",
                "OMRON","OHIO ELECTRIC","PATTERSON","PROMINENT","RANDOLPH GEAR","RELIANCE","REXROTH","RICKMEIER","ROBBINS & MYERS",
                "ROBERT BIRKENBEUL","ROCKWELL AUTOMATION","ROHR SYSTEM TECHNIK","ROOTS","ROTOR","ROTORK","RUHR POMPEN","SAAB","SAAB IVECO","SANDPIPER",
                "SANWA","SCHEERLE","SCHENCK","SEAT","SEIKA CORPORATION","SHIMADZU","SHIN NIPPON","SHOWA DENKI","SOLAR TURBINES","STAMFORD","STAHL",
                "STORK","SULLAIR","SULZER","SUMITOMO","SUNSTRAND","TAIKO KIKAI","TECO","THOMASSEN","THYSSEN","THYSSENKRUPP","TRACTOR MALAYSIA",
                "TUTHILL","UMW","UNITED CENTRIFUGAL PUMP","VIKING MARINE","VOITH","VOLVO","VOLVO PENTA","VOGEL","WARREN RUPP","WESTERN ELECTRIC",
                "WEG","WEIR","WESTINGHOUSE","WILDEN","WOOD","WINKELMANN","WILLIAMS","WORTHINGTON","YAMADA","YOKOTA","ZIEHL" };
            List<Filter> manufacturer = CreateFilters(mList);
            string[] pcList = new string[] { "PAPL", "PUPL", "PDPL", "PCM", "PCINO", "PC", "PHCO", "PCSB", "ZLNG","PCG", "PCML", "ABF", "PCFK", "PML",
                "MTBE", "PDH", "EMSB", "PMSB", "PEMSB", "KPSB", "AMSB", "PASB", "PCLDPE", "MLNG", "PGB", "PEGT", "PJH", "PPTSB", "PMSSB", "GDC", "KLCCH",
                "MRCSB", "GDCKLIA", "ITP", "PMTSB", "SUPSB", "PSI", "LISB", "PSB", "PETCO", "NGV", "RGT", "KLCCP", "PCMC", "KLCCPM", "PTTSB", "MPO", "DFP",
                "PRSB", "KLCCPH", "PFLNG1", "PPSB", "PL9SB", "VOLTAIRE", "RGT2", "RGT3", "PET-DGT", "PET-ICT", "PRPC", "GDCM", "GDCH", "MKSB", "KLCCREIT",
                "ICSB", "KLCONVENTION", "KLCCPS", "CTSB", "HLSB", "KLCCUH", "PET", "VPSB", "PFLNG2", "PTSSB", "PGTSSB", "CEFS", "PLNG2", "TTMMSB", "PIC",
                "PCML-", "PCP", "EPOMS", "PTVSB", "HCU", "KLCC", "TPC", "PCMI", "PCOSB", "SDA" };
            List<Filter> plantCode = CreateFilters(pcList);
            string[] mdList = new string[] { "Frame 5", "Frame 6", "AN200" };
            List<Filter> model = CreateFilters(mdList);
            string[] dList = new string[] { "Placeholder" };
            List<Filter> discipline = CreateFilters(dList);

            this.Filters = new Dictionary<string, Filter[]>
            {
                { "PlantCode", plantCode.ToArray()},
                { "Manufacturer", manufacturer.ToArray()},
                { "EquipmentClass", equipmentClass.ToArray()},
                { "Model", model.ToArray()},
                { "Discipline", discipline.ToArray()},
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
