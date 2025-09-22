namespace CloudGamesStore.Application.DTOs
{
    public class AppliedDiscount
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
    }

}
