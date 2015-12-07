using System;
using System.Threading;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Dispatchers;
using Restaurant.Messages.Commands;
using Restaurant.Messages.Events;

namespace Restaurant.Actors
{
    public class Cook : IHandle<CookOrder>
    {
        private readonly Bus _bus;
        private readonly int _sleep;

        public Cook(Bus bus, int sleep)
        {
            _bus = bus;
            _sleep = sleep;
        }

        public void Handle(CookOrder newOrder)
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