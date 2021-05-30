using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("Moderators")]
    public class Moderator {
        [Key]
        public string Login {get; set;}
        [Required]
        public DateTime dateSince {get; set;}
    }
}