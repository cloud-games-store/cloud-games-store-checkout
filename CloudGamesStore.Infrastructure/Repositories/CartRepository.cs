using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Interfaces;
using CloudGamesStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudGamesStore.Infrastructure.Repositories
{
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        public CartRepository(GameStoreCheckoutDbContext context, ILogger<CartRepository> logger)
            : base(context, logger)
        {
        }

        public override async Task<Cart?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Game)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart with ID {CartId}", id);
                throw;
            }
        }

        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _dbSet
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Game)
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Cart> GetOrCreateCartForUserAsync(int userId)
        {
            try
            {
                var cart = await GetByUserIdAsync(userId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Items = new List<CartItem>()
                    };
                    cart = await CreateAsync(cart);
                    _logger.LogInformation("Created new cart for user {UserId}", userId);
                }
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting or creating cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task AddItemToCartAsync(int userId, int gameId, int quantity = 1)
        {
            try
            {
                var cart = await GetOrCreateCartForUserAsync(userId);
                var existingItem = cart.Items.FirstOrDefault(i => i.GameId == gameId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    _context.CartItems.Update(existingItem);
                }
                else
                {
                    var game = await _context.Games.FindAsync(gameId);
                    if (game == null || !game.IsActive)
                        throw new InvalidOperationException($"Game with ID {gameId} not found or inactive");

                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        GameId = gameId,
                        Game = game,
                        Quantity = quantity,
                        UnitPrice = game.Price
                    };
                    await _context.CartItems.AddAsync(cartItem);
                }

                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Added {Quantity} of game {GameId} to cart for user {UserId}", quantity, gameId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task RemoveItemFromCartAsync(int userId, int gameId)
        {
            try
            {
                var cart = await GetByUserIdAsync(userId);
                if (cart == null) return;

                var item = cart.Items.FirstOrDefault(i => i.GameId == gameId);
                if (item != null)
                {
                    _context.CartItems.Remove(item);
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Removed game {GameId} from cart for user {UserId}", gameId, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task UpdateItemQuantityAsync(int userId, int gameId, int newQuantity)
        {
            try
            {
                if (newQuantity <= 0)
                {
                    await RemoveItemFromCartAsync(userId, gameId);
                    return;
                }

                var cart = await GetByUserIdAsync(userId);
                if (cart == null) return;

                var item = cart.Items.FirstOrDefault(i => i.GameId == gameId);
                if (item != null)
                {
                    item.Quantity = newQuantity;
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Updated quantity of game {GameId} to {Quantity} in cart for user {UserId}", gameId, newQuantity, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item quantity in cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            try
            {
                var cart = await GetByUserIdAsync(userId);
                if (cart != null && cart.Items.Any())
                {
                    _context.CartItems.RemoveRange(cart.Items);
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cleared cart for user {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            try
            {
                var cart = await GetByUserIdAsync(userId);
                return cart?.Items.Sum(i => i.Quantity) ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart item count for user {UserId}", userId);
                throw;
            }
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            try
            {
                var cart = await GetByUserIdAsync(userId);
                return cart?.Items.Sum(i => i.UnitPrice * i.Quantity) ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating cart total for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> IsGameInCartAsync(int userId, int gameId)
        {
            try
            {
                var cart = await GetByUserIdAsync(userId);
                return cart?.Items.Any(i => i.GameId == gameId) ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if game {GameId} is in cart for user {UserId}", gameId, userId);
                throw;
            }
        }
    }
}
