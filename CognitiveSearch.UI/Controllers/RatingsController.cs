using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CognitiveSearch.UI.Controllers
{
    public class RatingsController : Controller
    {
        private readonly FeedbackContext _context;

        public RatingsController(
            FeedbackContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            var userId = Request.Cookies["userId"];

            List<FeedbackModel> feedbacks = retrieveFeedback(userId);

            var viewModel = new RatingsViewModel
            {
                feedbacks = feedbacks,
            };
            return View(viewModel);
        }

        [HttpPost]
        public List<FeedbackModel> deleteFeedback(FeedbackModel feedbackVal)
        {
            FeedbackModel entry = this._context.Feedbacks.FirstOrDefault(x => x.userID.Equals(feedbackVal.userID) && x.documentName.Equals(feedbackVal.documentName) && x.query.Equals(feedbackVal.query));
            if (entry != null) {
                this._context.Feedbacks.Remove(entry);
            }
            this._context.SaveChanges();
            var userId = Request.Cookies["userId"];
            return retrieveFeedback(userId);
        }

        private List<FeedbackModel> retrieveFeedback(string userId)
        {
            return this._context.Feedbacks.Where(feedback => feedback.userID.Equals(userId)).ToList();

        }
    }
}