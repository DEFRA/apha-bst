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
    public class NewsMap :IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> entity)
        {
            entity.HasKey(e => e.DatePublished);

            entity.ToTable("tblNews");

            entity.Property(e => e.DatePublished)
                .HasColumnType("smalldatetime")
                .HasColumnName("Date Published");
            entity.Property(e => e.Author).HasMaxLength(50);
            entity.Property(e => e.NewsContent).HasColumnName("News Content");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.AuthorNavigation).WithMany(p => p.News)
                .HasForeignKey(d => d.Author)
                .HasConstraintName("FK_tblNews_tblUser");
        }
    }
}
