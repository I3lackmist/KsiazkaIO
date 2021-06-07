using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIKs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace APIKs.Data {
    public class AppDBContext : DbContext {
        public DbSet<Article> Articles {get; set;}
        public DbSet<ArticleComment> ArticleComments {get; set;}
        public DbSet<ArticlesComments> ArticlesComments {get; set;}
        public DbSet<Ban> Bans {get; set;}
        public DbSet<Category> Categories {get; set;}
        public DbSet<Moderator> Moderators {get; set;}
        public DbSet<Product> Products {get; set;}
        public DbSet<Recipe> Recipes {get; set;}
        public DbSet<RecipeComment> RecipeComments {get; set;}
        public DbSet<RecipesCategories> RecipesCategories {get; set;}
        public DbSet<RecipesProducts> RecipesProducts {get; set;}
        public DbSet<RecipesComments> RecipesComments {get; set;}
        public DbSet<User> Users {get; set;}

        public DbSet<RecipeTicket> RecipeTickets {get; set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ArticlesComments>().HasKey( acu => new {acu.ArticleCommentID, acu.ArticleID});
            modelBuilder.Entity<RecipesComments>().HasKey( rcu => new {rcu.RecipeID, rcu.RecipeCommentID});
            modelBuilder.Entity<RecipesCategories>().HasKey( rc => new {rc.RecipeID, rc.CategoryName});
            modelBuilder.Entity<RecipesProducts>().HasKey( rp => new { rp.RecipeID, rp.ProductID});
            modelBuilder.Entity<RecipeTicket>().HasKey( rt => new { rt.RecipeLocalID, rt.CreatorLogin });
        }
        public AppDBContext(DbContextOptions<AppDBContext> options):base(options) {}
    }
}