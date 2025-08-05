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
    public class ApplicationConfig : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.HasKey(a => a.ApplicationId);
            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(a => a.Section)
                .WithMany(s => s.Applications)
                .HasForeignKey(a => a.SectionId)
                .OnDelete(DeleteBehavior.NoAction);
           
            builder.HasMany(a => a.Notes)
                .WithOne(n => n.Application)
                .HasForeignKey(n => n.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
