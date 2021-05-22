using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIKs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace APIKs.Data {
    public class AppDBContext:DbContext {
        public DbSet<Products> Products {get; set;}
        public AppDBContext(DbContextOptions<AppDBContext> options):base(options){}
    }
}