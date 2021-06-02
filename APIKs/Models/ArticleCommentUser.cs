using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("ArticleCommentUser")]
    public class ArticleCommentUser {
        [Required]
        public int ArticleCommentID {get; set;}

        [Required]
        public int ArticleID {get; set;}

        [Required]
        public string Login {get; set;}
    }
}