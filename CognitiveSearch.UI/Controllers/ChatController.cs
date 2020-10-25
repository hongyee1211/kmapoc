using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CognitiveSearch.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Newtonsoft.Json;
using System.Configuration;
using CognitiveSearch.UI.Helpers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json.Linq;

namespace CognitiveSearch.UI.Controllers
{
    public class ChatController : Controller
    {
        private IConfiguration _configuration { get; set; }
        private DocumentSearchClient _docSearch { get; set; }
        private string _configurationError { get; set; }

        public ChatController(
            IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeDocSearch();
        }

        private void InitializeDocSearch()
        {
            try
            {
                _docSearch = new DocumentSearchClient(_configuration);
            }
            catch (Exception e)
            {
                _configurationError = $"The application settings are possibly incorrect. The server responded with this message: " + e.Message.ToString();
            }
        }

        [HttpGet]
        /*static void getFailureCount(string[] args)*/
        public IActionResult getFailureCount(string nodeName, string functionalLocation, string typeLevel, string chartType)
        {
            var result = "";
            string connectionString = "Data Source=ptsg-5pekapocsqldb01.database.windows.net;Initial Catalog=kapoc-sql-db;"
                 + "User ID=manager;Password=Poc@2020;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Integrated Security=False";
           
            string queryString = "";
            string whereString = "";
            if (typeLevel == "type1") {
                whereString = "OPU = '" + nodeName + "' AND FunctionalLocation IN " + functionalLocation;
            } else if (typeLevel == "type3") {
                whereString = "FunctionalLocation LIKE '%" + nodeName + "%'";
            }

           if (chartType == "Pie") {
                queryString = "SELECT Priority, COUNT(*) AS FailureCount FROM ProblemAnalysis WHERE " + whereString + " GROUP BY Priority ORDER BY Priority for JSON AUTO";
            } else if (chartType == "Histogram") {
                queryString = "SELECT ProblemID, COUNT(*) AS FailureCount FROM ProblemAnalysis WHERE "+ whereString + " GROUP BY ProblemID ORDER BY ProblemID for JSON AUTO";
            } else if (chartType == "Line") {
                queryString = "SELECT FailureDateYr, COUNT(*) AS FailureCount FROM ProblemAnalysis WHERE " + whereString + " GROUP BY FailureDateYr ORDER BY FailureDateYr for JSON AUTO";
            }

            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                /*command.ExecuteNonQuery();*/

                SqlDataReader reader = command.ExecuteReader();

                // Call Read before accessing data.

                while (reader.Read())
                {
                    result = reader.GetValue(0).ToString();
                }
                
                // Call Close when done reading.
                reader.Close();
                
            }

            return new JsonResult(result);
        }

        public async Task<ActionResult> Index()
        {
            var secret = ConfigurationHandler.getBotKey();

            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://directline.botframework.com/v3/directline/tokens/generate");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secret);

            var userId = $"dl_{Guid.NewGuid()}";

            request.Content = new StringContent(
                JsonConvert.SerializeObject(
                    new { User = new { Id = userId } }),
                    Encoding.UTF8,
                    "application/json");

            var response = await client.SendAsync(request);
            string token = String.Empty;

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<DirectLineToken>(body).token;
            }

            var config = new ChatViewModel()
            {
                Token = token,
                UserId = userId,
                facetableFields = _docSearch.Model.Facets.Select(k => k.Name).ToArray(),
                discipline = Request.Cookies["discipline"]
            };

            return View(config);
        }
    }
}
