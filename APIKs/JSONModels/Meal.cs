using System;

namespace APIKs.JSONModels {
    public class Meal {
        public string MealName {get; set;}
        public DateTime DateToConsume {get; set;}
        public int[] ProductIDs {get; set;}
        public int[] RecipeIDs {get; set;}
    }
}