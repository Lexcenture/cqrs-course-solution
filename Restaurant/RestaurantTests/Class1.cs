using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Restaurant;

namespace RestaurantTests
{
    [TestFixture]
    public class OrderTests
    {


        [Test]
        public void SerializeDeserialize()
        {
            // arrange
            OrderDocument order = new OrderDocument();
            Item item = new Item();
            item.Id = 3;
            item.Description = "Soup of the day";
            item.Price = 6.95m;
            item.Quantity = 2;
            order.AddItem(item);

            // act
            string json = order.Serialize();
            OrderDocument rehydratedOrder = new OrderDocument(json);

            // assert

            Assert.That(rehydratedOrder.GetItems().Count, Is.EqualTo(1));
            Item rehydratedItem = rehydratedOrder.GetItems()[0];
            Assert.That(rehydratedItem.Id, Is.EqualTo(3));
            Assert.That(rehydratedItem.Description, Is.EqualTo("Soup of the day"));
            Assert.That(rehydratedItem.Price, Is.EqualTo(6.95m));
            Assert.That(rehydratedItem.Quantity, Is.EqualTo(2));

        }


        [Test]
        public void DeserializeSerialize()
        {
            // arrange
            string jsonOrder = "{\r\n  \"items\": [],\r\n  \"ingredients\": [\r\n    \"salt\",\r\n    \"pepper\"\r\n  ],\r\n  \"vip\": true\r\n}";

            // act
            OrderDocument order = new OrderDocument(jsonOrder);
            string reserializedOrder = order.Serialize();

            //  Assert
            Assert.That(reserializedOrder, Is.EqualTo(jsonOrder));

        }


    }
}
