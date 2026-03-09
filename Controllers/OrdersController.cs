using Microsoft.AspNetCore.Mvc;
using BinDaddy.Backend.DTOs;
using BinDaddy.Backend.Services;

namespace BinDaddy.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound(new { message = $"Order {id} not found" });

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<OrderDto>>> GetOrdersByUser(int userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders for user {userId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(dto);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, dto);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Order {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);
                if (!result)
                    return NotFound(new { message = $"Order {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
