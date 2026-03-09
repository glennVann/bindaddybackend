using BinDaddy.Backend.DTOs;
using BinDaddy.Backend.Models;

namespace BinDaddy.Backend.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .ToListAsync();

                return products.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                throw;
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                return product != null ? MapToDto(product) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product {id}");
                throw;
            }
        }

        public async Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.CategoryId == categoryId)
                    .Include(p => p.Category)
                    .ToListAsync();

                return products.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting products for category {categoryId}");
                throw;
            }
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            try
            {
                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    CategoryId = dto.CategoryId,
                    ImageUrl = dto.ImageUrl
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                throw;
            }
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    throw new KeyNotFoundException($"Product {id} not found");

                if (!string.IsNullOrEmpty(dto.Name))
                    product.Name = dto.Name;
                if (!string.IsNullOrEmpty(dto.Description))
                    product.Description = dto.Description;
                if (dto.Price.HasValue)
                    product.Price = dto.Price.Value;
                if (dto.Stock.HasValue)
                    product.Stock = dto.Stock.Value;
                if (!string.IsNullOrEmpty(dto.ImageUrl))
                    product.ImageUrl = dto.ImageUrl;

                product.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product {id}");
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return false;

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product {id}");
                throw;
            }
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                ImageUrl = product.ImageUrl
            };
        }
    }
}
