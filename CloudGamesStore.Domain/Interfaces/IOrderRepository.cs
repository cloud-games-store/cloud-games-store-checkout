using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Domain.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetByUserIdAsync(Guid userId);
        Task<Order?> GetByOrderNumberAsync(string orderNumber);
        Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Order>> GetRecentOrdersAsync(int count = 10);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<bool> UpdateOrderStatusAsync(string orderNumber, OrderStatus newStatus);
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetOrderCountByStatusAsync(OrderStatus status);
        Task<bool> IsOrderNumberUniqueAsync(string orderNumber);
    }
}
