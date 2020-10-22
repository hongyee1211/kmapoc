using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Helpers
{
    public class GremlinConnector
    {
        private GremlinConfig config;
        private GremlinServer gremlinServer;
        public GremlinConnector()
        {
            //preferably make this parameter in the future
            this.config = ConfigurationHandler.getGremlinConfig();
            string dbCollection = "/dbs/" + config.DatabaseName + "/colls/" + config.CollectionName;
            gremlinServer = new GremlinServer(config.Host, 443, true, dbCollection, config.PrimaryKey);

            var gremlinClient = new GremlinClient(gremlinServer, new GraphSONReader(), new GraphSONWriter());
            Console.WriteLine("Connection Successful");
        }

        public string getPlantCodeDetails(string[] plantCode)
        {
            string gremlinQuery = "g.V()";
            if (plantCode.Length > 0)
            {
                gremlinQuery += gremlinHasBuilder(new string[2] { "OPU", "PlantCode" }, plantCode);
            }
            else
            {
                gremlinQuery +=".hasLabel('OPU')";
            }
            using (var client = new GremlinClient(gremlinServer))
            {
                var resultSet = client.SubmitAsync<dynamic>(gremlinQuery).Result;
                //var resultSet = client.SubmitAsync<dynamic>("g.V().has('OPU','PlantCode','"+ plantCode[0] + "')").Result;
                string opu = JsonConvert.SerializeObject(resultSet);
                return opu;
            }
        }

        public string getPlantCodeUnitRelationship(string[] plantCode)
        {
            string gremlinQuery = "g.V()";
            if (plantCode.Length > 0)
            {
                gremlinQuery += gremlinHasBuilder(new string[2] { "OPU", "PlantCode" }, plantCode);
            }
            else
            {
                gremlinQuery += ".hasLabel('OPU')";
            }
            gremlinQuery += ".outE('Has')";
            using (var client = new GremlinClient(gremlinServer))
            {
                var resultSet = client.SubmitAsync<dynamic>(gremlinQuery).Result;
                //var resultSet = client.SubmitAsync<dynamic>("g.V().has('OPU','PlantCode','" + plantCode[0] + "').outE('Has')").Result;
                string opuLink = JsonConvert.SerializeObject(resultSet);
                return opuLink;
            }
        }

        public string getUnitEquipmentRelationship(string[] plantCode, string[] equipmentModel, string[] equipmentClass, string[] manufacturer)
        {
            string gremlinQuery = "g.V()";
            if (plantCode.Length > 0)
            {
                gremlinQuery += gremlinHasBuilder(new string[2] { "OPU", "PlantCode" }, plantCode);
            }
            else
            {
                gremlinQuery += ".hasLabel('OPU')";
            }
            gremlinQuery += ".out().out()";
            gremlinQuery += gremlinHasBuilder(new string[2] { "Equipment", "EquipmentModel" }, equipmentModel);
            gremlinQuery += gremlinHasBuilder(new string[1] { "Manufacturer" }, manufacturer);
            gremlinQuery += gremlinHasBuilder(new string[1] { "EquipmentClass" }, equipmentClass);
            gremlinQuery += ".outE('is_located_at')";

            using (var client = new GremlinClient(gremlinServer))
            {
                var resultSet = client.SubmitAsync<dynamic>(gremlinQuery).Result;
                string equipmentLink = JsonConvert.SerializeObject(resultSet);
                return equipmentLink;
            }
        }

        public string getEquipmentDetails(string[] plantCode, string[] equipmentModel, string[] equipmentClass, string[] manufacturer)
        {
            string gremlinQuery = "g.V()";
            if (plantCode.Length > 0)
            {
                gremlinQuery += gremlinHasBuilder(new string[2] { "OPU", "PlantCode" }, plantCode);
            }
            else
            {
                gremlinQuery += ".hasLabel('OPU')";
            }
            gremlinQuery += ".out().out()";
            gremlinQuery += gremlinHasBuilder(new string[2] { "Equipment", "EquipmentModel"}, equipmentModel);
            gremlinQuery += gremlinHasBuilder(new string[2] { "Equipment", "Manufacturer" }, manufacturer);
            gremlinQuery += gremlinHasBuilder(new string[1] { "EquipmentClass" }, equipmentClass);

            using (var client = new GremlinClient(gremlinServer))
            {
                var resultSet = client.SubmitAsync<dynamic>(gremlinQuery).Result;
                string equipmentLink = JsonConvert.SerializeObject(resultSet);
                return equipmentLink;
            }
        }

        private string gremlinHasBuilder(string[] properties, string[] nodes)
        {
            if (properties.Length > 0 && nodes.Length > 0)
            {
                string gremlinQuery = ".has(";
                for (int i = 0; i < properties.Length; i++)
                {
                    gremlinQuery += "'" + properties[i] + "',";
                }
                gremlinQuery += " within(";
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (i != 0)
                    {
                        gremlinQuery += ',';
                    }
                    gremlinQuery += "'" + nodes[i] + "'";
                }
                gremlinQuery += "))";
                return gremlinQuery;
            }
            else
            {
                //not yet supported handling
                return "";
            }
        }
    }

    public class GremlinConfig
    {
        public string Host { get; set; }
        public string PrimaryKey { get; set; }
        public string CollectionName { get; set; }
        public string DatabaseName { get; set; }
    }

}
