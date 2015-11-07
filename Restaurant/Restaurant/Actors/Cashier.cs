using System;
using System.Collections.Generic;
using System.Linq;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Messages;

namespace Restaurant.Actors
{
    public class Cashier : IHandle<OrderPriced>
    {
        private readonly Bus _next;
        private readonly string _paidorder;
        private readonly Dictionary<Guid, OrderPriced> unpaidOrders = new Dictionary<Guid, OrderPriced>();

        public Cashier(Bus next, string paidorder)
        {
            _next = next;
            _paidorder = paidorder;
        }

        public void Handle(OrderPriced orderPricedMessage)
        {
            OrderDocument order = orderPricedMessage.Order;
            unpaidOrders.Add(order.Id, orderPricedMessage);
        }

        public void PayForOrder(Guid orderId)
        {
            OrderPriced orderPriced;
            if (unpaidOrders.TryGetValue(orderId, out orderPriced))
            {
                orderPriced.Order.IsPaid = true;
                unpaidOrders.Remove(orderId);
                _next.Publish(new OrderPaid(orderPriced.Order)
                {
                    MessageId = Guid.NewGuid(),
                    CorrelationId = orderPriced.CorrelationId,
                    CausationId = orderPriced.MessageId
                });
            }
        }

        public List<OrderDocument> GetAvailableOrders()
        {
            return unpaidOrders.Select(x => x.Value.Order).ToList();
        } 

    }
}
