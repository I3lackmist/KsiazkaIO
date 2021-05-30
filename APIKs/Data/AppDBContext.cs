using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIKs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace APIKs.Data {
    public class AppDBContext : DbContext {
        public DbSet<Product> Products {get; set;}
        public DbSet<User> Users {get; set;}
        public DbSet<Recipe> Recipes {get; set;}
        public DbSet<RecipesAuthors> RecipesAuthors {get; set;}
        public DbSet<RecipesProducts> RecipesProducts {get; set;}
        public DbSet<RecipesCategories> RecipesCategories {get; set;}
        public AppDBContext(DbContextOptions<AppDBContext> options):base(options) { }
    }
}