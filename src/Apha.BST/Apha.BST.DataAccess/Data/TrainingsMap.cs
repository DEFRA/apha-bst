using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Apha.BST.DataAccess.Data
{
    public class TrainingsMap : IEntityTypeConfiguration<Trainings>
    {
        public void Configure(EntityTypeBuilder<Trainings> entity)
        {
            entity.HasKey(e => new { e.TraineeNo, e.DateTrained, e.SpeciesTrained }).HasName("PK_tblTraining_1");

            entity.ToTable("tblTraining");

            entity.Property(e => e.TraineeNo).HasColumnName("Trainee_No");
            entity.Property(e => e.DateTrained)
                .HasMaxLength(255)
                .HasColumnName("Date_Trained");
            entity.Property(e => e.SpeciesTrained)
                .HasMaxLength(50)
                .HasColumnName("Species_Trained");
            entity.Property(e => e.TrainerId).HasColumnName("Trainer_ID");

            entity.HasOne(d => d.TraineeNoNavigation).WithMany(p => p.TblTrainings)
                .HasForeignKey(d => d.TraineeNo)
                .HasConstraintName("FK_tblTraining_tblTrainee");

            entity.HasOne(d => d.Trainer).WithMany(p => p.TblTrainings)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK_tblTraining_tblTrainers");

        }
    }
}
