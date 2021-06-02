using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("RecipesProducts")]
       public class RecipesProducts {
        [Required]
        public int RecipeID {get; set;}

        [Required]
        public int ProductID {get; set;}
        
        [Required]
        public float Amount {get; set;}
    }
}