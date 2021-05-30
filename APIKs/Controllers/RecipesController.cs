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

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Recipe>>> RecipesGetByCategory(string category) {
            var inCategory = await _context.RecipesCategories.Where( recipescategories => recipescategories.Category.Equals(category)).ToListAsync();
            List<Recipe> recipes = new List<Recipe>();
            foreach (RecipesCategories entry in inCategory) {
                recipes.Append(_context.Recipes.Find(entry.RecipeID));
            }
            return recipes;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> RecipesGetByAuthor([FromQuery] string userName) {
            string userLogin = _context.Users.Where( user => user.Name.Equals(userName)).First().Login;
            var byAuthor = await _context.RecipesAuthors.Where( recipesauthors => recipesauthors.Login.Equals(userLogin) ).ToListAsync();
            List<Recipe> list = new List<Recipe>();
            foreach (RecipesAuthors entry in byAuthor) {
                list.Append(_context.Recipes.Find(entry.RecipeID));
            }
            return list;
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe([FromBody] string data, [FromHeader] string userLogin) {
            dynamic json = JsonConvert.DeserializeObject(data);
            Recipe recipe = new Recipe { Name = json["Name"], Description = json["Description"]};
            _context.Recipes.Add(recipe);
            
            int[] productids = json["RecipeIDs"];
            foreach(int productid in productids) {
                _context.RecipesProducts.Add(new RecipesProducts {RecipeID = recipe.RecipeID, ProductID = productid});
            }

            _context.RecipesAuthors.Add(new RecipesAuthors {RecipeID = recipe.RecipeID, Login = userLogin});
            
            await _context.SaveChangesAsync();
            return CreatedAtAction("PostRecipe", new { id = recipe.RecipeID, name = recipe.Name, login = userLogin }, recipe);
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
            _context.Recipes.Remove(recipe);
            _context.RecipesAuthors.Remove(recipesauthorsentry);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
