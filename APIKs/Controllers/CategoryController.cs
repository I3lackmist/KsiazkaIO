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
        
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category) {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction("CreateCategory", new { name = category.CategoryName }, category);
        }
    }
}