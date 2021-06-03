using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("RecipesComments")]
    public class RecipesComments {
        [Required]
        public int RecipeCommentID {get; set;}
        
        [Required]
        public int RecipeID {get; set;}
    }
}