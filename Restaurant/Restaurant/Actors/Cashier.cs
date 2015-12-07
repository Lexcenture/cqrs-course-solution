using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Dispatchers;
using Restaurant.Messages;
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
            _next.Publish(new OrderReadyForPayment(orderPricedMessage.Order)
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = orderPricedMessage.CorrelationId,
                CausationId = orderPricedMessage.CausationId
            });
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

    public class RememberEvent<T> : Message where T : Message
    {
        private readonly T _message;

        public T Message
        {
            get { return _message; }
        }

        public RememberEvent(T message)
        {
            _message = message;
        }        
    }

    public class RemindmeCommand<T> : Message where T : Message
    {
        private readonly T _message;
        public int Seconds { get; private set; }

        public T Message
        {
            get { return _message; }
        }

        public RemindmeCommand(int seconds, T message)
        {
            _message = message;
            Seconds = seconds;
        }

    }
}
