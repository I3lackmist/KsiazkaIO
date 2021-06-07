using System;

namespace APIKs.Models {
    public class PrivateProduct {

        public int ProductID {get; set;}

        public string Name {get; set;}
        
        public float Carbohydrates {get; set;}

        public float Proteins {get; set;}

        public float Fats {get; set;}

        public float Kcal {get; set;}

        public string Note {get; set;} = "";
    }
}
