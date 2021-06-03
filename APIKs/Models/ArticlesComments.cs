using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("ArticlesComments")]
    public class ArticlesComments {
        [Required]
        public int ArticleCommentID {get; set;}

        [Required]
        public int ArticleID {get; set;}
    }
}