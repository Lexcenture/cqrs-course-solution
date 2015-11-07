using System;
using Restaurant.Infrastructure;
using Restaurant.Messages.Events;

namespace Restaurant.Actors
{
    public class OrderStatusBoard : IHandle<OrderPlaced>, IHandle<OrderCooked>, IHandle<OrderPriced>, IHandle<OrderPaid>
    {
        public void Handle(OrderPlaced message)
        {
            Console.WriteLine("Order {0} placed.", message.Order.Id);
        }

        public void Handle(OrderCooked message)
        {
            Console.WriteLine("Order {0} cooked.", message.Order.Id);
        }

        public void Handle(OrderPriced message)
        {
            Console.WriteLine("Order {0} priced.", message.Order.Id);
        }

        public void Handle(OrderPaid message)
        {
            Console.WriteLine("Order {0} paid.", message.Order.Id);
        }
    }
}