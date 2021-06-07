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
using APIKs.JSONModels;
using APIKs.Models;

namespace APIKs.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class DietController : ControllerBase {
        private readonly AppDBContext _context;
        public DietController(AppDBContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Meal>>> GetDiet([FromHeader] string userName) {
            string ulogin = GetUserLogin(userName);
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string dietpath = string.Concat(dbdir,"/",ulogin,"/diet.json");

            string userFavsData = await System.IO.File.ReadAllTextAsync(dietpath);
            var list = JsonConvert.DeserializeObject<List<Meal>>(userFavsData);
            return list;
        }

        [HttpPut]
        public async Task<IActionResult> AddMealToDiet([FromHeader] string userName, [FromBody] Meal meal) {
            string ulogin = GetUserLogin(userName);
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string dietpath = string.Concat(dbdir,"/",ulogin,"/diet.json");
            
            DeleteOldMeals(dietpath);

            string dietData = await System.IO.File.ReadAllTextAsync(dietpath);
            var list = JsonConvert.DeserializeObject<List<Meal>>(dietData);

            list.Add(meal);
            string newjson = JsonConvert.SerializeObject(list, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(dietpath, newjson);

            return Ok();
        }

        async void DeleteOldMeals(string path) {
            string dietData = await System.IO.File.ReadAllTextAsync(path);
            var list = JsonConvert.DeserializeObject<List<Meal>>(dietData);
            foreach (Meal meal in list) {
                if( meal.DateToConsume < DateTime.Today ) {
                    list.Remove(meal);
                }
            }

            string newjson = JsonConvert.SerializeObject(list, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(path, newjson);
        }

        [HttpPut("{mealname}/{days}/addproduct")] 
        public async Task<IActionResult> AddProductToMeal([FromHeader] string userName, [FromBody] ProductID productid, string mealname, int days) {
            if(days>7) return Forbid();
            
            string ulogin = GetUserLogin(userName);
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string dietpath = string.Concat(dbdir,"/",ulogin,"/diet.json");
            
            DateTime targetdate = DateTime.Now.AddDays(days).Date;

            string dietData = await System.IO.File.ReadAllTextAsync(dietpath);
            var list = JsonConvert.DeserializeObject<List<Meal>>(dietData);

            Meal targetmeal = list.Where( meal => meal.MealName == mealname && meal.DateToConsume.Date == targetdate).First();
            
            targetmeal.ProductIDs =  targetmeal.ProductIDs.Append(productid.id).ToArray(); 
            
            string newjson = JsonConvert.SerializeObject(list, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(dietpath, newjson);

            return Ok();
        }

        [HttpPut("{mealname}/{days}/addrecipe")] 
        public async Task<IActionResult> AddRecipeToMeal([FromHeader] string userName, [FromBody] RecipeID recipeid, string mealname, int days) {
            if(days>7) return Forbid();

            string ulogin = GetUserLogin(userName);
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string dietpath = string.Concat(dbdir,"/",ulogin,"/diet.json");

            DateTime targetdate = DateTime.Now.AddDays(days).Date;

            string dietData = await System.IO.File.ReadAllTextAsync(dietpath);
            var list = JsonConvert.DeserializeObject<List<Meal>>(dietData);
            
            Meal targetmeal = list.Where( meal => meal.MealName == mealname && meal.DateToConsume.Date == targetdate).First();

            targetmeal.ProductIDs =  targetmeal.ProductIDs.Append(recipeid.id).ToArray(); 

            string newjson = JsonConvert.SerializeObject(list, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(dietpath, newjson);

            return Ok();
        }

        [HttpGet("{mealname}/mealsummary")]
        public async Task<ActionResult<FoodSummary>> GetMealSummary([FromHeader] string userName, string mealname) {
            string ulogin = GetUserLogin(userName);
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string dietpath = string.Concat(dbdir,"/",ulogin,"/diet.json");

            string dietData = await System.IO.File.ReadAllTextAsync(dietpath);
            var list = JsonConvert.DeserializeObject<List<Meal>>(dietData);
            Meal meal = list.Where( meal => meal.MealName == mealname).First();
            FoodSummary summary = new FoodSummary();

            foreach(int recipeid in meal.RecipeIDs) {
                Recipe recipe = await _context.Recipes.FindAsync(recipeid);
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
            }

            foreach(int productid in meal.ProductIDs) {
                Product product = await _context.Products.FindAsync(productid);
                summary.Carbohydrates += product.Carbohydrates;
                summary.Fats += product.Fats;
                summary.Proteins += product.Proteins;
                summary.Kcal += product.Kcal;
            }

            return summary;
        }

        [HttpGet("{days}/daysummary")]
        public async Task<ActionResult<FoodSummary>> GetDaySummary([FromHeader] string userName, int days) {
            string ulogin = GetUserLogin(userName);
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string dietpath = string.Concat(dbdir,"/",ulogin,"/diet.json");
            

            string dietData = await System.IO.File.ReadAllTextAsync(dietpath);
            var list = JsonConvert.DeserializeObject<List<Meal>>(dietData);
            
            DateTime targetdate = DateTime.Now.AddDays(days);
            
            List<Meal> meals = list.Where( meal => meal.DateToConsume.Date == targetdate.Date).ToList();
            FoodSummary summary = new FoodSummary();
            foreach (Meal meal in meals) {
                foreach(int recipeid in meal.RecipeIDs) {
                    Recipe recipe = await _context.Recipes.FindAsync(recipeid);
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
                }

                foreach(int productid in meal.ProductIDs) {
                    Product product = await _context.Products.FindAsync(productid);
                    summary.Carbohydrates += product.Carbohydrates;
                    summary.Fats += product.Fats;
                    summary.Proteins += product.Proteins;
                    summary.Kcal += product.Kcal;
                }
            }

            return summary;
        }


        private string GetUserLogin(string userName) {
            var user = _context.Users.Where( user => user.Name == userName).First();
            return user.Login;
        }
    }
    
}