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
        Task<Cart?> GetByUserIdAsync(Guid userId);
        Task<Cart> GetOrCreateCartForUserAsync(Guid userId);
        Task AddItemToCartAsync(Guid userId, int gameId, int quantity = 1);
        Task RemoveItemFromCartAsync(Guid userId, int gameId);
        Task UpdateItemQuantityAsync(Guid userId, int gameId, int newQuantity);
        Task ClearCartAsync(Guid userId);
        Task<int> GetCartItemCountAsync(Guid userId);
        Task<decimal> GetCartTotalAsync(Guid userId);
        Task<bool> IsGameInCartAsync(Guid userId, int gameId);
    }
}
