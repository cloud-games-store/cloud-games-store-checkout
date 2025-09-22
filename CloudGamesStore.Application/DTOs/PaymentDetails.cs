namespace CloudGamesStore.Application.DTOs
{
    public class PaymentDetails
    {
        public string Method { get; set; } = string.Empty; // "CreditCard", "PayPal", etc.
        public string Token { get; set; } = string.Empty; // Payment token from frontend
    }

}
