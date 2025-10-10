using CloudGamesStore.Domain.Entities;
using CloudGamesStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudGamesStore.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public static OrderDto ToOrderDto(Order order)
        {
            if (order != null)
            {
                return new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    OrderNumber = order.OrderNumber,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    Items = order.Items?.Select(
                        orderItem => ToOrderItemDto(orderItem)
                        ).ToList()
                };
            }
            return new OrderDto();
        }

        public static OrderItemDto ToOrderItemDto(OrderItem orderItem)
        {
            if (orderItem != null)
            {
                return new OrderItemDto
                {
                    GameName = orderItem.GameName,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice,
                    TotalPrice = orderItem.TotalPrice
                };
            }
            return new OrderItemDto();
        }
    }
}
