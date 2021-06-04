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
    public class UserController : ControllerBase {
        private readonly AppDBContext _context;

        public UserController(AppDBContext context) {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers() {
            return await _context.Users.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user) {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string userdir = string.Concat(dbdir,"/",user.Login);
            Directory.CreateDirectory(userdir);

            string[] paths = {string.Concat(userdir,"/favrecipes.json"), string.Concat(userdir,"/userproducts.json"), string.Concat(userdir,"/userrecipes.json"), string.Concat(userdir,"/diet.json")};
            
            foreach(string path in paths) {
                using (StreamWriter sw = System.IO.File.CreateText(path)) {
                    sw.WriteLine("[");
                    sw.WriteLine("]");
                }
            }
            
            return CreatedAtAction("CreateUser", new{ login = user.Login, name = user.Name }, user);
        }

        [HttpGet("favrecipe")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetFavRecipes([FromHeader] string userName) {
            string userlogin = GetUserLogin(userName);
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string userfavspath = string.Concat(dbdir,"/",userlogin,"/favrecipes.json");

            string userFavsData = await System.IO.File.ReadAllTextAsync(userfavspath);
            var favlist = JsonConvert.DeserializeObject<List<RecipeID>>(userFavsData);
            var rlist = new List<Recipe>();

            foreach(RecipeID entry in favlist) {
                rlist.Add(await _context.Recipes.FindAsync(entry.id));
            }
            return rlist;
        }

        [HttpPost("favrecipe/{id}")]
        public async Task<ActionResult<bool>> AddRecipeToFavs([FromHeader] string userName, int id) {
            string userlogin = GetUserLogin(userName);
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string userfavspath = string.Concat(dbdir,"/",userlogin,"/favrecipes.json");

            string userFavsData = await System.IO.File.ReadAllTextAsync(userfavspath);
            var list = JsonConvert.DeserializeObject<List<RecipeID>>(userFavsData);

            foreach( RecipeID entry in list) {
                if(entry.id == id) return NoContent();
            }
            list.Add(new RecipeID {id = id});

            string newjson = JsonConvert.SerializeObject(list, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(userfavspath, newjson);
            return CreatedAtAction("AddRecipeToFavs", new { recipeid = id, uname = userName});
        }
        
        [HttpDelete("favrecipe/{id}")]
        public async Task<IActionResult> DeleteRecipeFromFavs([FromHeader] string userName, int id) {
            string userlogin = GetUserLogin(userName);
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string userfavspath = string.Concat(dbdir,"/",userlogin,"/favrecipes.json");

            string userFavsData = await System.IO.File.ReadAllTextAsync(userfavspath);
            var list = JsonConvert.DeserializeObject<List<RecipeID>>(userFavsData);
            
            foreach( RecipeID entry in list) {
                if(entry.id == id) list.Remove(entry);
                break;
            }

            string newjson = JsonConvert.SerializeObject(list, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(userfavspath, newjson);
            return NoContent();
        }

        [HttpPut("ban/{bannedUserLogin}")]
        public async Task<IActionResult> BanUser([FromHeader] string userName, string bannedUserLogin) {
            string ulogin = GetUserLogin(userName);
            if (!_context.Moderators.Any(mod => mod.Login == ulogin)) {
                return Forbid();
            }

            Ban ban = new Ban {Login = bannedUserLogin, DateSince = DateTime.Now, DateUntil = DateTime.Now.AddDays(14)};
            _context.Bans.Add(ban);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        private string GetUserLogin(string userName) {
            var user = _context.Users.Where( user => user.Name == userName).First();
            return user.Login;
        }
    }
}