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
    public class UserController : ControllerBase {
        private readonly AppDBContext _context;

        public UserController(AppDBContext context) {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser ([FromBody] User user) {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("CreateUser", new{ login = user.Login, name = user.Name }, user);
        }
    }
}