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
    public class AuditLogMap : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> entity)
        {
            entity
               .HasNoKey()
               .ToTable("Auditlog");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Parameters).HasColumnType("text");
            entity.Property(e => e.Procedure)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TransactionType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.User)
                .HasMaxLength(50)
                .IsUnicode(false);
        }

    }
}
