using System;
using System.Linq;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Messages.Commands;
using Restaurant.Messages.Events;

namespace Restaurant.Actors
{
    public class AssistantManager : IHandle<PriceOrder>
    {
        private readonly Bus _bus;
        public AssistantManager(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(PriceOrder meal)
        {
            OrderDocument order = meal.Order;
            order.SubTotal = order.GetItems().Sum(item => item.Price * item.Quantity);
            order.Tax = order.SubTotal*0.2m;
            order.Total = order.Tax + order.SubTotal;

            var messageId = Guid.NewGuid();
            _bus.Publish(new OrderPriced(order)
            {
                MessageId = messageId,
                CorrelationId = meal.CorrelationId,
                CausationId = meal.MessageId
            });
        }


    }
}
