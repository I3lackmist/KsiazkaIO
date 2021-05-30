using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace APIKs.Models {
    [Table("Categories")]
    public class Category {
        [Key]
        public string CategoryName {get; set;}
    }
}