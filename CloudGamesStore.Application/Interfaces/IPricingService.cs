using CloudGamesStore.Application.DTOs;
using CloudGamesStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Application.Interfaces
{
    public interface IPricingService
    {
        Task<List<AppliedDiscount>> CalculateAutomaticDiscountsAsync(List<CartItem> items);
        Task<List<AppliedDiscount>> CalculateCouponDiscountsAsync(
            List<CartItem> items, List<string> couponCodes, decimal subTotal);
        Task<decimal> CalculateTaxAsync(decimal amount, int userId);
    }
}
