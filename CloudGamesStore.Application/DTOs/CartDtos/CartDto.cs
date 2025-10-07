using CloudGamesStore.Domain.Entities;

namespace CloudGamesStore.Application.DTOs.CartDtos
{
    public class CartDto
    {
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static CartDto ToCartDto(Cart cart)
        {
            return new CartDto
            {
                UserId = cart.UserId,
                Items = cart.Items != null ?
                    cart.Items.Select(x =>
                        new CartItemDto
                        {
                            Id = x.Id,
                            CartId = x.CartId,
                            GameId = x.GameId,
                            GameName = x.GameName,
                            GameGenre = x.GameGenre,
                            Quantity = x.Quantity,
                            UnitPrice = x.UnitPrice
                        }).ToList() :
                        new List<CartItemDto>(),
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt
            };
        }

        public static Cart ToCart(NewCartDto cart)
        {
            return new Cart
            {
                UserId = cart.UserId,
                Items = cart.Items != null ?
                    cart.Items.Select(x =>
                        new CartItem
                        {
                            CartId = x.CartId,
                            GameId = x.GameId,
                            GameName = x.GameName,
                            GameGenre = x.GameGenre,
                            Quantity = x.Quantity,
                            UnitPrice = x.UnitPrice
                        }).ToList() :
                        new List<CartItem>(),
                CreatedAt = DateTime.Now
            };
        }
    }
}
