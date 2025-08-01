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
    public class TraineesMap : IEntityTypeConfiguration<Trainees>
    {
        public void Configure(EntityTypeBuilder<Trainees> entity)
        {
            entity.HasKey(e => e.TraineeNo);

            entity.ToTable("tblTrainee");

            entity.Property(e => e.TraineeNo).HasColumnName("Trainee_No");
            entity.Property(e => e.PlantNo)
                .HasMaxLength(11)
                .HasColumnName("Plant_No");
            entity.Property(e => e.Trainee).HasMaxLength(100);

            entity.HasOne(d => d.PlantNoNavigation).WithMany(p => p.TblTrainees)
                .HasForeignKey(d => d.PlantNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblTrainee_tblSites");
        }
    }
}
