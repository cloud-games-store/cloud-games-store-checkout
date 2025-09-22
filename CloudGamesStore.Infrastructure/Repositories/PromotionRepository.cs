using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Enums;
using CloudGamesStore.Domain.Interfaces;
using CloudGamesStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudGamesStore.Infrastructure.Repositories
{
    public class PromotionRepository : BaseRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(GameStoreCheckoutDbContext context, ILogger<PromotionRepository> logger)
            : base(context, logger)
        {
        }

        public override async Task<Promotion?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Include(p => p.Rules)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving promotion with ID {PromotionId}", id);
                throw;
            }
        }

        public async Task<List<Promotion>> GetActivePromotionsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                return await _dbSet
                    .Include(p => p.Rules)
                    .Where(p => p.IsActive &&
                               p.ValidFrom <= now &&
                               p.ValidUntil >= now)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active promotions");
                throw;
            }
        }

        public async Task<List<Promotion>> GetPromotionsByTypeAsync(PromotionType type)
        {
            try
            {
                return await _dbSet
                    .Include(p => p.Rules)
                    .Where(p => p.Type == type && p.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving promotions by type {Type}", type);
                throw;
            }
        }

        public async Task<List<Promotion>> GetPromotionsApplicableToCartAsync(List<CartItem> cartItems)
        {
            try
            {
                var activePromotions = await GetActivePromotionsAsync();
                var applicablePromotions = new List<Promotion>();

                foreach (var promotion in activePromotions)
                {
                    if (await IsPromotionApplicableToCartAsync(promotion, cartItems))
                    {
                        applicablePromotions.Add(promotion);
                    }
                }

                return applicablePromotions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving promotions applicable to cart");
                throw;
            }
        }

        public async Task<List<Promotion>> GetExpiredPromotionsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                return await _dbSet
                    .Include(p => p.Rules)
                    .Where(p => p.ValidUntil < now)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expired promotions");
                throw;
            }
        }

        public async Task<List<Promotion>> GetPromotionsExpiringInDaysAsync(int days)
        {
            try
            {
                var now = DateTime.UtcNow;
                var expiryDate = now.AddDays(days);
                return await _dbSet
                    .Include(p => p.Rules)
                    .Where(p => p.IsActive &&
                               p.ValidUntil >= now &&
                               p.ValidUntil <= expiryDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving promotions expiring in {Days} days", days);
                throw;
            }
        }

        public async Task DeactivatePromotionAsync(int promotionId)
        {
            try
            {
                var promotion = await GetByIdAsync(promotionId);
                if (promotion != null)
                {
                    promotion.IsActive = false;
                    await UpdateAsync(promotion);
                    _logger.LogInformation("Deactivated promotion {PromotionId}", promotionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating promotion {PromotionId}", promotionId);
                throw;
            }
        }

        public async Task ActivatePromotionAsync(int promotionId)
        {
            try
            {
                var promotion = await GetByIdAsync(promotionId);
                if (promotion != null)
                {
                    promotion.IsActive = true;
                    await UpdateAsync(promotion);
                    _logger.LogInformation("Activated promotion {PromotionId}", promotionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating promotion {PromotionId}", promotionId);
                throw;
            }
        }

        private async Task<bool> IsPromotionApplicableToCartAsync(Promotion promotion, List<CartItem> cartItems)
        {
            try
            {
                switch (promotion.Type)
                {
                    case PromotionType.BuyXGetY:
                        return await IsBuyXGetYApplicableAsync(promotion, cartItems);

                    case PromotionType.VolumeDiscount:
                        return await IsVolumeDiscountApplicableAsync(promotion, cartItems);

                    case PromotionType.GenreDiscount:
                        return await IsGenreDiscountApplicableAsync(promotion, cartItems);

                    case PromotionType.BundleDiscount:
                        return await IsBundleDiscountApplicableAsync(promotion, cartItems);

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if promotion {PromotionId} is applicable to cart", promotion.Id);
                return false;
            }
        }

        private async Task<bool> IsBuyXGetYApplicableAsync(Promotion promotion, List<CartItem> cartItems)
        {
            var totalQuantity = cartItems.Sum(item => item.Quantity);
            var minQuantityRule = promotion.Rules.FirstOrDefault(r => r.Type == PromotionRuleType.MinQuantity);

            if (minQuantityRule != null && int.TryParse(minQuantityRule.Value, out int minQty))
            {
                return totalQuantity >= minQty;
            }

            return totalQuantity >= 2; // Default: need at least 2 items
        }

        private async Task<bool> IsVolumeDiscountApplicableAsync(Promotion promotion, List<CartItem> cartItems)
        {
            var totalQuantity = cartItems.Sum(item => item.Quantity);
            var minQuantityRule = promotion.Rules.FirstOrDefault(r => r.Type == PromotionRuleType.MinQuantity);

            if (minQuantityRule != null && int.TryParse(minQuantityRule.Value, out int minQty))
            {
                return totalQuantity >= minQty;
            }

            return false;
        }

        private async Task<bool> IsGenreDiscountApplicableAsync(Promotion promotion, List<CartItem> cartItems)
        {
            var genreRule = promotion.Rules.FirstOrDefault(r => r.Type == PromotionRuleType.Genre);
            if (genreRule == null) return false;

            return cartItems.Any(item => item.Game.Genre.Equals(genreRule.Value, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<bool> IsBundleDiscountApplicableAsync(Promotion promotion, List<CartItem> cartItems)
        {
            var gameRules = promotion.Rules.Where(r => r.Type == PromotionRuleType.SpecificGame).ToList();
            if (!gameRules.Any()) return false;

            var cartGameIds = cartItems.Select(item => item.GameId).ToHashSet();
            var requiredGameIds = gameRules.Select(rule => int.Parse(rule.Value)).ToHashSet();

            return requiredGameIds.IsSubsetOf(cartGameIds);
        }
    }
}
