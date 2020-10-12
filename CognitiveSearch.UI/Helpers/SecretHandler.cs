using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Helpers
{
    public static class SecretHandler
    {
        private static string botKey;
        public static void initialize(IConfiguration Configuration)
        {
            botKey = Configuration.GetSection("BotFramework")["Authorization"];
        }

        public static string getBotKey()
        {
            return botKey;
        }
    }
}
