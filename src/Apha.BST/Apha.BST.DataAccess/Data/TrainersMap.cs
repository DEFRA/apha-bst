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
    public class TrainersMap : IEntityTypeConfiguration<Trainers>
    {
        public void Configure(EntityTypeBuilder<Trainers> entity)
        {
            entity.HasKey(e => e.TrainerId);

            entity.ToTable("tblTrainers");

            entity.Property(e => e.TrainerId).HasColumnName("Trainer_ID");
            entity.Property(e => e.LocId)
                .HasMaxLength(50)
                .HasColumnName("Loc_ID");
            entity.Property(e => e.PersonId).HasColumnName("Person_ID");
            entity.Property(e => e.Trainer).HasMaxLength(100);

            entity.HasOne(d => d.Loc).WithMany(p => p.TblTrainers)
                .HasForeignKey(d => d.LocId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblTrainers_tblVLALoc");
        }
    }
}
