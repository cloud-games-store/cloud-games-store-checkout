namespace CloudGamesStore.Application.DTOs
{
    public class CheckoutRequest
    {
        public int UserId { get; set; }
        public List<string> CouponCodes { get; set; } = new();
        public PaymentDetails Payment { get; set; } = new();
        public BillingAddress BillingAddress { get; set; } = new();
    }
}
