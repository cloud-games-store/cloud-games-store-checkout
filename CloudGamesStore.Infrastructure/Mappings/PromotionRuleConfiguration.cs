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
    public class PromotionRuleConfiguration : IEntityTypeConfiguration<PromotionRule>
    {
        public void Configure(EntityTypeBuilder<PromotionRule> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Value).IsRequired().HasMaxLength(500);

            builder.Property(e => e.Operator).HasMaxLength(10);
        }
    }
}
