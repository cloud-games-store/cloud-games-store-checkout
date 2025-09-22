using CloudGamesStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Domain.Entities
{
    public class PromotionRule
    {
        public int Id { get; set; }
        public int PromotionId { get; set; }
        public PromotionRuleType Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty; // ">=", "==", "in", etc.
    }
}
