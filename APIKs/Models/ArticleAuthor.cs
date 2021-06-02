using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("ArticleAuthor")]
    public class ArticleAuthor {
        [Required]
        public string Login {get; set;}

        [Required]
        public int ArticleID {get; set;}
    }
}