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
        public DbSet<ArticleAuthor> ArticleAuthor {get; set;}
        public DbSet<ArticleComment> ArticleComments {get; set;}
        public DbSet<ArticleCommentUser> ArticleCommentUser {get; set;}
        public DbSet<Ban> Bans {get; set;}
        public DbSet<Category> Categories {get; set;}
        public DbSet<Moderator> Moderators {get; set;}
        public DbSet<Product> Products {get; set;}
        public DbSet<Recipe> Recipes {get; set;}
        public DbSet<RecipeComment> RecipeComments {get; set;}
        public DbSet<RecipeCommentUser> RecipeCommentUser {get; set;}
        public DbSet<RecipesAuthors> RecipesAuthors {get; set;}
        public DbSet<RecipesCategories> RecipesCategories {get; set;}
        public DbSet<RecipesProducts> RecipesProducts {get; set;}
        public DbSet<User> Users {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ArticleAuthor>().HasKey( aa => new {aa.ArticleID, aa.Login});
            modelBuilder.Entity<ArticleCommentUser>().HasKey( acu => new {acu.ArticleCommentID, acu.ArticleID, acu.Login});
            modelBuilder.Entity<RecipeCommentUser>().HasKey( rcu => new {rcu.RecipeID, rcu.RecipeCommentID, rcu.Login});
            modelBuilder.Entity<RecipesAuthors>().HasKey( ra => new {ra.RecipeID, ra.Login});
            modelBuilder.Entity<RecipesCategories>().HasKey( rc => new {rc.RecipeID, rc.CategoryName});
            modelBuilder.Entity<RecipesProducts>().HasKey( rp => new { rp.RecipeID, rp.ProductID});
        }
        public AppDBContext(DbContextOptions<AppDBContext> options):base(options) {}
    }
}