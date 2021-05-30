using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    public class RecipesAuthors {
        [Key]
        [Required]
        public int RecipeID {get; set;}

        [Key]
        [Required]
        public string Login {get; set;}
    }
}