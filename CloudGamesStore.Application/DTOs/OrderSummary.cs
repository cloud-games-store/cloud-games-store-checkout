namespace CloudGamesStore.Application.DTOs
{
    public class OrderSummary
    {
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<AppliedDiscount> AppliedDiscounts { get; set; } = new();
        public List<OrderItemDto> Items { get; set; } = new();
    }

}
