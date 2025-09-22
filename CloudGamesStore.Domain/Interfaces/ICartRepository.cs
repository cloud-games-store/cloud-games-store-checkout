using CloudGamesStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Domain.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(int userId);
        Task<Cart> GetOrCreateCartForUserAsync(int userId);
        Task AddItemToCartAsync(int userId, int gameId, int quantity = 1);
        Task RemoveItemFromCartAsync(int userId, int gameId);
        Task UpdateItemQuantityAsync(int userId, int gameId, int newQuantity);
        Task ClearCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId);
        Task<bool> IsGameInCartAsync(int userId, int gameId);
    }
}
