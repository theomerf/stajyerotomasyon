using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Config
{
    public class SectionConfig : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder) 
        {
            builder.HasKey(s => s.SectionId);
            builder.Property(s => s.SectionName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(s => s.Department)
                .WithMany(d => d.Sections)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.Users)
                .WithOne(u => u.Section)
                .HasForeignKey(u => u.SectionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.Applications)
                .WithOne(a => a.Section)
                .HasForeignKey(a => a.SectionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
