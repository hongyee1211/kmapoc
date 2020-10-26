using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Models;
using CognitiveSearch.UI.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Graph;
using Microsoft.Owin.Security.Provider;
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
            if (comment == null)
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

        public List<ReviewRow> GetAllReviewFeedback()
        {
            var output = this._context.Search
                .Join(this._context.ReviewDocument,
                    s => s.searchId,
                    r => r.searchId,

                    (s, r) => new ReviewRow
                    {
                        searchId = s.searchId,
                        rating = r.rating,
                        document = r.documentName,
                        comment = r.comment,
                        query = s.query,
                        user = s.givenName,
                        id = r.revFeedbackId,
                    }).ToList();

            return output;
        }

        public void DeleteReview(int id)
        {
            FBReviewDocumentModel entry = this._context.ReviewDocument.FirstOrDefault(x => x.revFeedbackId == id);
            if (entry != null)
            {
                this._context.ReviewDocument.Remove(entry);
                this._context.SaveChanges();
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

        public List<RatingRow> GetUserBadRatingFeedback(string userId)
        {
            var output = this._context.Search.Where(x => x.userId == userId)
                .Join(this._context.RatingDocument.Where(y=>y.rating == 1),
                    s => s.searchId,
                    r => r.searchId,

                    (s, r) => new RatingRow
                    {
                        searchId = s.searchId,
                        rating = r.rating,
                        document = r.documentName,
                        query = s.query,
                        user = s.givenName,
                        id = r.rateFeedbackId
                    }).ToList();

            return output;
        }

        public void DeleteRating(int id)
        {
            FBRatingDocumentModel entry = this._context.RatingDocument.FirstOrDefault(x => x.rateFeedbackId == id);
            if (entry != null)
            {
                this._context.RatingDocument.Remove(entry);
                this._context.SaveChanges();
            }
        }

        public void AddAnnotation(int searchId, string documentName, string annotation, string tag)
        {
            var feedbackModel = new FBTagAnnotationModel();
            feedbackModel.searchId = searchId;
            feedbackModel.documentName = documentName;
            feedbackModel.tag = tag;
            feedbackModel.annotation = annotation;
            var existing = this._context.TagAnnotations.FirstOrDefault(f => f.searchId.Equals(feedbackModel.searchId) && f.documentName.Equals(feedbackModel.documentName) && f.tag.Equals(feedbackModel.tag));

            if (existing == null)
            {
                this._context.TagAnnotations.Add(feedbackModel);
            }
            else
            {
                feedbackModel.annotationFeedbackId = existing.annotationFeedbackId;
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

        public void AddCategoryAnnotation(int searchId, string annotation, string tag, string current)
        {
            var feedbackModel = new FBCategoryTagAnnotationModel();
            feedbackModel.searchId = searchId;
            feedbackModel.tag = tag;
            feedbackModel.annotation = annotation;
            feedbackModel.current = current;
            feedbackModel.allow = 0;
            var existing = this._context.CategoryTagAnnotations.FirstOrDefault(f => f.searchId.Equals(feedbackModel.searchId) && f.tag.Equals(feedbackModel.tag));

            if (existing == null)
            {
                this._context.CategoryTagAnnotations.Add(feedbackModel);
            }
            else
            {
                feedbackModel.categoryAnnotationFeedbackId = existing.categoryAnnotationFeedbackId;
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

        public List<CategoryRow> GetAllCategoryTags()
        {
            var output = this._context.CategoryTagAnnotations.Where(x=>x.allow == 0)
                .Join(this._context.Search,
                    c => c.searchId,
                    s => s.searchId,
              
                    (c, s) => new CategoryRow
                    {
                        searchId = s.searchId,
                        category = c.tag,
                        annotation = c.annotation,
                        current = c.current,
                        name = s.givenName,
                        id = c.categoryAnnotationFeedbackId
                    }).ToList();
            return output;
        }

        public void DeleteCategoryTag(int tagId)
        {
            FBCategoryTagAnnotationModel entry = this._context.CategoryTagAnnotations.FirstOrDefault(x => x.categoryAnnotationFeedbackId == tagId);
            if (entry != null)
            {
                this._context.CategoryTagAnnotations.Remove(entry);
                this._context.SaveChanges();
            }
        }

        public FBCategoryTagAnnotationModel ApproveCategoryTag(int tagId)
        {
            FBCategoryTagAnnotationModel entry = this._context.CategoryTagAnnotations.FirstOrDefault(x => x.categoryAnnotationFeedbackId == tagId);
            if (entry != null)
            {
                this._context.Entry(entry).CurrentValues.SetValues(new { allow = 1});
                this._context.SaveChanges();
            }
            return entry;
        }

    }

    public class RatingDTO
    {
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
}

