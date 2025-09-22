using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Domain.Interfaces
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code);
        Task<List<Coupon>> GetActiveCouponsAsync();
        Task<List<Coupon>> GetCouponsByTypeAsync(CouponType type);
        Task<List<Coupon>> GetExpiredCouponsAsync();
        Task<List<Coupon>> GetCouponsExpiringInDaysAsync(int days);
        Task IncrementUsageCountAsync(string code);
        Task<bool> IsCouponValidAsync(string code, decimal orderAmount = 0);
        Task<bool> IsCouponCodeUniqueAsync(string code, int? excludeId = null);
        Task DeactivateCouponAsync(string code);
        Task<List<Coupon>> GetCouponsApplicableToGameAsync(int gameId);
        Task<List<Coupon>> GetCouponsApplicableToGenreAsync(string genre);
    }
}
