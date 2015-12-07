using System;
using System.Collections.Generic;
using System.Linq;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Messages.Commands;
using Restaurant.Messages.Events;

namespace Restaurant.Actors
{
    public class Cashier : IHandle<PayForOrder>
    {
        private readonly Bus _next;
        private readonly Dictionary<Guid, PayForOrder> unpaidOrders = new Dictionary<Guid, PayForOrder>();

        public Cashier(Bus next)
        {
            _next = next;
        }

        public void Handle(PayForOrder orderPricedMessage)
        {
            OrderDocument order = orderPricedMessage.Order;
            unpaidOrders.Add(order.Id, orderPricedMessage);
        }

        public void PayForOrder(Guid orderId)
        {
            PayForOrder orderPriced;
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
