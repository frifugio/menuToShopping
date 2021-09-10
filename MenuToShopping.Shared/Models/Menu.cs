using System;
using System.Collections.Generic;

namespace MenuToShopping.Shared.Models
{
    public class Menu
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Food> Foods { get; set; }
    }
}
