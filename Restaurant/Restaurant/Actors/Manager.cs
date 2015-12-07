using System;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Messages.Events;

namespace Restaurant.Actors
{
    public class Manager : IHandle<OrderCompleted>
    {
        private int count = 0;
        private decimal total = 0m;

        public void Handle(OrderCompleted orderCompleted)
        {
            OrderDocument order = orderCompleted.Order;
            count++;
            total += order.Total;
        }

        public int Count { get { return count; } }
        public decimal Total { get { return total; } }
    }    
}