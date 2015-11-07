using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Restaurant.DomainModel
{
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

        public List<OrderItem> GetItems()
        {
            JArray jItems = (JArray)jsonObject["items"];
            return jItems.Select(jItem =>
            {
                OrderItem newOrderItem = new OrderItem();
                newOrderItem.Id = (int) jItem["Id"];
                newOrderItem.Description = (string) jItem["Description"];
                newOrderItem.Quantity = (int) jItem["Quantity"];
                newOrderItem.Price = (decimal) jItem["Price"];
                return newOrderItem;
            }).ToList();
        }

        public void AddItem(OrderItem orderItem)
        {
            JArray jItems = (JArray)jsonObject["items"];
            JObject jItem = new JObject();
            jItem["Id"] = orderItem.Id;
            jItem["Description"] = orderItem.Description;
            jItem["Quantity"] = orderItem.Quantity;
            jItem["Price"] = orderItem.Price;
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