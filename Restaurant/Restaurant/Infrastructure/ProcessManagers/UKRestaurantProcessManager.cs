using Restaurant.Messages.Commands;
using Restaurant.Messages.Events;

namespace Restaurant.Infrastructure.ProcessManagers
{
    public class UkRestaurantProcessManager : IHandle<OrderCooked>, IHandle<OrderPriced>, IHandle<OrderPaid>, IHandle<OrderCompleted>
    {
        private readonly Bus _bus;

        public UkRestaurantProcessManager(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(OrderCooked message)
        {
            var priceOrder = new PriceOrder(message.Order);
            _bus.Publish(priceOrder);
        }

        public void Handle(OrderPriced message)
        {
            var payForOrder = new PayForOrder(message.Order);
            _bus.Publish(payForOrder); ;
        }

        public void Handle(OrderPaid message)
        {
            var orderCompleted = new OrderCompleted(message.Order);
            _bus.Publish(orderCompleted);
        }

        public void Handle(OrderCompleted message)
        {
            // Unsubscribe from Message Bus
        }
    }
}