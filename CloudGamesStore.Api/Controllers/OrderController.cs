using CloudGamesStore.Application.DTOs.GameDtos;
using CloudGamesStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }
}
