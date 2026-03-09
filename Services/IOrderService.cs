using BinDaddy.Backend.DTOs;
using BinDaddy.Backend.Models;

namespace BinDaddy.Backend.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<List<OrderDto>> GetOrdersByUserAsync(int userId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto);
        Task<bool> DeleteOrderAsync(int id);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ToListAsync();

                return orders.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                throw;
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);

                return order != null ? MapToDto(order) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order {id}");
                throw;
            }
        }

        public async Task<List<OrderDto>> GetOrdersByUserAsync(int userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ToListAsync();

                return orders.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders for user {userId}");
                throw;
            }
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            try
            {
                var order = new Order
                {
                    UserId = dto.UserId,
                    Status = "Pending",
                    Notes = dto.Notes
                };

                decimal totalAmount = 0;

                foreach (var item in dto.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null)
                        throw new KeyNotFoundException($"Product {item.ProductId} not found");

                    if (product.Stock < item.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = product.Price
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += product.Price * item.Quantity;

                    // Update stock
                    product.Stock -= item.Quantity;
                }

                order.TotalAmount = totalAmount;
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return MapToDto(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                    throw new KeyNotFoundException($"Order {id} not found");

                order.Status = dto.Status;
                if (dto.Status == "Completed")
                    order.DeliveryDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return MapToDto(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order {id}");
                throw;
            }
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                    return false;

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order {id}");
                throw;
            }
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                Notes = order.Notes,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };
        }
    }
}
