using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Helpers;
using CognitiveSearch.UI.Models;
using CognitiveSearch.UI.Models.Data;
using Microsoft.AspNetCore.Mvc;

namespace CognitiveSearch.UI.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly SubscribeDBHelper dbHandler;

        public SubscribeController(
            SubscribeContext context)
        {
            this.dbHandler = new SubscribeDBHelper(context);
        }

        public IActionResult Index()
        {
            List<SubscribeSearchModel> subscriptions = Retrieve();

            var viewModel = new SubscribeViewModel
            {
                subscriptions = subscriptions,
            };
            return View(viewModel);
        }

        [HttpPost]
        public List<SubscribeSearchModel> Unsubscribe(string query)
        {
            string userId = Request.Cookies["userId"];
            dbHandler.DeleteSearchSubscription(userId, query);
            return Retrieve();
        }

        [HttpPost]
        public List<SubscribeSearchModel> Subscribe(string query, int count)
        {
            string userId = Request.Cookies["userId"];
            string userType = Request.Cookies["userType"];
            string givenName = Request.Cookies["givenName"];
            string email = Request.Cookies["email"];
            dbHandler.AddSearchQuery(userId, userType, givenName, query, count, email);
            return Retrieve();
        }

        private List<SubscribeSearchModel> Retrieve()
        {
            string userId = Request.Cookies["userId"];
            return dbHandler.GetAllUserSubscriptions(userId);
        }

        [HttpPost, HttpGet]
        public void test()
        {
            Console.WriteLine("this");
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
        }

    }
}