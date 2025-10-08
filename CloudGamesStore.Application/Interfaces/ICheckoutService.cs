using CloudGamesStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Application.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutResponse> ProcessCheckoutAsync(CheckoutRequest request);
        Task<OrderSummary> CalculateOrderSummaryAsync(Guid userId, List<string> couponCodes);
    }
}
