using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    public class Comment {
        [Key]
        public int CommentID {get; set;}
        public string Body {get; set;}
        public DateTime DatePosted {get; set;}
        public int Likes {get; set;}
    }
}