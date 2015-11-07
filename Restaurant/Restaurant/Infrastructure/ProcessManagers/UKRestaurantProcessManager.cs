using System;
using Restaurant.Actors;
using Restaurant.Messages.Commands;
using Restaurant.Messages.Events;

namespace Restaurant.Infrastructure.ProcessManagers
{
    public class UkRestaurantProcessManager : IHandle<OrderCooked>, IHandle<OrderPriced>, IHandle<OrderPaid>, IHandle<OrderCompleted>, IHandle<RememberEvent<CookOrder>>
    {
        private readonly Bus _bus;
        private int OrderCookedToken;
        private int OrderPricedToken;
        private int OrderPaidToken;
        private int OrderCompletedToken;
        private Guid _correlationId;
        private int ReminderToken;
        private bool isFoodCooked = false;

        public UkRestaurantProcessManager(Bus bus, OrderPlaced message)
        {
            _bus = bus;
            _correlationId = message.CorrelationId;
            OrderCookedToken = _bus.SubscribleToCorrelationId<OrderCooked>(_correlationId, this);
            OrderPricedToken = _bus.SubscribleToCorrelationId<OrderPriced>(_correlationId, this);
            OrderPaidToken = _bus.SubscribleToCorrelationId<OrderPaid>(_correlationId, this);
            OrderCompletedToken = _bus.SubscribleToCorrelationId<OrderCompleted>(_correlationId, this);
            ReminderToken = _bus.SubscribleToCorrelationId<RememberEvent<CookOrder>>(_correlationId, this);

            var cookOrder = new CookOrder(message.Order, DateTime.Now.AddSeconds(1000))
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                CausationId = message.MessageId
            };
            _bus.Publish(new RemindmeCommand<CookOrder>(2, cookOrder)
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                CausationId = message.MessageId
            });
            _bus.Publish(cookOrder);
        }

        public void Handle(RememberEvent<CookOrder> message)
        {
            if (!isFoodCooked)
            {
                _bus.Publish(new RemindmeCommand<CookOrder>(2, message.Message)
                {
                    MessageId = Guid.NewGuid(),
                    CorrelationId = message.CorrelationId,
                    CausationId = message.MessageId
                });
                _bus.Publish(message.Message);                
            }
        }

        public void Handle(OrderCooked message)
        {
            isFoodCooked = true;

            var priceOrder = new PriceOrder(message.Order)
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                CausationId = message.MessageId
            };
            _bus.Publish(priceOrder);
        }

        public void Handle(OrderPriced message)
        {
            var payForOrder = new PayForOrder(message.Order)
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                CausationId = message.MessageId
            };
            _bus.Publish(payForOrder); ;
        }

        public void Handle(OrderPaid message)
        {
            var orderCompleted = new OrderCompleted(message.Order)
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                CausationId = message.MessageId
            };
            _bus.Publish(orderCompleted);
        }

        public void Handle(OrderCompleted message)
        {
            _bus.UnsubscribeFromCorrelationId(_correlationId, OrderCookedToken);
            _bus.UnsubscribeFromCorrelationId(_correlationId, OrderPricedToken);
            _bus.UnsubscribeFromCorrelationId(_correlationId, OrderPaidToken);
            _bus.UnsubscribeFromCorrelationId(_correlationId, OrderCompletedToken);
            _bus.UnsubscribeFromCorrelationId(_correlationId, ReminderToken);
        }

    }
}