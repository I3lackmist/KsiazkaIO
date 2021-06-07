using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("RecipeTickets")]
    public class RecipeTicket {
        [Required]
        public int RecipeLocalID {get; set;}

        [Required]
        public int CreatorLogin {get; set;}

        [Required]
        public DateTime DatePosted {get; set;}
    }
}