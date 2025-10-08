using CloudGamesStore.Application.DTOs;
using CloudGamesStore.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace CloudGamesStore.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly HttpClient _httpClient;

        public PaymentService(ILogger<PaymentService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentDetails paymentDetails, decimal amount)
        {
            try
            {
                _logger.LogInformation("Processing payment of {Amount} using {Method}", amount, paymentDetails.Method);

                using (HttpClient client = new HttpClient())
                {
                    string url = "https://fiapgamestoreapp.azurewebsites.net";

                    var request = new
                    {
                        Payment = paymentDetails,
                        TotalAmount = amount,
                    };

                    string json = JsonSerializer.Serialize(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Requisição POST Function App
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    // Resposta Function App
                    string result = await response.Content.ReadAsStringAsync();

                    PaymentResult? paymentResult = JsonSerializer.Deserialize<PaymentResult>(result) ?? null;
                    
                    if (paymentResult == null)
                        throw new Exception("Error while Processing Payment.");
                    
                    _logger.LogInformation(paymentResult.Success ? $"Payment processed successfully. TransactionId: {paymentResult.TransactionId}" : paymentResult.ErrorMessage);

                    return paymentResult;
                }
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

        //public async Task<PaymentResult> ProcessPaymentAsync(PaymentDetails paymentDetails, decimal amount)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Processing payment of {Amount} using {Method}", amount, paymentDetails.Method);

        //        // In a real implementation, this would integrate with actual payment processors
        //        // like Stripe, PayPal, Square, etc.

        //        // Simulate payment processing
        //        await Task.Delay(1000); // Simulate network call

        //        // Mock validation
        //        if (string.IsNullOrEmpty(paymentDetails.Token))
        //        {
        //            return new PaymentResult
        //            {
        //                Success = false,
        //                ErrorMessage = "Payment token is required"
        //            };
        //        }

        //        if (amount <= 0)
        //        {
        //            return new PaymentResult
        //            {
        //                Success = false,
        //                ErrorMessage = "Invalid payment amount"
        //            };
        //        }

        //        // Simulate successful payment
        //        var transactionId = Guid.NewGuid().ToString();

        //        _logger.LogInformation("Payment processed successfully. TransactionId: {TransactionId}", transactionId);

        //        return new PaymentResult
        //        {
        //            Success = true,
        //            TransactionId = transactionId
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error processing payment");
        //        return new PaymentResult
        //        {
        //            Success = false,
        //            ErrorMessage = "Payment processing failed"
        //        };
        //    }
        //}
    }
}
