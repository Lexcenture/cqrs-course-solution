using System;
using NUnit.Framework;
using Restaurant;
using Restaurant.DomainModel;

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
            OrderItem orderItem = new OrderItem();
            orderItem.Id = 3;
            orderItem.Description = "Soup of the day";
            orderItem.Price = 6.95m;
            orderItem.Quantity = 2;
            order.AddItem(orderItem);

            // act
            string json = order.Serialize();
            OrderDocument rehydratedOrder = new OrderDocument(json);

            // assert

            Assert.That(rehydratedOrder.GetItems().Count, Is.EqualTo(1));
            OrderItem rehydratedOrderItem = rehydratedOrder.GetItems()[0];
            Assert.That(rehydratedOrderItem.Id, Is.EqualTo(3));
            Assert.That(rehydratedOrderItem.Description, Is.EqualTo("Soup of the day"));
            Assert.That(rehydratedOrderItem.Price, Is.EqualTo(6.95m));
            Assert.That(rehydratedOrderItem.Quantity, Is.EqualTo(2));

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
