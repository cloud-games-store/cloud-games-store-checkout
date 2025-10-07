namespace CloudGamesStore.Application.DTOs.CartDtos
{
    public class NewCartItemDto
    {
        public int CartId { get; set; }
        public int GameId { get; set; }
        //public NewGameDto Game { get; set; } = null!;
        public string GameName { get; set; }
        public string GameGenre { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
    }
}
