namespace CloudGamesStore.Application.DTOs
{
    public class OrderItemDto
    {
        public string GameName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
