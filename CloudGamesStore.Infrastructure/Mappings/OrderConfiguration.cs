using CloudGamesStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudGamesStore.Infrastructure.Mappings
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            builder.Property(e => e.SubTotal).HasPrecision(18, 2);
            builder.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            builder.Property(e => e.TaxAmount).HasPrecision(18, 2);
            builder.Property(e => e.TotalAmount).HasPrecision(18, 2);
            builder.HasIndex(e => e.OrderNumber).IsUnique();
            builder.HasIndex(e => e.UserId);

            // Convert list to JSON for storage
            builder.Property(e => e.AppliedPromoCodes)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

            builder.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
