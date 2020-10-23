using CognitiveSearch.UI.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.DAL
{
    public class DisciplineContext : DbContext
    {
        public DisciplineContext(DbContextOptions<DisciplineContext> options) : base(options)
        {
        }
        public DbSet<ExpertsDisciplineModel> DocumentDisciplines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExpertsDisciplineModel>(discipline =>
            {
                discipline.HasNoKey();
                discipline.ToTable("myExpertsDiscipline");
            });
        }
    }
}
