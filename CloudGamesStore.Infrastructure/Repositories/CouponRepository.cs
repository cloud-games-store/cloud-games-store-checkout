using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Enums;
using CloudGamesStore.Domain.Interfaces;
using CloudGamesStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudGamesStore.Infrastructure.Repositories
{
    public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(GameStoreCheckoutDbContext context, ILogger<CouponRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            try
            {
                return await _dbSet
                    .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coupon with code {Code}", code);
                throw;
            }
        }

        public async Task<List<Coupon>> GetActiveCouponsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                return await _dbSet
                    .Where(c => c.IsActive &&
                               c.ValidFrom <= now &&
                               c.ValidUntil >= now)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active coupons");
                throw;
            }
        }

        public async Task<List<Coupon>> GetCouponsByTypeAsync(CouponType type)
        {
            try
            {
                return await _dbSet
                    .Where(c => c.Type == type && c.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coupons by type {Type}", type);
                throw;
            }
        }

        public async Task<List<Coupon>> GetExpiredCouponsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                return await _dbSet
                    .Where(c => c.ValidUntil < now)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expired coupons");
                throw;
            }
        }

        public async Task<List<Coupon>> GetCouponsExpiringInDaysAsync(int days)
        {
            try
            {
                var now = DateTime.UtcNow;
                var expiryDate = now.AddDays(days);
                return await _dbSet
                    .Where(c => c.IsActive &&
                               c.ValidUntil >= now &&
                               c.ValidUntil <= expiryDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coupons expiring in {Days} days", days);
                throw;
            }
        }

        public async Task IncrementUsageCountAsync(string code)
        {
            try
            {
                var coupon = await GetByCodeAsync(code);
                if (coupon != null)
                {
                    coupon.UsageCount++;
                    await UpdateAsync(coupon);
                    _logger.LogInformation("Incremented usage count for coupon {Code} to {Count}", code, coupon.UsageCount);
                }
                else
                {
                    _logger.LogWarning("Attempted to increment usage count for non-existent coupon {Code}", code);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing usage count for coupon {Code}", code);
                throw;
            }
        }

        public async Task<bool> IsCouponValidAsync(string code, decimal orderAmount = 0)
        {
            try
            {
                var coupon = await GetByCodeAsync(code);
                if (coupon == null)
                    return false;

                var now = DateTime.UtcNow;

                // Check basic validity
                if (!coupon.IsActive ||
                    now < coupon.ValidFrom ||
                    now > coupon.ValidUntil)
                    return false;

                // Check usage limit
                if (coupon.UsageLimit.HasValue &&
                    coupon.UsageCount >= coupon.UsageLimit.Value)
                    return false;

                // Check minimum order amount
                if (coupon.MinimumOrderAmount.HasValue &&
                    orderAmount < coupon.MinimumOrderAmount.Value)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating coupon {Code}", code);
                throw;
            }
        }

        public async Task<bool> IsCouponCodeUniqueAsync(string code, int? excludeId = null)
        {
            try
            {
                var query = _dbSet.Where(c => c.Code.ToLower() == code.ToLower());
                if (excludeId.HasValue)
                {
                    query = query.Where(c => c.Id != excludeId.Value);
                }
                return !await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking uniqueness of coupon code {Code}", code);
                throw;
            }
        }

        public async Task DeactivateCouponAsync(string code)
        {
            try
            {
                var coupon = await GetByCodeAsync(code);
                if (coupon != null)
                {
                    coupon.IsActive = false;
                    await UpdateAsync(coupon);
                    _logger.LogInformation("Deactivated coupon {Code}", code);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating coupon {Code}", code);
                throw;
            }
        }

        public async Task<List<Coupon>> GetCouponsApplicableToGameAsync(int gameId)
        {
            try
            {
                return await _dbSet
                    .Where(c => c.IsActive &&
                               (!c.ApplicableGameIds.Any() || c.ApplicableGameIds.Contains(gameId)))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coupons applicable to game {GameId}", gameId);
                throw;
            }
        }

        public async Task<List<Coupon>> GetCouponsApplicableToGenreAsync(string genre)
        {
            try
            {
                return await _dbSet
                    .Where(c => c.IsActive &&
                               (!c.ApplicableGenres.Any() || c.ApplicableGenres.Contains(genre)))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coupons applicable to genre {Genre}", genre);
                throw;
            }
        }
    }
}
