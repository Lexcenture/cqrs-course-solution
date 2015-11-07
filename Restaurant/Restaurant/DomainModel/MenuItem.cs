using System.Collections.Generic;

namespace Restaurant.DomainModel
{
    public class MenuItem
    {
        public int Id;
        public string Description;
        public List<string> Ingredients = new List<string>();
        public decimal Price;
    }
}