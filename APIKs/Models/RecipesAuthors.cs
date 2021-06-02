using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("RecipesAuthors")]
    public class RecipesAuthors {
        [Required]
        public int RecipeID {get; set;}

        [Required]
        public string Login {get; set;}
    }
}