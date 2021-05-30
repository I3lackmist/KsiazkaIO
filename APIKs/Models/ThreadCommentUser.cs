using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    public class ArticleCommentUser {
        [Key]
        public int ArticleCommentID {get; set;}
        [Key]
        public int ArticleID {get; set;}
        [Key]
        public string Login {get; set;}
    }
}