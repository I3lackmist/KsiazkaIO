using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("Bans")]
    public class Ban {
        [Key]
        [Required]
        public string Login {get; set;}

        [Required]
        public DateTime DateSince {get; set;}

        [Required]
        public DateTime DateUntil {get; set;}
    }
}