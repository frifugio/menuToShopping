using System;
using System.Collections.Generic;

namespace MenuToShopping.Shared.Models
{
    public class Food
    {
        public Guid Id { get; set; }
        public FoodCategoryEnum Category { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }

    public enum FoodCategoryEnum
    {
        NotDefined = 0,
        Appetizer = 1,
        FirstCourse = 2,
        MainCourse = 3,
        Dessert = 4
    }
}