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
        private readonly FeedbackDBHelper dbHandler;

        public CategoryController(
            FeedbackContext context)
        {
            this.dbHandler = new FeedbackDBHelper(context);
        }

        public IActionResult Index()
        {
            var dept = Request.Cookies["discipline"];
            if (dept == "Knowledge Management")
            {
                List<CategoryRow> categories = dbHandler.GetAllCategoryTags();
                var viewModel = new CategoryViewModel
                {
                    categories = categories,
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
            dbHandler.DeleteCategoryTag(tagId);
            List<CategoryRow> categories = dbHandler.GetAllCategoryTags();
            return categories;
        }

        [HttpPost]
        public List<CategoryRow> ApproveAnnotation([FromForm] int tagId)
        {
            dbHandler.ApproveCategoryTag(tagId);
            List<CategoryRow> categories = dbHandler.GetAllCategoryTags();
            return categories;
        }

    }
}