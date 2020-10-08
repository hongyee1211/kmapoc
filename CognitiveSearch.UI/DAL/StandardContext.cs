using CognitiveSearch.UI.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.DAL
{
    public class StandardContext : DbContext
    {
        public StandardContext(DbContextOptions<StandardContext> options) : base(options)
        {
        }
        public DbSet<DocumentStandardModel> Standards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentStandardModel>(standard =>
            {
                standard.HasNoKey();
                standard.ToTable("StandardsDocument");
            });
        }
    }
}
