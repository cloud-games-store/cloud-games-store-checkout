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
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Value).HasPrecision(18, 2);
            builder.Property(e => e.MinimumOrderAmount).HasPrecision(18, 2);
            builder.Property(e => e.MaximumDiscountAmount).HasPrecision(18, 2);
            builder.HasIndex(e => e.Code).IsUnique();

            // Convert lists to JSON for storage
            builder.Property(e => e.ApplicableGameIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>());

            builder.Property(e => e.ApplicableGenres)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
        }
    }
}
