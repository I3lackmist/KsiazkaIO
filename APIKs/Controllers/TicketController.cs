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
    public class TicketController : ControllerBase {
        
        private readonly AppDBContext _context;
        public TicketController(AppDBContext context) {
            _context = context;
        }
        private bool isMod(string name) {
            string userLogin = _context.Users.Where( u => u.Equals(name)).First().Login;
            bool is_mod = _context.Moderators.Any( mod => mod.Login.Equals(userLogin));
            return is_mod;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeTicket>>> GetTickets([FromHeader] string userName) {
            if(!isMod(userName)) return Forbid();
            return await _context.RecipeTickets.ToListAsync();
        }

        [HttpGet("{login}")]
        public async Task<ActionResult<IEnumerable<RecipeTicket>>> GetTicketsFromUser([FromHeader] string userName, string login) {
            if(!isMod(userName)) return Forbid();
            return await _context.RecipeTickets.Where( rt => rt.CreatorLogin.Equals(login)).ToListAsync();
        }

        [HttpPost("{login}/{id}")] //Typically the way recipes and products will be created
        public async Task<IActionResult> ApproveUserRecipe([FromHeader] string userName,string login, int id) {
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");

            string recipepath = string.Concat(dbdir,"/",login,"/userrecipes.json");
            string productpath = string.Concat(dbdir,"/",login,"/userproducts.json");

            string recipeData = await System.IO.File.ReadAllTextAsync(recipepath);
            string productData = await System.IO.File.ReadAllTextAsync(productpath);

            User user = await _context.Users.FindAsync(login);
            string uname = user.Name;

            var rlist = JsonConvert.DeserializeObject<List<PrivateRecipe>>(recipeData);
            var plist = JsonConvert.DeserializeObject<List<PrivateProduct>>(productData);
             
            PrivateRecipe precipe = rlist.Where( r => r.RecipeID == id).First();

            var participatingproducts = new List<PrivateProduct>();

            Recipe recipe = new Recipe {
                Name = precipe.Name,
                Description = precipe.Description,
                Author = uname
            };
            _context.Recipes.Add(recipe);

            int idx = 0;
            foreach(int productid in precipe.ProductIDs) {
                if(productid<0) {
                    PrivateProduct prproduct = plist.Where( ppr => ppr.ProductID == productid).First();
                    Product product = new Product { 
                            Name = prproduct.Name,
                            Kcal = prproduct.Kcal,
                            Carbohydrates = prproduct.Carbohydrates,
                            Fats = prproduct.Fats,
                            Proteins = prproduct.Proteins,
                            Note = prproduct.Note
                        };
                    _context.Products.Add(product);
                    _context.RecipesProducts.Add(new RecipesProducts { RecipeID = recipe.RecipeID, ProductID = product.ProductID, Amount = precipe.Amounts[idx++]});
                }
            }

            foreach(string category in precipe.Categories) {
                if (!_context.Categories.Any(cat => cat.CategoryName ==  category)) {
                    _context.Categories.Add(new Category{CategoryName = category});
                }
                _context.RecipesCategories.Add(new RecipesCategories {RecipeID = recipe.RecipeID, CategoryName = category});
            }

            await _context.SaveChangesAsync();
            
            rlist.Remove(precipe);
            foreach(PrivateProduct product in participatingproducts) {
                plist.Remove(product);
            }
            string newjson = JsonConvert.SerializeObject(rlist, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(recipepath, newjson);
            
            newjson = JsonConvert.SerializeObject(plist, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(productpath, newjson);
            
            return Ok();
        }
    }
}