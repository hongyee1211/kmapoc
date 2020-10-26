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
        private readonly SubscribeDBHelper sDbHandler;
        private readonly FeedbackDBHelper fbDbHandler;

        public SubscribeController(
            SubscribeContext sContext,
            FeedbackContext fContext)
        {
            this.sDbHandler = new SubscribeDBHelper(sContext);
            this.fbDbHandler = new FeedbackDBHelper(fContext);
        }

        public IActionResult Index()
        {
            List<SubscribeSearchModel> subscriptions = RetrieveSubscriptions();
            List<RatingRow> ratings = RetrieveRating();
            var viewModel = new SubscribeViewModel
            {
                subscriptions = subscriptions,
                feedbacks = ratings,
            };
            return View(viewModel);
        }

        [HttpPost]
        public List<SubscribeSearchModel> Unsubscribe(string query)
        {
            string userId = Request.Cookies["userId"];
            sDbHandler.DeleteSearchSubscription(userId, query);
            return RetrieveSubscriptions();
        }

        [HttpPost]
        public List<SubscribeSearchModel> Subscribe(string query, int count)
        {
            string userId = Request.Cookies["userId"];
            string userType = Request.Cookies["userType"];
            string name = Request.Cookies["displayName"];
            sDbHandler.AddSearchQuery(userId, userType, name, query, count);
            return RetrieveSubscriptions();
        }

        private List<SubscribeSearchModel> RetrieveSubscriptions()
        {
            string userId = Request.Cookies["userId"];
            return sDbHandler.GetAllUserSubscriptions(userId);
        }

        [HttpPost]
        public List<RatingRow> DeleteRating([FromForm] int id)
        {
            fbDbHandler.DeleteRating(id);
            return RetrieveRating();
        }

        private List<RatingRow> RetrieveRating()
        {
            string userId = Request.Cookies["userId"];
            return fbDbHandler.GetUserBadRatingFeedback(userId);
        }

    }
}