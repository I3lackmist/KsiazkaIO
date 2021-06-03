using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIKs.Data;
using APIKs.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace APIKs.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase {
        private readonly AppDBContext _context;

        public RecipesController(AppDBContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> RecipesGetList() {
            return await _context.Recipes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> RecipesGetByID(int id) {
            return await _context.Recipes.FindAsync(id);
        }

        [HttpGet("by/{userName}")]
        public async Task<ActionResult<IEnumerable<Recipe>>> RecipesGetByAuthor(string userName) {
            var byAuthor = await _context.Recipes.Where( recipe => recipe.Author.Equals(userName) ).ToListAsync();
            return byAuthor;
        }

        [HttpGet("{id}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesInRecipe(int id) {
            List<Category> categories = new List<Category>();
            var recipescategories = await _context.RecipesCategories.Where(rc => rc.RecipeID == id).ToListAsync();
            foreach (var entry in recipescategories) {
                categories.Add(new Category{ CategoryName = entry.CategoryName });
            }
            return categories;
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsInRecipe(int id) {
            List<Product> products = new List<Product>();
            var recipesproducts = await _context.RecipesProducts.Where(rp => rp.RecipeID == id).ToListAsync();
            foreach (var entry in recipesproducts) {
                products.Add(_context.Products.Find(entry.ProductID));
            }
            return products;
        }

        [HttpGet("{id}/comments")]
        public async Task<ActionResult<IEnumerable<RecipeComment>>> GetRecipeComments(int id) {
            var rcu = await _context.RecipesComments.Where( rscs => rscs.RecipeID == id).ToListAsync();
            List<RecipeComment> comments = new List<RecipeComment>();
            foreach(var entry in rcu) {
                comments.Add(_context.RecipeComments.Find(entry.RecipeCommentID));
            }
            return comments;
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe([FromBody] JsonElement data, [FromHeader] string userName) {
            string jsonstr = System.Text.Json.JsonSerializer.Serialize(data);
            dynamic json = JsonConvert.DeserializeObject(jsonstr);
            Recipe recipe = new Recipe { Name = json["Name"], Description = json["Description"], Author = userName };
            _context.Recipes.Add(recipe);
            
            await _context.SaveChangesAsync();
            
            Newtonsoft.Json.Linq.JArray productids = json["ProductIDs"];
            List<int> idarray = productids.ToObject<List<int>>();

            foreach(int productid in idarray) {
                _context.RecipesProducts.Add(new RecipesProducts {RecipeID = recipe.RecipeID, ProductID = productid});
            }
            
            string category = json["Category"];
            _context.RecipesCategories.Add(new RecipesCategories {RecipeID = recipe.RecipeID, CategoryName = category});

            await _context.SaveChangesAsync();
            return CreatedAtAction("PostRecipe", new { id = recipe.RecipeID, name = recipe.Name, login = userName }, recipe);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe([FromHeader] string userName, int id) {
            Recipe recipe = await _context.Recipes.FindAsync(id);
            if(recipe == null) {
                return NotFound();
            }
            //Mod check
            _context.Recipes.Remove(recipe);

            try {
                RecipesCategories recipescategoriesentry = _context.RecipesCategories.Where(recipe => recipe.RecipeID.Equals(id)).First();
                _context.RecipesCategories.Remove(recipescategoriesentry);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            try {
                List<RecipesProducts> recipeproductlist = await _context.RecipesProducts.Where(recipe => recipe.RecipeID.Equals(id)).ToListAsync();
                foreach(RecipesProducts entry in recipeproductlist) {
                    _context.Remove(entry);
                }
            }
             catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}