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
    public class VlaLocMap : IEntityTypeConfiguration<VlaLoc>
    {
        public void Configure(EntityTypeBuilder<VlaLoc> entity)
        {
            entity.HasKey(e => e.LocId);

            entity.ToTable("tblVLALoc");

            entity.Property(e => e.LocId)
                .HasMaxLength(50)
                .HasColumnName("Loc_ID");
            entity.Property(e => e.Ahvla)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("AHVLA");
            entity.Property(e => e.VlaLocation)
                .HasMaxLength(50)
                .HasColumnName("VLA_Location");
        }
    }
}
