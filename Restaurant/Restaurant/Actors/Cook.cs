using System;
using System.Threading;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Messages;

namespace Restaurant.Actors
{
    public class Cook : IHandle<OrderPlaced>
    {
        private readonly Bus _bus;
        private readonly string _publishOn;
        private readonly int _sleep;

        public Cook(Bus bus, string publishOn, int sleep)
        {
            _bus = bus;
            _publishOn = publishOn;
            _sleep = sleep;
        }

        public void Handle(OrderPlaced newOrder)
        {
            OrderDocument order = newOrder.Order;
            Thread.Sleep(_sleep);
            order.AddIngredient("Salt");
            order.AddIngredient("Pepper");
            order.AddIngredient("Tomatoes");
            order.AddIngredient("Chorizo");
            order.AddIngredient("Sugar");
            _bus.Publish(new OrderCooked(order)
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = newOrder.CorrelationId,
                CausationId = newOrder.MessageId
            });
        }
    }
}
