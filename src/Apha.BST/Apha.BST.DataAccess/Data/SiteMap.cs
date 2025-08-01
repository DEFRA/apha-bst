﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Apha.BST.DataAccess.Data
{
    public class SiteMap : IEntityTypeConfiguration<Site>
    {
        public void Configure(EntityTypeBuilder<Site> entity)
        {
            entity
               //.HasNoKey()
               .ToTable("tSites");
            entity.HasKey(e => e.PlantNo);
            entity.Property(e => e.AddressCounty).HasMaxLength(50);
            entity.Property(e => e.AddressLine1).HasMaxLength(50);
            entity.Property(e => e.AddressLine2).HasMaxLength(50);
            entity.Property(e => e.AddressPostCode).HasMaxLength(50);
            entity.Property(e => e.AddressTown).HasMaxLength(50);
            entity.Property(e => e.Ahvla)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("AHVLA");
            entity.Property(e => e.Fax).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PlantNo).HasMaxLength(11);
            entity.Property(e => e.Telephone).HasMaxLength(50);
        }
    }
}
