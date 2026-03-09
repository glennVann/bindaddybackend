using BinDaddy.Backend.DTOs;
using BinDaddy.Backend.Models;

namespace BinDaddy.Backend.Services
{
    public interface ICartService
    {
        Task<CartDto?> GetCartByUserIdAsync(int userId);
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);
        Task<CartDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto dto);
        Task<CartDto> RemoveFromCartAsync(int userId, int cartItemId);
        Task<CartDto> ClearCartAsync(int userId);
    }

    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartService> _logger;

        public CartService(ApplicationDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartDto?> GetCartByUserIdAsync(int userId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                return cart != null ? MapToDto(cart) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart for user {userId}");
                throw;
            }
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    throw new KeyNotFoundException($"Cart for user {userId} not found");

                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"Product {dto.ProductId} not found");

                if (product.Stock < dto.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += dto.Quantity;
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity,
                        Price = product.Price
                    };
                    cart.CartItems.Add(cartItem);
                }

                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return MapToDto(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding to cart for user {userId}");
                throw;
            }
        }

        public async Task<CartDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    throw new KeyNotFoundException($"Cart for user {userId} not found");

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (cartItem == null)
                    throw new KeyNotFoundException($"Cart item {cartItemId} not found");

                var product = cartItem.Product;
                if (product == null)
                    throw new KeyNotFoundException($"Product not found for cart item");

                if (product.Stock < dto.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

                cartItem.Quantity = dto.Quantity;
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return MapToDto(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart item {cartItemId}");
                throw;
            }
        }

        public async Task<CartDto> RemoveFromCartAsync(int userId, int cartItemId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    throw new KeyNotFoundException($"Cart for user {userId} not found");

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (cartItem == null)
                    throw new KeyNotFoundException($"Cart item {cartItemId} not found");

                cart.CartItems.Remove(cartItem);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return MapToDto(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cart item {cartItemId}");
                throw;
            }
        }

        public async Task<CartDto> ClearCartAsync(int userId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    throw new KeyNotFoundException($"Cart for user {userId} not found");

                cart.CartItems.Clear();
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return MapToDto(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing cart for user {userId}");
                throw;
            }
        }

        private CartDto MapToDto(Cart cart)
        {
            var cartItems = cart.CartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product?.Name ?? string.Empty,
                Quantity = ci.Quantity,
                Price = ci.Price,
                Subtotal = ci.Price * ci.Quantity
            }).ToList();

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                CartItems = cartItems,
                TotalPrice = cartItems.Sum(ci => ci.Subtotal)
            };
        }
    }
}
