using System;
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
    public class ArticleController : ControllerBase {
        private readonly AppDBContext _context;

        public ArticleController(AppDBContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> ArticlesGetList() {
            return await _context.Articles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> ArticlesGetByID(int id) {
            return await _context.Articles.FindAsync(id);
        }

        [HttpGet("by/{userName}")]
        public async Task<ActionResult<IEnumerable<Article>>> ArticlesGetByAuthor(string userName) {
            var byAuthor = await _context.Articles.Where( article => article.Author.Equals(userName) ).ToListAsync();
            return byAuthor;
        }

         [HttpGet("{id}/comments")]
        public async Task<ActionResult<IEnumerable<ArticleComment>>> GetArticleComments(int id) {
            var acu = await _context.ArticlesComments.Where( ascs => ascs.ArticleID == id).ToListAsync();
            List<ArticleComment> comments = new List<ArticleComment>();
            foreach(var entry in acu) {
                comments.Add(_context.ArticleComments.Find(entry.ArticleCommentID));
            }
            return comments;
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> PostArticle([FromBody] JsonElement data, [FromHeader] string userName) {
            string jsonstr = System.Text.Json.JsonSerializer.Serialize(data);
            dynamic json = JsonConvert.DeserializeObject(jsonstr);
            Article article = new Article { Title = json["Title"], Body = json["Body"], Author = userName };
            _context.Articles.Add(article);
        
            await _context.SaveChangesAsync();
            return CreatedAtAction("PostArticle", new { id = article.ArticleID, title = article.Title, userName = userName }, article);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle([FromHeader] string userName, int id) {
            Article article = await _context.Articles.FindAsync(id);
            if(article == null) {
                return NotFound();
            }
            
            string userLogin = _context.Users.Where( u => u.Name == userName).First().Login;
            bool isOwner = (_context.Articles.Find(id).Author == userName) && (_context.Moderators.Any( mod => mod.Login == userLogin));
            
            if(!isOwner) return Forbid();
            _context.Articles.Remove(article);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}