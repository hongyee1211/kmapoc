using Microsoft.EntityFrameworkCore;
using CognitiveSearch.UI.Models;

namespace CognitiveSearch.UI.DAL
{
    public class FeedbackContext : DbContext
    {
        public FeedbackContext(DbContextOptions<FeedbackContext> options) : base(options)
        {
        }

        public DbSet<FeedbackModel> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedbackModel>(feedback =>
            {
                feedback.HasKey(c => new { c.userID, c.documentName, c.query });
                feedback.ToTable("feedbacks");
            });
        }
    }
}
