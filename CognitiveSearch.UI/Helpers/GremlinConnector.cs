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
            if (plantCode.Length > 0)
            {
                using (var client = new GremlinClient(gremlinServer))
                {
                    var resultSet = client.SubmitAsync<dynamic>("g.V().has('OPU','PlantCode','"+ plantCode[0] + "')").Result;
                    string opu = JsonConvert.SerializeObject(resultSet);
                    return opu;
                }
            }
            return null;
        }

        public string getPlantCodeUnitRelationship(string[] plantCode)
        {
            if (plantCode.Length > 0)
            {
                using (var client = new GremlinClient(gremlinServer))
                {
                    var resultSet = client.SubmitAsync<dynamic>("g.V().has('OPU','PlantCode','" + plantCode[0] + "').outE('Has')").Result;
                    string opuLink = JsonConvert.SerializeObject(resultSet);
                    return opuLink;
                }
            }
            return null;
        }

        public string getUnitEquipmentModelRelationship(string[] plantCode, string[] equipmentModel) 
        {
            if (plantCode.Length > 0 && equipmentModel.Length > 0)
            {
                using (var client = new GremlinClient(gremlinServer))
                {
                    var resultSet = client.SubmitAsync<dynamic>("g.V().has('OPU', 'PlantCode', '" + plantCode[0] + "').out().out().has('Equipment', 'EquipmentModel', '" + equipmentModel[0] + "').outE('is_located_at')").Result;
                    string equipmentLink = JsonConvert.SerializeObject(resultSet);
                    return equipmentLink;
                }
            }
            return null;
        }

        public string getEquipmentModelDetails(string[] plantCode, string[] equipmentModel)
        {
            if (plantCode.Length > 0 && equipmentModel.Length > 0)
            {
                using (var client = new GremlinClient(gremlinServer))
                {
                    var resultSet = client.SubmitAsync<dynamic>("g.V().has('OPU', 'PlantCode', '" + plantCode[0] + "').out().out().has('Equipment', 'EquipmentModel', '" + equipmentModel[0] + "')").Result;
                    string equipmentLink = JsonConvert.SerializeObject(resultSet);
                    return equipmentLink;
                }
            }
            return null;
        }
        public void Query(string[] plantCode, string[] equipmentModel){
            string opu = getPlantCodeDetails(plantCode);
            string opuLink = getPlantCodeUnitRelationship(plantCode);
            string equipmentLink = getUnitEquipmentModelRelationship(plantCode, equipmentModel);
            string equipment = getEquipmentModelDetails(plantCode, equipmentModel);
            D3Converter.StaticQuery(opu, opuLink, equipmentLink, equipment);

            Console.WriteLine("Query Successful");
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
