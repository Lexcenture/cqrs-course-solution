using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    class Cashier : IHandleOrder
    {
        private readonly IHandleOrder _next;
        private readonly Dictionary<Guid,OrderDocument> unpaidOrders = new Dictionary<Guid, OrderDocument>();

        public Cashier(IHandleOrder next)
        {
            _next = next;
        }

        public void Handle(OrderDocument order)
        {
            unpaidOrders.Add(order.Id, order);
        }

        public void PayForOrder(Guid orderId)
        {
            OrderDocument order;
            if (unpaidOrders.TryGetValue(orderId, out order))
            {
                order.IsPaid = true;
                unpaidOrders.Remove(orderId);
                _next.Handle(order);
            }
        }

        public List<OrderDocument> GetAvailableOrders()
        {
            return unpaidOrders.Select(x => x.Value).ToList();
        } 

    }
}
