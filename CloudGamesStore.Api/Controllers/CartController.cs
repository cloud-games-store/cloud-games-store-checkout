using CloudGamesStore.Application.DTOs.CartDtos;
using CloudGamesStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace CloudGamesStore.Api.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService,
            ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpPost("AddItem")]
        public async Task<ActionResult<CartItemDto>> AddItemToCartAsync(int userId,
            int gameId, int quantity = 1)
        {
            try
            {
                await _cartService.AddItemCartByUserIdAsync(userId, gameId, quantity);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding an item to the cart for user {UserId}", userId);
                return StatusCode(500, "An error occurred while adding an item to the cart");
            }
        }

        [HttpDelete("RemoveItem")]
        public async Task<ActionResult> RemoveItemFromCartAsync(int userId,
            int gameId)
        {
            try
            {
                await _cartService.RemoveItemFromCartAsync(userId, gameId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing an item from the cart for user {UserId}", userId);
                return StatusCode(500, "An error occurred while removing an item from the cart");
            }
        }

        [HttpPost("UpdateItem")]
        public async Task<ActionResult<CartItemDto>> UpdateItemQuantityAsync(int userId,
            int gameId, int newQuantity)
        {
            try
            {
                await _cartService.UpdateItemQuantityAsync(userId, gameId, newQuantity);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating an item quantity from the cart for user {UserId}", userId);
                return StatusCode(500, "An error occurred while updating an item quantity from the cart");
            }
        }
    }
}
