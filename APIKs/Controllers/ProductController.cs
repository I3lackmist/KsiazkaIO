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

namespace APIKs.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase {
        private readonly AppDBContext _context;

        public ProductController(AppDBContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct() {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct([FromBody] int id) {
            var product = await _context.Products.FindAsync(id);

            if (product == null) {
                return NotFound();
            }

            return product;
        }

        [HttpGet("name")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct([FromQuery] string name) {
            var products = await _context.Products.Where(product => product.Name.Contains(name)).ToListAsync();
            if (products == null) {
                return NotFound();
            }
            return new ActionResult<IEnumerable<Product>>(products);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product) {
            if (id != product.ProductID) {
                return BadRequest();
            }
            _context.Entry(product).State = EntityState.Modified;
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ProductExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] PrivateProduct data) {
            Product product = new Product { 
                Name = data.Name, 
                Carbohydrates = data.Carbohydrates,
                Proteins = data.Proteins,
                Fats = data.Fats,
                Kcal = data.Kcal,
                Note = data.Note 
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = product.ProductID }, product);
        }

        [HttpPut("add")]
        public async Task<IActionResult> AddPrivateProduct([FromBody] PrivateProduct product, [FromHeader] string userName) {
            string userLogin = _context.Users.Where( u => u.Name == userName).First().Login;
            
            string dbdir = string.Concat(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(),"/DB/Users");

            string productpath = string.Concat(dbdir,"/",userLogin,"/userproducts.json");

            string productData = await System.IO.File.ReadAllTextAsync(productpath);
            
            var plist = JsonConvert.DeserializeObject<List<PrivateProduct>>(productData);
            product.ProductID = -(plist.Count + 1);

            plist.Add(product);

            string newjson = JsonConvert.SerializeObject(plist, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(productpath, newjson);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id) {
            var product = await _context.Products.FindAsync(id);
            if (product == null) {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ProductExists(int id) {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
