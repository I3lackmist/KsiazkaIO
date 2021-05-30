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
    public class ProductsController : ControllerBase {
        private readonly AppDBContext _context;

        public ProductsController(AppDBContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct() {
            return await _context.Products.ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<Product>> GetProduct([FromBody] int id) {
            var product = await _context.Products.FindAsync(id);

            if (product == null) {
                return NotFound();
            }

            return product;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct([FromBody] string name) {
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
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product product) {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = product.ProductID }, product);
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
