using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Helpers
{
    public class FeedbackDBHelper
    {
        public FeedbackContext _context;


        public FeedbackDBHelper(FeedbackContext context)
        {
            this._context = context;
        }

        public void AddImplicitDocumentQuery(int searchId, string documentName, string query = "")
        {
            FBImplicitDocumentQueryModel feedback = new FBImplicitDocumentQueryModel();
            feedback.query = query;
            feedback.searchId = searchId;
            feedback.documentName = documentName;
            this._context.ImplicitDocumentQueryFeedbacks.Add(feedback);
            try
            {
                this._context.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException err)
            {
                //Ignore as it will ignore err updates with same query+user id
                //as opposed to searching first before adding
            }
        }

        public void AddCategoryFeedback(int searchId, string category, string tag, int rating)
        {
            FBCategoryRatingModel feedback = new FBCategoryRatingModel();
            feedback.searchId = searchId;
            feedback.category = category;
            feedback.name = tag;
            feedback.rating = rating;
            this._context.CategoryRating.Add(feedback);
            try
            {
                this._context.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException err)
            {
                //Ignore as it will ignore err updates with same query+user id
                //as opposed to searching first before adding
            }
        }

        public void AddImplicitDocumentResult(int searchId, string documentName, string tag = "")
        {
            FBImplicitDocumentResultModel feedback = new FBImplicitDocumentResultModel();
            feedback.tagSelected = tag;
            feedback.searchId = searchId;
            feedback.documentName = documentName;
            this._context.ImplicitDocumentResultFeedbacks.Add(feedback);
            try {
                this._context.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException err)
            {
                //Ignore as it will ignore err updates with same query+user id
                //as opposed to searching first before adding
            }
        }


        public Task<int> AddSearchQueryAsync(string userId, string userType, string givenName, string query)
        {
            FBSearchModel searchQuery = new FBSearchModel();
            searchQuery.userId = userId;
            searchQuery.userType = userType;
            searchQuery.givenName = givenName;
            searchQuery.query = query;
            this._context.Search.Add(searchQuery);
            try
            {
                return this._context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException err)
            {
                //Ignore as it will ignore err updates with same query+user id
                //as opposed to searching first before adding
                return null;
            }
        }

        public FBSearchModel AddSearchQuery(string userId, string userType, string givenName, string query)
        {
            FBSearchModel searchQuery = new FBSearchModel();
            searchQuery.userId = userId;
            searchQuery.userType = userType;
            searchQuery.givenName = givenName;
            searchQuery.query = query;
            this._context.Search.Add(searchQuery);
            this._context.SaveChanges();
            return searchQuery;
        }

        public void SaveReviewFeedback(int searchId, string documentName, int rating, string comment)
        {
            var feedbackModel = new FBReviewDocumentModel();
            feedbackModel.searchId = searchId;
            feedbackModel.documentName = documentName;
            feedbackModel.rating = rating;
            if(comment == null)
            {
                comment = "";
            }
            feedbackModel.comment = comment;
            var existing = this._context.ReviewDocument.FirstOrDefault(f => f.searchId.Equals(feedbackModel.searchId) && f.documentName.Equals(feedbackModel.documentName));

            if (existing == null)
            {
                this._context.ReviewDocument.Add(feedbackModel);
            }
            else
            {
                feedbackModel.revFeedbackId = existing.revFeedbackId;
                this._context.Entry(existing).CurrentValues.SetValues(feedbackModel);
            }
            try
            {
                this._context.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException err)
            {
                //Ignore as it will ignore err updates with same query+user id
                //as opposed to searching first before adding
            }
        }

        public void SaveRatingFeedback(int searchId, string documentName, int rating)
        {
            var feedbackModel = new FBRatingDocumentModel();
            feedbackModel.searchId = searchId;
            feedbackModel.documentName = documentName;
            feedbackModel.rating = rating;
            var existing = this._context.RatingDocument.FirstOrDefault(f => f.searchId.Equals(feedbackModel.searchId) && f.documentName.Equals(feedbackModel.documentName));

            if (existing == null)
            {
                this._context.RatingDocument.Add(feedbackModel);
            }
            else
            {
                feedbackModel.rateFeedbackId = existing.rateFeedbackId;
                this._context.Entry(existing).CurrentValues.SetValues(feedbackModel);
            }
            try
            {
                this._context.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException err)
            {
                //Ignore as it will ignore err updates with same query+user id
                //as opposed to searching first before adding
            }
        }
    }
}

public class RatingDTO{
    public string documentName;
    public int searchId;
    public int rating;
}

public class ReviewDTO
{
    public string documentName;
    public int searchId;
    public int rating;
    public string comment;
}

