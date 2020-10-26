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
    public class RatingController : Controller
    {
        private readonly FeedbackDBHelper dbHandler;

        public RatingController(
            FeedbackContext context)
        {
            this.dbHandler = new FeedbackDBHelper(context);
        }

        public IActionResult Index()
        {
            List<RatingRow> ratings = Retrieve();

            var viewModel = new RatingViewModel
            {
                feedbacks = ratings,
            };
            return View(viewModel);
        }

        [HttpPost]
        public List<RatingRow> Delete([FromForm]int id)
        {
            dbHandler.DeleteRating(id);
            return Retrieve();
        }

        private List<RatingRow> Retrieve()
        {
            string userId = Request.Cookies["userId"];
            return dbHandler.GetUserBadRatingFeedback(userId);
        }

    }
}