using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Restaurant.Infrastructure;
using Restaurant.Messages.Events;

namespace Restaurant.Actors
{
    public class OrderReadModel : IStartable, IHandle<OrderPlaced>, IHandle<OrderCooked>, IHandle<OrderPriced>, IHandle<OrderPaid>, IHandle<OrderCompleted>
    {
        private readonly Dictionary<Guid, OrderStatus> _orderStatuses = new Dictionary<Guid, OrderStatus>(); 

        private enum OrderStatus
        {
            Placed,
            Cooked,
            Priced,
            Paid,
            Completed
        };

        public void Handle(OrderPlaced message)
        {
            _orderStatuses.Add(message.Order.Id, OrderStatus.Placed);
        }

        public void Handle(OrderCooked message)
        {
            _orderStatuses[message.Order.Id] = OrderStatus.Cooked;
        }

        public void Handle(OrderPriced message)
        {
            _orderStatuses[message.Order.Id] = OrderStatus.Priced;
        }

        public void Handle(OrderPaid message)
        {
            _orderStatuses[message.Order.Id] = OrderStatus.Paid;
        }

        public void Start()
        {
            var outputThread = new Thread(OutputOrderState) {IsBackground = true};
            outputThread.Start();
        }

        private void OutputOrderState()
        {
            while (true)
            {
                Thread.Sleep(1000);
                var output = String.Format("Order Status as at {0}:", DateTime.UtcNow);
                var result = _orderStatuses.Aggregate(output, (s, pair) => s + String.Format("    Order Id: {0}, Status: {1}", pair.Key, pair.Value) + Environment.NewLine);
                result += Environment.NewLine;
                Console.WriteLine(result);
            }
        }

        public void Handle(OrderCompleted message)
        {
            _orderStatuses[message.Order.Id] = OrderStatus.Completed;
        }
    }
}