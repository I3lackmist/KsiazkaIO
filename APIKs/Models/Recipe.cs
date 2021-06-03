using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("Recipes")]
    public class Recipe {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecipeID {get; set;}

        [Required]
        public string Name {get; set;}

        [Required]
        public string Description {get; set;}

        [Required]
        public string Author {get; set;}
        
        public int Likes {get; set;} = 0;
    }
}