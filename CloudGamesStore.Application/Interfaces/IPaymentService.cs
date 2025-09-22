using CloudGamesStore.Application.DTOs;

namespace CloudGamesStore.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentDetails paymentDetails, decimal amount);
    }
}
