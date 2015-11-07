using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Restaurant
{
    public class Item
    {
        //public Item(int id, string descriptoin, int quantity, decimal price)
        //{
        //    Id = id;
        //    Description = descriptoin;
        //    Quantity = quantity;
        //    Price = price;
        //}
        public int Id;
        public string Description;
        public int Quantity;
        public decimal Price;
    }

    public class OrderDocument
    {
        private readonly JObject jsonObject;

        public OrderDocument()
        {
            jsonObject = new JObject();
            jsonObject["items"] = new JArray();
            jsonObject["Paid"] = false;
            jsonObject["Id"] = Guid.NewGuid();
            jsonObject["SubTotal"] = 0m;
            jsonObject["Tax"] = 0m;
            jsonObject["Total"] = 0m;
            jsonObject["MillisecondsToCook"] = 0;
            jsonObject["Ingredients"] = new JArray();
        }
        public OrderDocument(string json)
        {
            jsonObject = JObject.Parse(json);
        }

        public bool IsPaid
        {
            get { return (bool)jsonObject["Paid"]; }
            set { jsonObject["Paid"] = value; }
        }

        public Guid Id
        {
            get { return (Guid)jsonObject["Id"]; }
            set { jsonObject["Id"] = value; }
        }

        public decimal SubTotal
        {
            get { return (decimal)jsonObject["SubTotal"]; }
            set { jsonObject["SubTotal"] = value; }
        }

        public decimal Total
        {
            get { return (decimal)jsonObject["Total"]; }
            set { jsonObject["Total"] = value; }
        }

        public decimal Tax
        {
            get { return (decimal)jsonObject["Tax"]; }
            set { jsonObject["Tax"] = value; }
        }

        public List<Item> GetItems()
        {
            JArray jItems = (JArray)jsonObject["items"];
            return jItems.Select(jItem =>
            {
                Item newItem = new Item();
                newItem.Id = (int) jItem["Id"];
                newItem.Description = (string) jItem["Description"];
                newItem.Quantity = (int) jItem["Quantity"];
                newItem.Price = (decimal) jItem["Price"];
                return newItem;
            }).ToList();
        }

        public void AddItem(Item item)
        {
            JArray jItems = (JArray)jsonObject["items"];
            JObject jItem = new JObject();
            jItem["Id"] = item.Id;
            jItem["Description"] = item.Description;
            jItem["Quantity"] = item.Quantity;
            jItem["Price"] = item.Price;
            jItems.Add(jItem);
        }

        public List<string> GetIngredients()
        {
            JArray jIngs = (JArray)jsonObject["Ingredients"];
            return jIngs.Select(jIng => jIng.ToString()).ToList();
        }

        public void AddIngredient(string ingredient)
        {
            JArray jIngs = (JArray) jsonObject["Ingredients"];
            jIngs.Add(ingredient);
        }

        public string Serialize()
        {
            return jsonObject.ToString();
        }
    }

    
}
