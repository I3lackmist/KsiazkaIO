using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("ArticleComments")]
    public class ArticleComment {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleCommentID {get; set;}

        [Required]
        public string Body {get; set;}

        [Required]
        public DateTime Date {get; set;}

        [Required]
        public string Author {get; set;}

        public int Likes {get; set;} = 0;
    }
}