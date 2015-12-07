using System;
using Restaurant.Actors;
using Restaurant.Infrastructure.Dispatchers;
using Restaurant.Messages.Commands;
using Restaurant.Messages.Events;

namespace Restaurant.Infrastructure.ProcessManagers
{
    public class UkRestaurantProcessManager : IHandle<OrderCooked>, IHandle<OrderPriced>, IHandle<OrderPaid>, IHandle<OrderCompleted>, IHandle<RememberEvent<CookOrder>>, IHandle<RememberEvent<OrderPlaced>>
    {
        private readonly Bus _bus;
        private readonly int OrderCookedToken;
        private readonly int OrderPricedToken;
        private readonly int OrderPaidToken;
        private readonly int OrderCompletedToken;
        private readonly Guid _correlationId;
        private readonly int ReminderToken;
        private readonly int OrderScrewedReminder;
        
        private bool isFoodCooked = false;
        private bool isCompleted = false;

        private Guid _expectedNextCausationId;

        public UkRestaurantProcessManager(Bus bus, OrderPlaced message)
        {
            _bus = bus;
            _correlationId = message.CorrelationId;

            OrderCookedToken = _bus.SubscribleToCorrelationId<OrderCooked>(_correlationId, this);
            OrderPricedToken = _bus.SubscribleToCorrelationId<OrderPriced>(_correlationId, this);
            OrderPaidToken = _bus.SubscribleToCorrelationId<OrderPaid>(_correlationId, this);
            OrderCompletedToken = _bus.SubscribleToCorrelationId<OrderCompleted>(_correlationId, this);
            ReminderToken = _bus.SubscribleToCorrelationId<RememberEvent<CookOrder>>(_correlationId, this);
            OrderScrewedReminder = _bus.SubscribleToCorrelationId<RememberEvent<OrderPlaced>>(_correlationId, this);

            var cookOrder = new CookOrder(message.Order, DateTime.Now.AddSeconds(1000))
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = message.CorrelationId,
                CausationId = message.MessageId
            };

            _expectedNextCausationId = cookOrder.MessageId;
            //_bus.Publish(new RemindmeCommand<CookOrder>(5, cookOrder)
            //{
            //    MessageId = Guid.NewGuid(),
            //    CorrelationId = message.CorrelationId,
            //    CausationId = message.MessageId
            //});

            _bus.Publish(new RemindmeCommand<OrderPlaced>(5, message)
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
            if (_expectedNextCausationId == message.CausationId)
            {
                if (isFoodCooked)
                {
                    Console.WriteLine("Food was cooked twice: {0}", message.Order.Id);
                }

                isFoodCooked = true;

                var priceOrder = new PriceOrder(message.Order)
                {
                    MessageId = Guid.NewGuid(),
                    CorrelationId = message.CorrelationId,
                    CausationId = message.MessageId
                };
                _expectedNextCausationId = priceOrder.MessageId;
                _bus.Publish(priceOrder);    
            }
            else Console.WriteLine("Received Evil Message: Order Cooked {0}", message.Order.Id);
        }

        public void Handle(OrderPriced message)
        {
            if (_expectedNextCausationId == message.CausationId)
            {
                var payForOrder = new PayForOrder(message.Order)
                {
                    MessageId = Guid.NewGuid(),
                    CorrelationId = message.CorrelationId,
                    CausationId = message.MessageId
                };
                _expectedNextCausationId = payForOrder.MessageId;
                _bus.Publish(payForOrder);     
            }
            else Console.WriteLine("Received Evil Message: Order Priced {0}", message.Order.Id);
        }

        public void Handle(OrderPaid message)
        {
            if (_expectedNextCausationId == message.CausationId)
            {
                var orderCompleted = new OrderCompleted(message.Order)
                {
                    MessageId = Guid.NewGuid(),
                    CorrelationId = message.CorrelationId,
                    CausationId = message.MessageId
                };
                _expectedNextCausationId = orderCompleted.MessageId;
                _bus.Publish(orderCompleted);    
            }
            else Console.WriteLine("Received Evil Message: Order Paid {0}", message.Order.Id);
        }

        public void Handle(OrderCompleted message)
        {
            if (_expectedNextCausationId == message.MessageId)
            {
                isCompleted = true;
                _bus.UnsubscribeFromCorrelationId(_correlationId, OrderCookedToken);
                _bus.UnsubscribeFromCorrelationId(_correlationId, OrderPricedToken);
                _bus.UnsubscribeFromCorrelationId(_correlationId, OrderPaidToken);
                _bus.UnsubscribeFromCorrelationId(_correlationId, OrderCompletedToken);
                _bus.UnsubscribeFromCorrelationId(_correlationId, ReminderToken);
                _bus.UnsubscribeFromCorrelationId(_correlationId, OrderScrewedReminder);    
            }
            else Console.WriteLine("Received Evil Message: Order Completed {0}", message.Order.Id);
        }

        public void Handle(RememberEvent<OrderPlaced> message)
        {
            if (!isCompleted)
            {
                _bus.Publish(new HumanInterventionRequired(message.Message)
                {
                    MessageId =  Guid.NewGuid(),
                    CorrelationId = message.CorrelationId,
                    CausationId = message.CausationId
                });
            }
        }
    }
}