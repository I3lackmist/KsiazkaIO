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

        [HttpPost("favrecipe/{id}")]
        public async Task<ActionResult<bool>> AddRecipeToFavs([FromHeader] string userName, int id) {
            string userlogin = GetUserLogin(userName);
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");
            string userfavspath = string.Concat(dbdir,"/",userlogin,"/favrecipes.json");

            string userFavsData = await System.IO.File.ReadAllTextAsync(userfavspath);
            var list = JsonConvert.DeserializeObject<List<RecipeID>>(userFavsData);
            list.Add(new RecipeID {id = id});

            string newjson = JsonConvert.SerializeObject(list, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(userfavspath, newjson);
            return CreatedAtAction("AddRecipeToFavs", new { recipeid = id, uname = userName});
        }

        private string GetUserLogin(string userName) {
            var user = _context.Users.Where( user => user.Name == userName).First();
            return user.Login;
        }
    }
}