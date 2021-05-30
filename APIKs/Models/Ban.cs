using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("Bans")]
    public class Ban {
        [Key]
        public string Login {get; set;}
        public DateTime DateSince {get; set;}
        public DateTime DateUntil {get; set;}
    }
}