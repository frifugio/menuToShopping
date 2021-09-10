using System;
using System.Collections.Generic;
using System.Text;

namespace MenuToShopping.Shared.Models
{
    public class ShoppingList
    {
        public Guid Id { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public int Guests { get; set; }
        public string Notes { get; set; }
    }
}
