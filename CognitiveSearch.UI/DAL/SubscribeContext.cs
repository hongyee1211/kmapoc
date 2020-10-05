using CognitiveSearch.UI.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.DAL
{
    public class SubscribeContext : DbContext
    {
        public SubscribeContext(DbContextOptions<SubscribeContext> options) : base(options)
        {
        }
        public DbSet<SubscribeSearchModel> Search { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubscribeSearchModel>(feedback =>
            {
                feedback.HasKey(c => new { c.userId, c.query });
                feedback.ToTable("SubscribeSearch");
            });
        }
    }
}
