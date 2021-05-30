using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("Users")]
    public class User {
        [Required]
        [Key]
        public string Login {get; set;}

        [Required]
        public string Email {get; set;}

        [Required]
        public string Password {get; set;}
        
        [Required]
        public string Name {get; set;}

        [Required]
        public DateTime DateRegistered {get; set;}
    }
}