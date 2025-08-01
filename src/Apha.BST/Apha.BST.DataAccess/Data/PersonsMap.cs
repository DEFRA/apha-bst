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
    public class PersonsMap:IEntityTypeConfiguration<Persons>
    {
        public void Configure(EntityTypeBuilder<Persons> entity)
        {
            entity.HasKey(e => e.PersonId);

            entity.ToTable("tPerson");

            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.LocationId)
                .HasMaxLength(11)
                .HasColumnName("LocationID");
            entity.Property(e => e.Person).HasMaxLength(100);
            entity.Property(e => e.VlalocationId).HasColumnName("VLALocationID");
        }
    }
}
