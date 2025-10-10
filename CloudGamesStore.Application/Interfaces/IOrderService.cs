using CloudGamesStore.Application.DTOs;
using CloudGamesStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> GetByOrderNumberAsync(string orderNumber);
        Task<List<OrderDto>> GetOrdersForUser(Guid userId);
    }
}
