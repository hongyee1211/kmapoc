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
    public class CategoryController : Controller
    {
        private readonly FeedbackDBHelper fbDbHandler;
        private readonly TrainingDBHelper tDbHandler;

        public CategoryController(
            FeedbackContext fbContext,
            TrainingContext tContext)
        {
            this.fbDbHandler = new FeedbackDBHelper(fbContext);
            this.tDbHandler = new TrainingDBHelper(tContext);
        }

        public IActionResult Index()
        {
            var dept = Request.Cookies["discipline"];
            if (dept == "Knowledge Management")
            {
                List<CategoryRow> categories = fbDbHandler.GetAllCategoryTags();
                List<ReviewRow> reviews = fbDbHandler.GetAllReviewFeedback();
                var viewModel = new AdminViewModel
                {
                    categories = categories,
                    reviews = reviews,
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public List<CategoryRow> DeleteAnnotation([FromForm] int tagId)
        {
            fbDbHandler.DeleteCategoryTag(tagId);
            List<CategoryRow> categories = fbDbHandler.GetAllCategoryTags();
            return categories;
        }

        [HttpPost]
        public List<CategoryRow> ApproveAnnotation([FromForm] int tagId)
        {
            var approved = fbDbHandler.ApproveCategoryTag(tagId);
            tDbHandler.UpdateDBIndex(approved);
            List<CategoryRow> categories = fbDbHandler.GetAllCategoryTags();
            return categories;
        }

        [HttpPost]
        public List<ReviewRow> DeleteReview([FromForm] int id)
        {
            fbDbHandler.DeleteReview(id);
            List<ReviewRow> reviews = fbDbHandler.GetAllReviewFeedback();
            return reviews;
        }

    }
}