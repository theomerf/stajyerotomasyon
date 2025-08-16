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
    public class MessageConfig : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.MessageId);
            builder.Property(m => m.MessageTitle).IsRequired()
                .HasMaxLength(50);
            builder.Property(m => m.MessageBody).IsRequired();

            builder.HasMany(m => m.Interns)
                .WithMany(i => i.Messages)
                .UsingEntity(j => j.ToTable("MessageInterns"));
        }
    }
}
