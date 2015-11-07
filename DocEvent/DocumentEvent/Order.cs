using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DocumentEvent
{
    public class Order
    {
        private readonly JObject _orderJson;

        public static Order Deserialize(string json)
        {
            return new Order(json);
        }

        public string Serialize()
        {
            return _orderJson.ToString();
        }

        public Order() : this("{}")
        {
            
        }

        private Order(string json)
        {
            _orderJson = JObject.Parse(json);
            Initialize();
        }

        private void Initialize()
        {
            if (_orderJson["Items"] == null)
            {
                _orderJson["Items"] = new JArray();
            }
            
            if (_orderJson["Id"] == null)
            {
                _orderJson["Id"] = string.Empty;
            }

            if (_orderJson["Total"] == null)
            {
                _orderJson["Total"] = 0.0m;
            }

            if (_orderJson["Tax"] == null)
            {
                _orderJson["Tax"] = 0.0m;
            }

            if (_orderJson["SubTotal"] == null)
            {
                _orderJson["SubTotal"] = 0.0m;
            }

            if (_orderJson["Paid"] == null)
            {
                _orderJson["Paid"] = false;
            }

            if (_orderJson["Ingrediants"] == null)
            {
                _orderJson["Ingrediants"] = new JArray();
            }
        }

        public decimal Total
        {
            get { return Convert.ToDecimal(_orderJson["Total"]); }
            set { _orderJson["Total"] = value; }
        }

        public decimal Tax
        {
            get { return Convert.ToDecimal(_orderJson["Tax"]); }
            set { _orderJson["Tax"] = value; }
        }
        public decimal SubTotal
        {
            get { return Convert.ToDecimal(_orderJson["SubTotal"]); }
            set { _orderJson["SubTotal"] = value; }
        } 
        
        public bool Paid
        {
            get { return Convert.ToBoolean(_orderJson["Paid"]); }
            set { _orderJson["Paid"] = value; }
        }

        public Guid Id
        {
            get { return _orderJson["Id"].ToString() == string.Empty ? Guid.Empty : new Guid(_orderJson["Id"].ToString()); }
            set { _orderJson["Id"] = value; }
        }

        public void AddItem(Item item)
        {
            var jsonItems = (JArray)_orderJson["Items"];

            var jsonItem = new JObject();
            jsonItem["Id"] = item.Id;
            jsonItem["Description"] = item.Description;
            jsonItem["Quantity"] = item.Quantity;
            jsonItem["Price"] = item.Price;

            jsonItems.Add(jsonItem);
        }

      

        public IEnumerable<Item> GetItems()
        {
            var jsonItems = (JArray)_orderJson["Items"];

            return
                jsonItems.Select(
                    x =>
                        new Item(Convert.ToInt32(x["Id"]), x["Description"].ToString(), Convert.ToInt32(x["Quantity"]),
                            Convert.ToDecimal(x["Price"])));
        }

        public void AddIngrediants(string ingrediant)
        {
            var jsonItems = (JArray)_orderJson["Ingrediants"];
            jsonItems.Add(ingrediant);
        }

        public IEnumerable<string> GetIngrediants()
        {
            var jsonItems = (JArray)_orderJson["Ingrediants"];

            return jsonItems.Select(x => x.ToString());
        }
    }
}