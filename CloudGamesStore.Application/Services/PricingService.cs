using CloudGamesStore.Application.DTOs;
using CloudGamesStore.Application.Interfaces;
using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Enums;
using CloudGamesStore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Application.Services
{
    public class PricingService : IPricingService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly ICouponRepository _couponRepository;
        private const decimal TAX_RATE = 0.0m; // 8% tax rate

        public PricingService(
            IPromotionRepository promotionRepository,
            ICouponRepository couponRepository)
        {
            _promotionRepository = promotionRepository;
            _couponRepository = couponRepository;
        }

        public async Task<List<AppliedDiscount>> CalculateAutomaticDiscountsAsync(List<CartItem> items)
        {
            var discounts = new List<AppliedDiscount>();
            var activePromotions = await _promotionRepository.GetActivePromotionsAsync();

            foreach (var promotion in activePromotions)
            {
                var discount = await CalculatePromotionDiscount(promotion, items);
                if (discount != null)
                    discounts.Add(discount);
            }

            return discounts;
        }

        public async Task<List<AppliedDiscount>> CalculateCouponDiscountsAsync(
            List<CartItem> items, List<string> couponCodes, decimal subTotal)
        {
            var discounts = new List<AppliedDiscount>();

            foreach (var code in couponCodes)
            {
                var coupon = await _couponRepository.GetByCodeAsync(code);
                if (coupon != null && IsCouponValid(coupon, subTotal))
                {
                    var discount = CalculateCouponDiscount(coupon, items, subTotal);
                    if (discount != null)
                        discounts.Add(discount);
                }
            }

            return discounts;
        }

        public async Task<decimal> CalculateTaxAsync(decimal amount, int userId)
        {
            // Consider user's location and applicable tax rates
            return Math.Round(amount * TAX_RATE, 2);
        }

        private async Task<AppliedDiscount?> CalculatePromotionDiscount(
            Promotion promotion, List<CartItem> items)
        {
            switch (promotion.Type)
            {
                case PromotionType.BuyXGetY:
                    return CalculateBuyXGetYDiscount(promotion, items);

                case PromotionType.VolumeDiscount:
                    return CalculateVolumeDiscount(promotion, items);

                case PromotionType.GenreDiscount:
                    return CalculateGenreDiscount(promotion, items);

                default:
                    return null;
            }
        }

        private AppliedDiscount? CalculateBuyXGetYDiscount(Promotion promotion, List<CartItem> items)
        {
            // Example: Buy 2 get 1 free
            var totalQuantity = items.Sum(i => i.Quantity);
            var freeItems = totalQuantity / 3; // Every 3rd item is free

            if (freeItems > 0)
            {
                var cheapestItemPrice = items.Min(i => i.UnitPrice);
                return new AppliedDiscount
                {
                    Code = "AUTO-BUY2GET1",
                    Name = promotion.Name,
                    Amount = cheapestItemPrice * freeItems,
                    Type = "Promotion"
                };
            }

            return null;
        }

        private AppliedDiscount? CalculateVolumeDiscount(Promotion promotion, List<CartItem> items)
        {
            var totalQuantity = items.Sum(i => i.Quantity);
            var minQuantityRule = promotion.Rules.FirstOrDefault(r => r.Type == PromotionRuleType.MinQuantity);

            if (minQuantityRule != null && int.TryParse(minQuantityRule.Value, out int minQty))
            {
                if (totalQuantity >= minQty)
                {
                    var totalAmount = items.Sum(i => i.UnitPrice * i.Quantity);
                    var discountAmount = totalAmount * (promotion.Value / 100);

                    return new AppliedDiscount
                    {
                        Code = "AUTO-VOLUME",
                        Name = promotion.Name,
                        Amount = discountAmount,
                        Type = "Promotion"
                    };
                }
            }

            return null;
        }

        private AppliedDiscount? CalculateGenreDiscount(Promotion promotion, List<CartItem> items)
        {
            var genreRule = promotion.Rules.FirstOrDefault(r => r.Type == PromotionRuleType.Genre);
            if (genreRule == null) return null;

            var applicableItems = items.Where(i => i.Game.Genre == genreRule.Value).ToList();
            if (applicableItems.Any())
            {
                var applicableAmount = applicableItems.Sum(i => i.UnitPrice * i.Quantity);
                var discountAmount = applicableAmount * (promotion.Value / 100);

                return new AppliedDiscount
                {
                    Code = "AUTO-GENRE",
                    Name = promotion.Name,
                    Amount = discountAmount,
                    Type = "Promotion"
                };
            }

            return null;
        }

        private AppliedDiscount? CalculateCouponDiscount(
            Coupon coupon, List<CartItem> items, decimal subTotal)
        {
            decimal applicableAmount = subTotal;

            // Filter items if coupon has restrictions
            if (coupon.ApplicableGameIds.Any() || coupon.ApplicableGenres.Any())
            {
                var applicableItems = items.Where(item =>
                    coupon.ApplicableGameIds.Contains(item.GameId) ||
                    coupon.ApplicableGenres.Contains(item.Game.Genre)).ToList();

                applicableAmount = applicableItems.Sum(i => i.UnitPrice * i.Quantity);
            }

            decimal discountAmount = coupon.Type == CouponType.Percentage
                ? applicableAmount * (coupon.Value / 100)
                : coupon.Value;

            // Apply maximum discount limit
            if (coupon.MaximumDiscountAmount.HasValue)
            {
                discountAmount = Math.Min(discountAmount, coupon.MaximumDiscountAmount.Value);
            }

            return new AppliedDiscount
            {
                Code = coupon.Code,
                Name = coupon.Name,
                Amount = Math.Round(discountAmount, 2),
                Type = "Coupon"
            };
        }

        private bool IsCouponValid(Coupon coupon, decimal subTotal)
        {
            return coupon.IsActive &&
                   DateTime.UtcNow >= coupon.ValidFrom &&
                   DateTime.UtcNow <= coupon.ValidUntil &&
                   (!coupon.MinimumOrderAmount.HasValue || subTotal >= coupon.MinimumOrderAmount.Value) &&
                   (!coupon.UsageLimit.HasValue || coupon.UsageCount < coupon.UsageLimit.Value);
        }
    }
}
