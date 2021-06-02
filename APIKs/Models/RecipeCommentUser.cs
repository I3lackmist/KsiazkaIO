using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("RecipeCommentUser")]
    public class RecipeCommentUser {
        [Required]
        public int RecipeCommentID {get; set;}
        
        [Required]
        public int RecipeID {get; set;}

        [Required]
        public string Login {get; set;}
    }
}