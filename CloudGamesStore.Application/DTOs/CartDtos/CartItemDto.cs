namespace CloudGamesStore.Application.DTOs.CartDtos
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int GameId { get; set; }
        //public GameDto Game { get; set; } = null!;
        public string GameName { get; set; }
        public string GameGenre { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
    }
}
