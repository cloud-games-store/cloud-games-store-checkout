using CloudGamesStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Infrastructure.Mappings
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.UnitPrice).HasPrecision(18, 2);
            builder.Property(e => e.TotalPrice).HasPrecision(18, 2);
            //builder.HasOne(e => e.Game)
            //    .WithMany()
            //    .HasForeignKey(e => e.GameId);
        }
    }
}
