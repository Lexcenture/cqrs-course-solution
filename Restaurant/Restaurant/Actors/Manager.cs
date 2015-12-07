using System;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Messages.Events;

namespace Restaurant.Actors
{
    public class Manager : IHandle<OrderCompleted>, IHandle<HumanInterventionRequired>
    {
        private int count = 0;
        private decimal total = 0m;

        public void Handle(OrderCompleted orderCompleted)
        {
            OrderDocument order = orderCompleted.Order;
            //Console.WriteLine("Order completed: {0}", orderCompleted.CorrelationId);
            count++;
            total += order.Total;
        }

        public int Count { get { return count; } }
        public decimal Total { get { return total; } }

        public void Handle(HumanInterventionRequired message)
        {
            Console.WriteLine("Order was screwed: {0}", message.CorrelationId);
        }
    }    
}