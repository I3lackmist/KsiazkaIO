using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("RecipesCategories")]
    public class RecipesCategories {
        [Required]
        public int RecipeID {get; set;}

        [Required]
        public string CategoryName {get; set;}
    }
}