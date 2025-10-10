using CloudGamesStore.Application.DTOs;
using CloudGamesStore.Application.Interfaces;
using CloudGamesStore.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IOrderRepository _orderRepository;

        public OrderService(ILogger<OrderService> logger,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> GetByOrderNumberAsync(string orderNumber)
        {
            var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);
            OrderDto result = new OrderDto();

            if (order != null)
                result = OrderDto.ToOrderDto(order);

            return result;
        }

        public async Task<List<OrderDto>> GetOrdersForUser(Guid userId)
        {
            var order = await _orderRepository.GetByUserIdAsync(userId);
            List<OrderDto> result = [];

            if (order != null)
                result = order.Select(o => OrderDto.ToOrderDto(o)).ToList();

            return result;
        }
    }
}
