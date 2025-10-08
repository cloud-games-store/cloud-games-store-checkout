using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Enums;
using CloudGamesStore.Domain.Interfaces;
using CloudGamesStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudGamesStore.Infrastructure.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(GameStoreCheckoutDbContext context, ILogger<OrderRepository> logger)
            : base(context, logger)
        {
        }

        public override async Task<Order?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Include(o => o.Items)
                        //.ThenInclude(i => i.Game)
                    .FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order with ID {OrderId}", id);
                throw;
            }
        }

        public async Task<List<Order>> GetByUserIdAsync(Guid userId)
        {
            try
            {
                return await _dbSet
                    .Include(o => o.Items)
                        //.ThenInclude(i => i.Game)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            try
            {
                return await _dbSet
                    .Include(o => o.Items)
                        //.ThenInclude(i => i.Game)
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order with number {OrderNumber}", orderNumber);
                throw;
            }
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try
            {
                return await _dbSet
                    .Include(o => o.Items)
                        //.ThenInclude(i => i.Game)
                    .Where(o => o.Status == status)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders with status {Status}", status);
                throw;
            }
        }

        public async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _dbSet
                    .Include(o => o.Items)
                        //.ThenInclude(i => i.Game)
                    .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders between {StartDate} and {EndDate}", startDate, endDate);
                throw;
            }
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count = 10)
        {
            try
            {
                return await _dbSet
                    .Include(o => o.Items)
                        //.ThenInclude(i => i.Game)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent orders");
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            try
            {
                var order = await _dbSet.FindAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found for status update", orderId);
                    return false;
                }

                var oldStatus = order.Status;
                order.Status = newStatus;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} status updated from {OldStatus} to {NewStatus}",
                    orderId, oldStatus, newStatus);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderNumber, OrderStatus newStatus)
        {
            try
            {
                var order = await _dbSet.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
                if (order == null)
                {
                    _logger.LogWarning("Order with number {OrderNumber} not found for status update", orderNumber);
                    return false;
                }

                return await UpdateOrderStatusAsync(order.Id, newStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for order {OrderNumber}", orderNumber);
                throw;
            }
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            try
            {
                return await _dbSet
                    .Where(o => o.Status == OrderStatus.Completed)
                    .SumAsync(o => o.TotalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total revenue");
                throw;
            }
        }

        public async Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _dbSet
                    .Where(o => o.Status == OrderStatus.Completed &&
                               o.CreatedAt >= startDate &&
                               o.CreatedAt <= endDate)
                    .SumAsync(o => o.TotalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating revenue between {StartDate} and {EndDate}", startDate, endDate);
                throw;
            }
        }

        public async Task<int> GetOrderCountByStatusAsync(OrderStatus status)
        {
            try
            {
                return await _dbSet.CountAsync(o => o.Status == status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting orders with status {Status}", status);
                throw;
            }
        }

        public async Task<bool> IsOrderNumberUniqueAsync(string orderNumber)
        {
            try
            {
                return !await _dbSet.AnyAsync(o => o.OrderNumber == orderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking uniqueness of order number {OrderNumber}", orderNumber);
                throw;
            }
        }
    }
}
