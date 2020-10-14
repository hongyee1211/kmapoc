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

namespace CognitiveSearch.UI.Controllers
{
    public class ChatController : Controller
    {
        //public IActionResult Index()
        //{
        //    var model = new ChatViewModel();
        //    model.guid = Guid.NewGuid().ToString();
        //    return View(model);
        //}
        public async Task<ActionResult> Index()
        {
            var secret = SecretHandler.getBotKey();

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
                UserId = userId
            };

            return View(config);
        }
    }
}
