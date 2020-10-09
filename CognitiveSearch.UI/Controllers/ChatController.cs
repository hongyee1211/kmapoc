using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CognitiveSearch.UI.Bot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace CognitiveSearch.UI.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Send([FromBody] BotRequestModel c)
        {
            BotRequestModel x = new BotRequestModel();
            x.context = c.context;
            x.text = c.text;
            ChatModel request = new ChatModel(x);
            //request.context["branch"] = "Query";
            BotOutputModel output = await BotManager.Process(request);
            return Json(output);
        }
    }

    public class ChatbotRequestModel
    {
        [Required]
        public string text { get; set; }

        [Required]
        public Hashtable context { get; set; }
    }
}
