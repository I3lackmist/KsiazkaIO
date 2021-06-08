using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

using APIKs.Data;
using APIKs.Models;
using APIKs.JSONModels;

namespace APIKs.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase {
        private readonly AppDBContext _context;

        public RecipeController(AppDBContext context) {
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

        [HttpGet("{category}")]
        public async Task<ActionResult<IEnumerable<Recipe>>> RecipesGetByCategory(string category) {
            var inCategory = await _context.RecipesCategories.Where( recipescategories => recipescategories.CategoryName.Equals(category)).ToListAsync();
            List<Recipe> recipes = new List<Recipe>();
            foreach (RecipesCategories entry in inCategory) {
                recipes.Add(_context.Recipes.Find(entry.RecipeID));
            }
            return recipes;
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

        [HttpPost] // For moderators for instant access
        public async Task<ActionResult<Recipe>> PostRecipe([FromBody] PrivateRecipe data, [FromHeader] string userName) {
            Recipe recipe = new Recipe { Name = data.Name, Description = data.Description, Author = userName };
            _context.Recipes.Add(recipe);
            
            int[] productids = data.ProductIDs;

            foreach(int productid in productids) {
                _context.RecipesProducts.Add(new RecipesProducts {RecipeID = recipe.RecipeID, ProductID = productid});
            }
            
            string[] categories = data.Categories;
            foreach(string category in categories){
                if (!_context.Categories.Any(cat => cat.CategoryName ==  category)) {
                    _context.Categories.Add(new Category{CategoryName = category});
                }
                _context.RecipesCategories.Add(new RecipesCategories {RecipeID = recipe.RecipeID, CategoryName = category});
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction("PostRecipe", new { id = recipe.RecipeID, name = recipe.Name, username = userName }, recipe);
        }

        [HttpPost("{id}/comment")]
        public async Task<IActionResult> PostComment([FromBody] RecipeComment comment, [FromHeader] string userName, int id) {
            comment.Author = userName;
            comment.Date = DateTime.Now;
            _context.RecipeComments.Add(comment);
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet("{id}/summary")]
        public async Task<ActionResult<FoodSummary>> GetRecipeSummary(int id) {
            Recipe recipe = _context.Recipes.Find(id);
            FoodSummary summary = new FoodSummary();

            var recipesProducts = await _context.RecipesProducts.Where( rp => rp.RecipeID == recipe.RecipeID).ToListAsync();
            
            List<Product> plist = new List<Product>();
                
            foreach(var entry in recipesProducts) {
                plist.Add(_context.Products.Find(entry.ProductID));
            }

            foreach(Product product in plist) {
                summary.Carbohydrates += product.Carbohydrates;
                summary.Fats += product.Fats;
                summary.Proteins += product.Proteins;
                summary.Kcal += product.Kcal;
            }

            return summary;
        }

        [HttpPost("ticket")]
        public async Task<IActionResult> PostRecipeTicket([FromBody] PrivateRecipe data, [FromHeader] string userName) {
            string userLogin = _context.Users.Where( u => u.Name == userName).First().Login;
            
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");

            string recipepath = string.Concat(dbdir,"/",userLogin,"/userrecipes.json");

            string recipeData = await System.IO.File.ReadAllTextAsync(recipepath);

            var rlist = JsonConvert.DeserializeObject<List<PrivateRecipe>>(recipeData);
            
            data.RecipeID = -(rlist.Count+1);   

            rlist.Add(data);
            
            string newjson = JsonConvert.SerializeObject(rlist, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(recipepath, newjson);  

            _context.RecipeTickets.Add(new RecipeTicket{ RecipeLocalID = data.RecipeID, CreatorLogin = userLogin, DatePosted = DateTime.Now });
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe([FromHeader] string userName, int id) {
            Recipe recipe = await _context.Recipes.FindAsync(id);
            if(recipe == null) {
                return NotFound();
            }

            string userLogin = _context.Users.Where( u => u.Name == userName).First().Login;
            bool isOwnerOrModerator = (_context.Recipes.Find(id).Author == userName) || (_context.Moderators.Any( mod => mod.Login == userLogin));
            
            if(!isOwnerOrModerator) return Forbid();

            _context.Recipes.Remove(recipe);

            try {
                IEnumerable<RecipesCategories> recipescategoriesentries = await _context.RecipesCategories.Where(recipe => recipe.RecipeID.Equals(id)).ToListAsync();
                foreach(var entry in recipescategoriesentries) {
                    _context.RecipesCategories.Remove(entry);
                }
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
            return Ok();
        }
    }
}