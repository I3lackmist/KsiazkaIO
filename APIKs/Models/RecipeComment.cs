using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("RecipeComments")]
    public class RecipeComment {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecipeCommentID {get; set;}
        [Required]
        public string Body {get; set;}
        [Required]
        public DateTime Date {get; set;}
        public int Likes {get; set;}
    }
}