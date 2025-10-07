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
        Task<CartDto> GetCartByUserIdAsync(int userId);
        Task DeleteCartByUserIdAsync(int userId);
        Task AddItemCartByUserIdAsync(int userId, int gameId, int quantity = 1);
        Task RemoveItemFromCartAsync(int userId, int gameId);
        Task UpdateItemQuantityAsync(int userId, int gameId, int newQuantity);
    }
}
