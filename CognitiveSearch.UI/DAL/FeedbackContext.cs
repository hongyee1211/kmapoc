using CognitiveSearch.UI.Models;
using CognitiveSearch.UI.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace CognitiveSearch.UI.DAL
{
    public class FeedbackContext : DbContext
    {
        public FeedbackContext(DbContextOptions<FeedbackContext> options) : base(options)
        {
        }

        public DbSet<FeedbackModel> Feedbacks { get; set; }
        public DbSet<FBSearchModel> Search { get; set; }
        public DbSet<FBReviewDocumentModel> ReviewDocument { get; set; }
        public DbSet<FBRatingDocumentModel> RatingDocument { get; set; }
        public DbSet<FBImplicitDocumentResultModel> ImplicitDocumentResultFeedbacks { get; set; }
        public DbSet<FBImplicitDocumentQueryModel> ImplicitDocumentQueryFeedbacks { get; set; }
        public DbSet<FBCategoryRatingModel> CategoryRating { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedbackModel>(feedback =>
            {
                feedback.HasKey(c => new { c.userID, c.documentName, c.query });
                feedback.ToTable("feedbacks");
            });
            modelBuilder.Entity<FBSearchModel>(feedback =>
            {
                feedback.ToTable("Search");
            });
            modelBuilder.Entity<FBReviewDocumentModel>(feedback =>
            {
                feedback.HasKey(c => new { c.searchId, c.documentName });
                feedback.ToTable("FeedbackReviews");
            });
            modelBuilder.Entity<FBRatingDocumentModel>(feedback =>
            {
                feedback.HasKey(c => new { c.searchId, c.documentName });
                feedback.ToTable("FeedbackRatings");
            });
            modelBuilder.Entity<FBImplicitDocumentResultModel>(feedback =>
            {
                feedback.HasKey(c => new { c.searchId, c.documentName, c.tagSelected });
                feedback.ToTable("FeedbackImplicitDocumentResults");
            });
            modelBuilder.Entity<FBImplicitDocumentQueryModel>(feedback =>
            {
                feedback.HasKey(c => new { c.searchId, c.documentName, c.query });
                feedback.ToTable("FeedbackImplicitDocumentQueries");
            });
            modelBuilder.Entity<FBCategoryRatingModel>(feedback =>
            {
                feedback.HasKey(c => new { c.searchId, c.category, c.name });
                feedback.ToTable("FeedbackCategoryRatings");
            });
        }
    }
}
