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

        public PaymentService(ILogger<PaymentService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentDetails paymentDetails, decimal amount)
        {
            try
            {
                _logger.LogInformation("Processing payment of {Amount} using {Method}", amount, paymentDetails.Method);

                string url = _configuration["PaymentFunction:Url"];

                var request = new
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Payment = paymentDetails,
                    TotalAmount = amount,
                };

                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Requisição POST Function App
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                // Resposta Function App
                string result = await response.Content.ReadAsStringAsync();

                PaymentResult? paymentResult = JsonSerializer.Deserialize<PaymentResult>(result) ?? null;

                if (paymentResult == null)
                    throw new Exception("Error while Processing Payment.");

                _logger.LogInformation(paymentResult.Success ? $"Payment processed successfully. TransactionId: {paymentResult.TransactionId}" : paymentResult.ErrorMessage);

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
    }
}
