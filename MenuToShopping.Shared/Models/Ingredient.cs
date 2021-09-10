using System;

namespace MenuToShopping.Shared.Models
{
    public class Ingredient
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int StandardQuantity { get; set; }
    }
}