using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("ArticleAuthor")]
    public class ArticleAuthor {
        [Key]
        public string Login {get; set;}
        [Key]
        public int ArticleID {get; set;}
    }
}