using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("Articles")]
    public class Article {
        [Key]
        public int ArticleID {get; set;}
        [Required]
        public string Title {get; set;}
        [Required]
        public string Body {get; set;}
    }
}