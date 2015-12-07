using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Dispatchers;
using Restaurant.Messages.Events;

namespace Restaurant.Actors
{
    public class Customer : IHandle<OrderReadyForPayment>
    {
        private readonly Bus _bus;
        private readonly Waiter _water;
        private readonly Cashier _cashier;

        public Customer(Bus bus, Waiter water, Cashier cashier)
        {
            _bus = bus;
            _water = water;
            _cashier = cashier;
        }

        public Guid PlaceOrder()
        {
            List<Tuple<int, int>> items = new List<Tuple<int, int>>();
            items.Add(new Tuple<int, int>(1, 2));
            items.Add(new Tuple<int, int>(2, 1));
            items.Add(new Tuple<int, int>(3, 2));
            items.Add(new Tuple<int, int>(4, 2));
            Guid orderId = Guid.NewGuid();
            _bus.SubscribleToCorrelationId(orderId, this);
            _water.PlaceOrder(items, orderId);
            return orderId;
        }

        public void Handle(OrderReadyForPayment message)
        {
            _cashier.PayForOrder(message.Order.Id);
        }
    }
}
