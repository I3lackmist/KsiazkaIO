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
            var byAuthor = await _context.RecipesAuthors.Where( recipesauthors => recipesauthors.Login.Equals(userName) ).ToListAsync();
            List<Recipe> list = new List<Recipe>();

            foreach (RecipesAuthors entry in byAuthor) {
                list.Add(_context.Recipes.Find(entry.RecipeID));
            }
            return list;
        }

        [HttpGet("{id}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesInRecipe(int id) {
            List<Category> categories = new List<Category>();
            var recipescategories = await _context.RecipesCategories.Where(rc => rc.RecipeID == id).ToListAsync();
            foreach (RecipesCategories entry in recipescategories) {
                categories.Add(new Category{ CategoryName = entry.CategoryName });
            }
            return categories;
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe([FromBody] JsonElement data) {
            string jsonstr = System.Text.Json.JsonSerializer.Serialize(data);
            dynamic json = JsonConvert.DeserializeObject(jsonstr);
            Recipe recipe = new Recipe { Name = json["Name"], Description = json["Description"]};
            _context.Recipes.Add(recipe);
            
            await _context.SaveChangesAsync();
            
            Newtonsoft.Json.Linq.JArray productids = json["ProductIDs"];
            List<int> idarray = productids.ToObject<List<int>>();
            string uname = json["UserName"];
            foreach(int productid in idarray) {
                _context.RecipesProducts.Add(new RecipesProducts {RecipeID = recipe.RecipeID, ProductID = productid});
            }
            
            string category = json["Category"];
            _context.RecipesCategories.Add(new RecipesCategories {RecipeID = recipe.RecipeID, CategoryName = category});

            _context.RecipesAuthors.Add(new RecipesAuthors {RecipeID = recipe.RecipeID, Login = uname});
            
            await _context.SaveChangesAsync();
            return CreatedAtAction("PostRecipe", new { id = recipe.RecipeID, name = recipe.Name, login = uname }, recipe);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe([FromHeader] string userLogin, int id) {
            Recipe recipe = await _context.Recipes.FindAsync(id);
            if(recipe == null) {
                return NotFound();
            }
            RecipesAuthors recipesauthorsentry = _context.RecipesAuthors.Where(recipe => recipe.RecipeID.Equals(id)).First();
            if(!recipesauthorsentry.Login.SequenceEqual(userLogin)) {
                //Moderator check
                return NoContent();
            }

            RecipesCategories recipescategoriesentry = _context.RecipesCategories.Where(recipe => recipe.RecipeID.Equals(id)).First();

            List<RecipesProducts> recipeproductlist = await _context.RecipesProducts.Where(recipe => recipe.RecipeID.Equals(id)).ToListAsync();

            _context.Recipes.Remove(recipe);
            _context.RecipesAuthors.Remove(recipesauthorsentry);
            _context.RecipesCategories.Remove(recipescategoriesentry);
            foreach(RecipesProducts entry in recipeproductlist) {
                _context.Remove(entry);
            }
            
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}