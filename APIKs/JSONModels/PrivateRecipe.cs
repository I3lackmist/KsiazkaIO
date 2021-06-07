using System;

using APIKs.Models;

namespace APIKs.JSONModels {
    public class PrivateRecipe {
        public int RecipeID {get; set;}

        public string Name {get; set;}
        
        public string Description {get; set;}

        public string[] Categories {get; set;}

        public int[] ProductIDs {get; set;}

        public float[] Amounts {get; set;}
    }
}