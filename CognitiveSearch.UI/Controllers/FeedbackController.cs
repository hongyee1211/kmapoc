using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CognitiveSearch.UI.Controllers
{
    [Route("api/feedback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackDBHelper feedbackHandler;

        public FeedbackController(FeedbackContext feedbackContext)
        {
            this.feedbackHandler = new FeedbackDBHelper(feedbackContext);
        }

        [Route("MonitorDocumentQuery")]
        [HttpPost]
        public void MonitorDocumentQuery([FromForm] int searchFbId, [FromForm] string documentName, [FromForm] string query)
        {
            feedbackHandler.AddImplicitDocumentQuery(searchFbId, documentName, query);
        }

        [Route("MonitorDocumentResultTags")]
        [HttpPost]
        public void MonitorDocumentResultTags([FromForm] int searchFbId, [FromForm] string documentName, [FromForm] string tag)
        {
            feedbackHandler.AddImplicitDocumentResult(searchFbId, documentName, tag);
        }

        [Route("MonitorCategoryTags")]
        [HttpPost]
        public void MonitorCategoryTags([FromForm] int searchFbId, [FromForm] string category, [FromForm] string tag, [FromForm] int rating)
        {
            feedbackHandler.AddCategoryFeedback(searchFbId, category, tag, rating);
        }

        [Route("AnnotateTags")]
        [HttpPost]
        public void AnnotateTags([FromForm] int searchFbId,[FromForm] string documentName, [FromForm] string annotation, [FromForm] string tag)
        {
            feedbackHandler.AddAnnotation(searchFbId, documentName, annotation, tag);
        }
    }
}
