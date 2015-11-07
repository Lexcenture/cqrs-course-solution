using System.Collections.Generic;

namespace Restaurant.DomainModel
{
    public class Menu
    {
        private readonly Dictionary<int,MenuItem> mnu = new Dictionary<int, MenuItem>();

        public Menu()
        {
            MenuItem menuitem1 = new MenuItem() {Id = 1, Description = "Soup", Ingredients = new List<string>() { "Salt", "Stock", "Chicken" }, Price = 4.50m};
            MenuItem menuitem2 = new MenuItem() {Id = 2, Description = "Vegetable Soup", Ingredients = new List<string>() { "Salt", "Stock", "Carrot" }, Price = 2.05m};
            MenuItem menuitem3 = new MenuItem() {Id = 3, Description = "Salad", Ingredients = new List<string>() { "Leaves", "Dressing", "Cucumber" }, Price = 6.75m};
            MenuItem menuitem4 = new MenuItem() {Id = 4, Description = "Pizza", Ingredients = new List<string>() { "Pizza base", "Tomato Sauce", "cheese" }, Price = 8.90m};
            mnu.Add(menuitem1.Id, menuitem1);
            mnu.Add(menuitem2.Id, menuitem2);
            mnu.Add(menuitem3.Id, menuitem3);
            mnu.Add(menuitem4.Id, menuitem4);
        }

        public MenuItem GetItem(int id)
        {
            return mnu[id];
        }
        
    }
}
