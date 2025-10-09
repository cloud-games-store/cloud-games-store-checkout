using CloudGamesStore.Application.DTOs.CartDtos;
using CloudGamesStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CloudGamesStore.Api.Controllers
{
    [ApiController]
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

        [HttpGet("GetCart")]
        public async Task<ActionResult<CartItemDto>> GetCart()
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

                var cart = await _cartService.GetCartByUserIdAsync(userId);

                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting cart");
                return StatusCode(500, "An error occurred while getting the cart");
            }
            throw new NotImplementedException();
        }

        [HttpPost("AddItem")]
        public async Task<ActionResult<CartItemDto>> AddItemToCartAsync(Guid userId,
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
        public async Task<ActionResult> RemoveItemFromCartAsync(Guid userId,
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
        public async Task<ActionResult<CartItemDto>> UpdateItemQuantityAsync(Guid userId,
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

        [HttpDelete("ClearCart")]
        public async Task<ActionResult> ClearCart(Guid userId)
        {
            try
            {
                await _cartService.ClearCart(userId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while clearing cart {cartId}", userId);
                return StatusCode(500, "An error occurred while clearing the cart");
            }
        }
    }
}
