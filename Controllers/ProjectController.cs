
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAuthDotBet2_WithDatabase.Repositories;

namespace UserAuthDotBet2_WithDatabase
{

    [ApiController]
    [Route("api/categories")]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private ICategoryRepository _cat;
        private IProductRepository _product;
        private ICartRepository _cart;

        public ProjectController(ILogger<ProjectController> logger, ICategoryRepository cat, IProductRepository product, ICartRepository cart)
        {
            _logger = logger;
            _cat = cat;
            _product = product;
            _cart = cart;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            // Test Only
            // Retrieve the user's claims
            var userClaims = HttpContext.User.Claims;

            // Print user claims to the console
            foreach (var claim in userClaims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                // Console.WriteLine(userClaims);
            }

            var categories = await _cat.get();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<String>> CreateCategory([FromBody] CategoryDTO newCategory)
        {
            if (newCategory == null)
            {
                return BadRequest("Invalid category data.");
            }

            try
            {
                var res = await _cat.AddCategory(newCategory);
                return res;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating category: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }

        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _product.get();
            return Ok(products);
        }

        [HttpGet("cart")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetUserCart(int userId)
        {
            var cart = await _cart.getCartById(userId);

            if (cart == null || !cart.Any())
            {
                return NoContent(); // Return 204 No Content if the cart is empty or doesn't exist.
            }
            return Ok(cart);
        }

        [HttpPost("cart/add/{userId}")]
        public async Task<ActionResult<string>> AddProductToCart([FromBody] Product product, int userId)
        {
            try
            {
                var result = await _cart.AddToCart(product, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while adding the product to the cart.");
            }
        }


    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class CategoryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; } // Assuming you have a category ID for the product
    }

    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; } // Assuming you have a category ID for the product
    }


    public class Cart
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; } // Added userId property
    }

}