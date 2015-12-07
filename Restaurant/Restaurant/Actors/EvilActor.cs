using System;
using System.Threading.Tasks;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Dispatchers;
using Restaurant.Messages;
using Restaurant.Messages.Events;

namespace Restaurant.Actors
{
    public class EvilActor : IHandle<OrderPlaced>
    {
        private readonly Random _random;
        private readonly Bus _bus;

        public EvilActor(Bus bus)
        {
            _bus = bus;
            _random = new Random();
        }

        public void Handle(OrderPlaced message)
        {
            var randomIndex = _random.Next(0, 9);

            var randomMessage = CreateMessage(randomIndex, message);
            if (randomMessage != null)
            {
                randomMessage.MessageId = Guid.NewGuid();
                randomMessage.CausationId = message.CausationId;
                randomMessage.CorrelationId = message.CorrelationId;

                var randomDelay = _random.Next(0, 10000);
                Task.Delay(randomDelay).ContinueWith(i => _bus.Publish(randomMessage));
            }
        }

        private static Message CreateMessage(int index, OrderPlaced orderPlaced)
        {
            switch (index)
            {   
                case 0:
                    Console.WriteLine("Evil Actor: Sending Order Cooked {0}", orderPlaced.Order.Id);
                    return new OrderCooked(orderPlaced.Order);
                case 1:
                    Console.WriteLine("Evil Actor: Sending Order Dropped {0}", orderPlaced.Order.Id);
                    return new OrderDropped(orderPlaced.Order);
                case 2:
                    Console.WriteLine("Evil Actor: Sending Order Paid {0}", orderPlaced.Order.Id);
                    return new OrderPaid(orderPlaced.Order);
                case 3:
                    Console.WriteLine("Evil Actor: Sending Order Completed {0}", orderPlaced.Order.Id);
                    return new OrderCompleted(orderPlaced.Order);
                default:
                    return null;
            }
        }
    }
}