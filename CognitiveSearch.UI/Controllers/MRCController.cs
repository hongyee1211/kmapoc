using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CognitiveSearch.UI.Controllers
{
    [Route("api/mrc")]
    [ApiController]
    public class MRCController : ControllerBase
    {
        private readonly string secret;

        public MRCController()
        {
            this.secret = ConfigurationHandler.getMRCKey();
        }

        [Route("Ask")]
        [HttpPost]
        public async Task<MRCObject> getAnswer([FromForm]string title, [FromForm] string content,  [FromForm] string query)
        {
            using var client = new HttpClient();
            title = "Controlled Ventilation of New Hot insulation using flexible metal air ducting.xls";
            string uri = "https://mrcdemo-v1.azurewebsites.net/api/mrcdemo2?code=" + this.secret +
                "&document=" + content + "&query=" + query;
            var result = await client.GetAsync(uri);
            result.EnsureSuccessStatusCode();
            var contentStream = await result.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            using var jsonReader = new JsonTextReader(streamReader);

            JsonSerializer serializer = new JsonSerializer();

            try
            {
                return serializer.Deserialize<MRCObject>(jsonReader);
            }
            catch (JsonReaderException err)
            {
                Console.WriteLine("Invalid JSON.");
                throw err;
            }
        }
    }

    public class MRCObject
    {
        public string answer { get; set; }
        public string query { get; set; }
        public string title { get; set; }
        public string paragraph { get; set; }
    }
}
