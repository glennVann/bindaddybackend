using Microsoft.AspNetCore.Mvc;
using BinDaddy.Backend.DTOs;
using BinDaddy.Backend.Services;

namespace BinDaddy.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound(new { message = $"Product {id} not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting products for category {categoryId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
        {
            try
            {
                var product = await _productService.CreateProductAsync(dto);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto dto)
        {
            try
            {
                var product = await _productService.UpdateProductAsync(id, dto);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound(new { message = $"Product {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
