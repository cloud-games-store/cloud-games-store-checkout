using CloudGamesStore.Application.DTOs;
using CloudGamesStore.Application.DTOs.GameDtos;
using CloudGamesStore.Application.Interfaces;
using CloudGamesStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CloudGamesStore.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService checkoutService,
            ILogger<OrderController> logger)
        {
            _orderService = checkoutService;
            _logger = logger;
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<List<GameDto>>> GetById(string orderId)
        {
            var result = await _orderService.GetByOrderNumberAsync(orderId);

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetOrders()
        {
            try
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                Guid userId = Guid.Empty;

                if (claim != null)
                {
                    if (Guid.TryParse(claim.Value, out Guid tokenUserId))
                    {
                        userId = tokenUserId;
                    }
                }

                if (userId == Guid.Empty)
                    return NotFound("There is not a valid user logged in");

                var orders = await _orderService.GetOrdersForUser(userId);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting orders");
                return StatusCode(500, "An error occurred while getting orders");
            }
        }
    }
}
