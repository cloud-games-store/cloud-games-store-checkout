namespace CloudGamesStore.Application.DTOs
{
    public class CheckoutResponse
    {
        public bool Success { get; set; }
        public string? OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public List<string> Errors { get; set; } = new();
        public OrderSummary? OrderSummary { get; set; }
    }

}
