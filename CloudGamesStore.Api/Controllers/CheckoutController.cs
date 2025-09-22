using CloudGamesStore.Application.DTOs;
using CloudGamesStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CloudGamesStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(ICheckoutService checkoutService,
            ILogger<CheckoutController> logger)
        {
            _checkoutService = checkoutService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CheckoutResponse>> ProcessCheckout([FromBody] CheckoutRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _checkoutService.ProcessCheckoutAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing checkout request");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("summary/{userId}")]
        public async Task<ActionResult<OrderSummary>> GetOrderSummary(
            int userId,
            [FromQuery] List<string> couponCodes)
        {
            try
            {
                var summary = await _checkoutService.CalculateOrderSummaryAsync(userId, couponCodes ?? new List<string>());
                return Ok(summary);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating order summary for user {UserId}", userId);
                return StatusCode(500, "An error occurred while calculating order summary");
            }
        }
    }
}
