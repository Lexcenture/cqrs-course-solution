using System;
using Restaurant.Actors;
using Restaurant.Infrastructure.Dispatchers;
using Restaurant.Messages.Commands;
using Restaurant.Messages.Events;

namespace Restaurant.Infrastructure.ProcessManagers
{
    public class UkRestaurantDodgyProcessManager : IHandle<OrderCooked>, IHandle<OrderPriced>, IHandle<OrderPaid>, IHandle<OrderCompleted>, IHandle<RememberEvent<CookOrder>>
    {
        private readonly Bus _bus;
        private readonly int _orderCookedToken;
        private readonly int _orderPricedToken;
        private readonly int _orderPaidToken;
        private readonly int _orderCompletedToken;
        private readonly Guid _correlationId;
        private readonly int _reminderToken;
        private bool _isFoodCooked;

        public UkRestaurantDodgyProcessManager(Bus bus, OrderPlaced message)
        {
            _bus = bus;
            _correlationId = message.CorrelationId;
            _orderCookedToken = _bus.SubscribleToCorrelationId<OrderCooked>(_correlationId, this);
            _orderPricedToken = _bus.SubscribleToCorrelationId<OrderPriced>(_correlationId, this);
            _orderPaidToken = _bus.SubscribleToCorrelationId<OrderPaid>(_correlationId, this);
            _orderCompletedToken = _bus.SubscribleToCorrelationId<OrderCompleted>(_correlationId, this);
            _reminderToken = _bus.SubscribleToCorrelationId<RememberEvent<CookOrder>>(_correlationId, this);

            var priceOrder = new PriceOrder(message.Order)
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                CausationId = message.MessageId
            };
            _bus.Publish(priceOrder);
        }

        public void Handle(RememberEvent<CookOrder> message)
        {
            if (!_isFoodCooked)
            {
                _bus.Publish(new RemindmeCommand<CookOrder>(5, message.Message)
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
            if (_isFoodCooked)
            {
                Console.WriteLine("Food was cooked twice for dodgy customer: {0}", message.Order.Id);
            }

            _isFoodCooked = true;
            var orderCompleted = new OrderCompleted(message.Order)
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                CausationId = message.MessageId
            };
            _bus.Publish(orderCompleted);

        }

        public void Handle(OrderPriced message)
        {
            var payForOrder = new PayForOrder(message.Order)
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                CausationId = message.MessageId
            };
            _bus.Publish(payForOrder);
        }

        public void Handle(OrderPaid message)
        {
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

        public void Handle(OrderCompleted message)
        {
            _bus.UnsubscribeFromCorrelationId(_correlationId, _orderCookedToken);
            _bus.UnsubscribeFromCorrelationId(_correlationId, _orderPricedToken);
            _bus.UnsubscribeFromCorrelationId(_correlationId, _orderPaidToken);
            _bus.UnsubscribeFromCorrelationId(_correlationId, _orderCompletedToken);
            _bus.UnsubscribeFromCorrelationId(_correlationId, _reminderToken);
        }

    }
}