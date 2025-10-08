using CloudGamesStore.Application.DTOs.CartDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartByUserIdAsync(Guid userId);
        Task DeleteCartByUserIdAsync(Guid userId);
        Task AddItemCartByUserIdAsync(Guid userId, int gameId, int quantity = 1);
        Task RemoveItemFromCartAsync(Guid userId, int gameId);
        Task UpdateItemQuantityAsync(Guid userId, int gameId, int newQuantity);
        Task ClearCart(Guid userId);
    }
}
