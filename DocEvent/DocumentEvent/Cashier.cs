using System;
using System.Collections.Generic;

namespace DocumentEvent
{
    public class Cashier : IHandle
    {
        private readonly IHandle _nextHandle;
        private readonly IDictionary<Guid, Order> _orders; 

        public Cashier(IHandle nextHandle)
        {
            _nextHandle = nextHandle;
            _orders = new Dictionary<Guid, Order>();
        }

        public void PayFor(Guid orderId)
        {
            Order orderToPayFor = null;
            if (_orders.TryGetValue(orderId, out orderToPayFor))
            {
                orderToPayFor.Paid = true;
                _nextHandle.Handle(orderToPayFor);
                _orders.Remove(orderId);
            }
        }
        public void Handle(Order order)
        {
            _orders.Add(order.Id, order);
        }
    }
}