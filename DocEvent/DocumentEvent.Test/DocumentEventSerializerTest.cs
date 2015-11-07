using System.Linq;
using NUnit.Framework;

namespace DocumentEvent.Test
{
    public class DocumentEventSerializerTest
    {

        [Test]
        public void Adding_an_item_to_simple_json_should_add_item()
        {
            var json = @"{}";

            var order = Order.Deserialize(json);

            var expectedItem = new Item(1, "Pizza", 2, 2.95m);

            order.AddItem(expectedItem);

            var actualItem = order.GetItems().SingleOrDefault();

            Assert.AreEqual(expectedItem, actualItem);
        }

        [Test]
        public void Creating_an_order_should_give_the_right_json()
        {
            var order = new Order();
            order.Total = 100;
            order.Tax = 20;
            order.SubTotal = 120;
            order.AddIngrediants("Salt");
            order.AddIngrediants("Oil");
            order.AddIngrediants("Suger");
            order.AddItem(new Item(1, "Pizza", 2, 2.95m));


            const string expectedJsonSerializedJson = @"{
  ""Items"": [
    {
      ""Id"": 1,
      ""Description"": ""Pizza"",
      ""Quantity"": 2,
      ""Price"": 2.95
    }
  ],
  ""Id"": """",
  ""Total"": 100.0,
  ""Tax"": 20.0,
  ""SubTotal"": 120.0,
  ""Paid"": false,
  ""Ingrediants"": [
    ""Salt"",
    ""Oil"",
    ""Suger""
  ]
}";
            var actualJsonSerializedJson = order.Serialize();

            Assert.AreEqual(expectedJsonSerializedJson, actualJsonSerializedJson);
        }
    }
}
