using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Helpers
{
    public static class ConfigurationHandler
    {
        private static string botKey;
        private static string mrcKey;
        private static GremlinConfig gremlinConfig;
        public static void initialize(IConfiguration Configuration)
        {
            botKey = Configuration.GetSection("BotFramework")["Authorization"];
            gremlinConfig = new GremlinConfig
            {
                Host = Configuration.GetSection("CosmosGremlinConfig")["Host"],
                PrimaryKey = Configuration.GetSection("CosmosGremlinConfig")["PrimaryKey"],
                CollectionName = Configuration.GetSection("CosmosGremlinConfig")["CollectionName"],
                DatabaseName = Configuration.GetSection("CosmosGremlinConfig")["DatabaseName"]
            };
            mrcKey = Configuration.GetSection("MRC")["Key"];
        }

        public static string getBotKey()
        {
            return botKey;
        }

        public static string getMRCKey()
        {
            return mrcKey;
        }

        public static GremlinConfig getGremlinConfig()
        {
            return gremlinConfig;
        }
    }
}
