using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repositories.Config
{
    public class WorkConfig : IEntityTypeConfiguration<Work>
    {
        public void Configure(EntityTypeBuilder<Work> builder)
        {
            builder.HasKey(w => w.WorkId);
            builder.Property(w => w.WorkName)
                .IsRequired();
            builder.Property(w => w.ImageUrls)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>()
                )
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

            builder.HasOne(w => w.TaskMaster)
                .WithMany(a => a.SupervisedWorks)
                .HasForeignKey(w => w.TaskMasterId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(w => w.Reports)
                .WithOne(r => r.Work)
                .HasForeignKey(r => r.WorkId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(w => w.Interns)
                .WithMany(a => a.Works);
        }
    }
}
