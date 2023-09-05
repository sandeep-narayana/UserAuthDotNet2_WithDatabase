
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

        public ProjectController(ILogger<ProjectController> logger, ICategoryRepository cat)
        {
            _logger = logger;
            _cat = cat;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
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

}