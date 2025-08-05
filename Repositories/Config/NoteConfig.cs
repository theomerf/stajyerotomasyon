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
    public class NoteConfig : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.HasKey(n => n.NoteId);
            builder.Property(n => n.Content)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasOne(n => n.Account)
                .WithMany(a => a.Notes)
                .HasForeignKey(n => n.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Application)
                .WithMany(ap => ap.Notes)
                .HasForeignKey(n => n.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
