using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("Products")]
    public class Product {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID {get; set;}

        [Required]
        public string Name {get; set;}
        
        [Required]
        public float Carbohydrates {get; set;}

        [Required]
        public float Proteins {get; set;}

        [Required]
        public float Fats {get; set;}

        [Required]
        public float Kcal {get; set;}

        public string Note {get; set;} = "";
    }
}
