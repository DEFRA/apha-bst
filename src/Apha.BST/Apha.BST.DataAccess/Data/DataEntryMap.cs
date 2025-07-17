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
    public class DataEntryMap: IEntityTypeConfiguration<DataEntry>
    {
        public void Configure(EntityTypeBuilder<DataEntry> entity) 
        {
            entity.HasKey(e => e.ActiveView);

            entity.ToTable("tblDataEntry");

            entity.Property(e => e.ActiveView).ValueGeneratedNever();
            entity.Property(e => e.ActiveViewName).HasMaxLength(50);
            entity.Property(e => e.CanWrite)
                .HasMaxLength(1)
                .IsFixedLength();
        }
    }
}
