using CognitiveSearch.UI.Models.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.DAL
{
    public class TrainingContext : DbContext
    {
        public TrainingContext(DbContextOptions<TrainingContext> options) : base(options)
        {
        }
        public DbSet<TrainingAbbreviationModel> Abbreviation { get; set; }
        public DbSet<TrainingPlantCodeModel> PlantCode { get; set; }
        public DbSet<TrainingEquipmentComponentModel> Component { get; set; }
        public DbSet<TrainingEquipmentClassModel> EquipmentClass { get; set; }
        public DbSet<TrainingFailureModeModel> FailureMode { get; set; }
        public DbSet<TrainingManufacturerModel> Manufacturer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrainingAbbreviationModel>(training =>
            {
                training.ToTable("Entity_Abbreviation");
            });

            modelBuilder.Entity<TrainingPlantCodeModel>(training =>
            {
                training.ToTable("Entity_PlantCode");
            });

            modelBuilder.Entity<TrainingEquipmentComponentModel>(training =>
            {
                training.ToTable("Entity_EquipmentComponent");
            });

            modelBuilder.Entity<TrainingEquipmentClassModel>(training =>
            {
                training.ToTable("Entity_EquipmentClass");
            });

            modelBuilder.Entity<TrainingFailureModeModel>(training =>
            {
                training.ToTable("Entity_FailureMode");
            });

            modelBuilder.Entity<TrainingManufacturerModel>(training =>
            {
                training.ToTable("Entity_Manufacturer");
            });
        }
    }
}
