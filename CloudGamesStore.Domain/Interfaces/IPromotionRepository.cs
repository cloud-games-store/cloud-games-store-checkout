using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Domain.Interfaces
{
    public interface IPromotionRepository : IRepository<Promotion>
    {
        Task<List<Promotion>> GetActivePromotionsAsync();
        Task<List<Promotion>> GetPromotionsByTypeAsync(PromotionType type);
        Task<List<Promotion>> GetPromotionsApplicableToCartAsync(List<CartItem> cartItems);
        Task<List<Promotion>> GetExpiredPromotionsAsync();
        Task<List<Promotion>> GetPromotionsExpiringInDaysAsync(int days);
        Task DeactivatePromotionAsync(int promotionId);
        Task ActivatePromotionAsync(int promotionId);
    }
}
