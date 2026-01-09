namespace CloudGamesStore.Application.DTOs
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public Guid? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
