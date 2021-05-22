using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    public class Products {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID {get; set;}
        public string Name {get; set;}
        public float Carbohydrates {get; set;}
        public float Proteins {get; set;}
        public float Fats {get; set;}
        public float Kcal {get; set;}
        public string Note {get; set;}
    }
}
