using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIKs.Data;
using APIKs.Models;

namespace APIKs.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase {
        private readonly AppDBContext _context;

        public CategoryController(AppDBContext context) {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories () {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("{category}")]
        public async Task<ActionResult<IEnumerable<Recipe>>> RecipesGetByCategory(string category) {
            var inCategory = await _context.RecipesCategories.Where( recipescategories => recipescategories.CategoryName.Equals(category)).ToListAsync();
            List<Recipe> recipes = new List<Recipe>();
            foreach (RecipesCategories entry in inCategory) {
                recipes.Add(_context.Recipes.Find(entry.RecipeID));
            }
            return recipes;
        }
        
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category) {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction("CreateCategory", new { name = category.CategoryName }, category);
        }
    }
}