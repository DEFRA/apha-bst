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
    public class TrainingMap : IEntityTypeConfiguration<Training>
    {
        public void Configure(EntityTypeBuilder<Training> entity) 
        {
            entity.HasKey(e => new {
                e.PersonId,
                e.TrainingAnimal,
                e.TrainingDateTime
            }).HasName("PK_tTraining_1");

            entity.ToTable("tTraining");

            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.TrainingAnimal).HasMaxLength(50);
            entity.Property(e => e.TrainingDateTime).HasColumnType("datetime");
            entity.Property(e => e.TrainerId).HasColumnName("TrainerID");
            entity.Property(e => e.TrainingType).HasMaxLength(50);

        }
       
}
}
