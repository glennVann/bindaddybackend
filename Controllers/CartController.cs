using Microsoft.AspNetCore.Mvc;
using BinDaddy.Backend.DTOs;
using BinDaddy.Backend.Services;

namespace BinDaddy.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<CartDto>> GetCart(int userId)
        {
            try
            {
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null)
                    return NotFound(new { message = $"Cart for user {userId} not found" });

                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart for user {userId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("user/{userId}/items")]
        public async Task<ActionResult<CartDto>> AddToCart(int userId, AddToCartDto dto)
        {
            try
            {
                var cart = await _cartService.AddToCartAsync(userId, dto);
                return Ok(cart);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding to cart for user {userId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("user/{userId}/items/{cartItemId}")]
        public async Task<ActionResult<CartDto>> UpdateCartItem(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            try
            {
                var cart = await _cartService.UpdateCartItemAsync(userId, cartItemId, dto);
                return Ok(cart);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart item {cartItemId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("user/{userId}/items/{cartItemId}")]
        public async Task<ActionResult<CartDto>> RemoveFromCart(int userId, int cartItemId)
        {
            try
            {
                var cart = await _cartService.RemoveFromCartAsync(userId, cartItemId);
                return Ok(cart);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cart item {cartItemId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("user/{userId}")]
        public async Task<ActionResult<CartDto>> ClearCart(int userId)
        {
            try
            {
                var cart = await _cartService.ClearCartAsync(userId);
                return Ok(cart);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing cart for user {userId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
