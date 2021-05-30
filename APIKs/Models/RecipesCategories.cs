using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    public class RecipesCategories {
        [Key]
        [Required]
        public int RecipeID {get; set;}

        [Key]
        [Required]
        public string Category {get; set;}
    }
}