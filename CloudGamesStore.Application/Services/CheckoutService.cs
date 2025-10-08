using CloudGamesStore.Application.DTOs;
using CloudGamesStore.Application.Interfaces;
using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Enums;
using CloudGamesStore.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CloudGamesStore.Application.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPricingService _pricingService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<CheckoutService> _logger;

        public CheckoutService(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            ICouponRepository couponRepository,
            IPromotionRepository promotionRepository,
            IPricingService pricingService,
            IPaymentService paymentService,
            ILogger<CheckoutService> logger)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _couponRepository = couponRepository;
            _promotionRepository = promotionRepository;
            _pricingService = pricingService;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<CheckoutResponse> ProcessCheckoutAsync(CheckoutRequest request)
        {
            try
            {
                _logger.LogInformation("Starting checkout process for user {UserId}", request.UserId);

                // Get user's cart
                var cart = await _cartRepository.GetByUserIdAsync(request.UserId);
                if (cart == null || !cart.Items.Any())
                {
                    return new CheckoutResponse
                    {
                        Success = false,
                        Errors = { "Cart is empty" }
                    };
                }

                // Calculate order summary with discounts
                var orderSummary = await CalculateOrderSummaryAsync(request.UserId, request.CouponCodes);

                // Validate coupons
                var validationResult = await ValidateCouponsAsync(request.CouponCodes, orderSummary.SubTotal);
                if (!validationResult.IsValid)
                {
                    return new CheckoutResponse
                    {
                        Success = false,
                        Errors = validationResult.Errors
                    };
                }

                // Process payment
                var paymentResult = await _paymentService.ProcessPaymentAsync(
                    request.Payment, orderSummary.TotalAmount);

                if (!paymentResult.Success)
                {
                    return new CheckoutResponse
                    {
                        Success = false,
                        Errors = { "Payment processing failed: " + paymentResult.ErrorMessage }
                    };
                }

                // Create order
                var order = await CreateOrderAsync(cart, orderSummary, request.CouponCodes);

                // Update coupon usage
                await UpdateCouponUsageAsync(request.CouponCodes);

                // Clear cart
                await _cartRepository.ClearCartAsync(request.UserId);

                _logger.LogInformation("Checkout completed successfully for user {UserId}, Order {OrderNumber}",
                    request.UserId, order.OrderNumber);

                return new CheckoutResponse
                {
                    Success = true,
                    OrderNumber = order.OrderNumber,
                    TotalAmount = order.TotalAmount,
                    OrderSummary = orderSummary
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing checkout for user {UserId}", request.UserId);
                return new CheckoutResponse
                {
                    Success = false,
                    Errors = { "An error occurred while processing your order" }
                };
            }
        }

        public async Task<OrderSummary> CalculateOrderSummaryAsync(Guid userId, List<string> couponCodes)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException("Cart is empty");

            var subTotal = cart.Items.Sum(item => item.UnitPrice * item.Quantity);

            // Apply automatic promotions
            var automaticDiscounts = await _pricingService.CalculateAutomaticDiscountsAsync(cart.Items);

            // Apply coupon discounts
            var couponDiscounts = await _pricingService.CalculateCouponDiscountsAsync(
                cart.Items, couponCodes, subTotal);

            var totalDiscountAmount = automaticDiscounts.Sum(d => d.Amount) +
                                    couponDiscounts.Sum(d => d.Amount);

            var discountedAmount = subTotal - totalDiscountAmount;
            var taxAmount = await _pricingService.CalculateTaxAsync(discountedAmount, userId);
            var totalAmount = discountedAmount + taxAmount;

            return new OrderSummary
            {
                SubTotal = subTotal,
                DiscountAmount = totalDiscountAmount,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                AppliedDiscounts = automaticDiscounts.Concat(couponDiscounts).ToList(),
                Items = cart.Items.Select(item => new OrderItemDto
                {
                    GameName = item.GameName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.UnitPrice * item.Quantity
                }).ToList()
            };
        }

        private async Task<(bool IsValid, List<string> Errors)> ValidateCouponsAsync(
            List<string> couponCodes, decimal subTotal)
        {
            var errors = new List<string>();

            foreach (var code in couponCodes)
            {
                var coupon = await _couponRepository.GetByCodeAsync(code);
                if (coupon == null)
                {
                    errors.Add($"Coupon '{code}' not found");
                    continue;
                }

                if (!coupon.IsActive)
                {
                    errors.Add($"Coupon '{code}' is no longer active");
                    continue;
                }

                if (DateTime.UtcNow < coupon.ValidFrom || DateTime.UtcNow > coupon.ValidUntil)
                {
                    errors.Add($"Coupon '{code}' is not valid at this time");
                    continue;
                }

                if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
                {
                    errors.Add($"Coupon '{code}' has reached its usage limit");
                    continue;
                }

                if (coupon.MinimumOrderAmount.HasValue && subTotal < coupon.MinimumOrderAmount.Value)
                {
                    errors.Add($"Minimum order amount of ${coupon.MinimumOrderAmount:F2} required for coupon '{code}'");
                    continue;
                }
            }

            return (!errors.Any(), errors);
        }

        private async Task<Order> CreateOrderAsync(Cart cart, OrderSummary summary, List<string> couponCodes)
        {
            var order = new Order
            {
                UserId = cart.UserId,
                OrderNumber = GenerateOrderNumber(),
                SubTotal = summary.SubTotal,
                DiscountAmount = summary.DiscountAmount,
                TaxAmount = summary.TaxAmount,
                TotalAmount = summary.TotalAmount,
                Status = OrderStatus.Confirmed,
                CreatedAt = DateTime.UtcNow,
                AppliedPromoCodes = couponCodes,
                Items = cart.Items.Select(item => new OrderItem
                {
                    GameId = item.GameId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.UnitPrice * item.Quantity
                }).ToList()
            };

            return await _orderRepository.CreateAsync(order);
        }

        private async Task UpdateCouponUsageAsync(List<string> couponCodes)
        {
            foreach (var code in couponCodes)
            {
                await _couponRepository.IncrementUsageCountAsync(code);
            }
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }
    }
}
