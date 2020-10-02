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

            List<FBReviewDocumentModel> feedbacks = retrieveFeedback(userId);

            var viewModel = new RatingsViewModel
            {
                feedbacks = feedbacks,
            };
            return View(viewModel);
        }

        [HttpPost]
        public List<FBReviewDocumentModel> deleteFeedback(FBReviewDocumentModel feedbackVal)
        {
            FBReviewDocumentModel entry = this._context.ReviewDocument.FirstOrDefault(x => x.searchId.Equals(feedbackVal.searchId) && x.documentName.Equals(feedbackVal.documentName));
            if (entry != null) {
                this._context.ReviewDocument.Remove(entry);
            }
            this._context.SaveChanges();
            var userId = Request.Cookies["userId"];
            return retrieveFeedback(userId);
        }

        private List<FBReviewDocumentModel> retrieveFeedback(string userId)
        {
            return this._context.ReviewDocument.ToList();

        }
    }
}