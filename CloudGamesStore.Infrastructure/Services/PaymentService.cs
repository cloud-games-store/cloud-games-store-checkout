using CloudGamesStore.Application.DTOs;
using CloudGamesStore.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace CloudGamesStore.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IOrderProcessingService _orderProcessingService;

        public PaymentService(ILogger<PaymentService> logger, HttpClient httpClient, IConfiguration configuration, IOrderProcessingService orderProcessingService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
            _orderProcessingService = orderProcessingService;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentDetails paymentDetails, decimal amount)
        {
            try
            {
                _logger.LogInformation("Processing payment of {Amount} using {Method}", amount, paymentDetails.Method);

                var request = new
                {
                    TransactionId = Guid.NewGuid(),
                    Payment = paymentDetails,
                    TotalAmount = amount,
                };

                // Requisição POST Function App
                var paymentResult = await Payment(request);

                if (paymentResult == null)
                    throw new Exception("Error while Processing Payment.");

                _logger.LogInformation(paymentResult.Success ? $"Payment processed successfully. TransactionId: {paymentResult.TransactionId}" : paymentResult.ErrorMessage);

                var evento = new CompraRealizadaEvent(paymentResult.TransactionId ?? Guid.NewGuid(), paymentDetails.UserId, paymentDetails.GameId, paymentResult.Success, DateTime.Now);

                await _orderProcessingService.ProcessPendingOrdersAsync(evento, "fila-liberar-jogo");

                return paymentResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment");
                return new PaymentResult
                {
                    Success = false,
                    ErrorMessage = "Payment processing failed"
                };
            }
        }

        private async Task<PaymentResult> Payment(object request)
        {
            await Task.Delay(Random.Shared.Next(200, 1000));
            var aprovado = Random.Shared.Next(100) < 50;

            return new PaymentResult
            {
                Success = aprovado,
                TransactionId = aprovado ? request.GetType().GetProperty("TransactionId")?.GetValue(request) as Guid? : null,
                ErrorMessage = aprovado ? null : "Payment declined"
            };
        }
    }
}
