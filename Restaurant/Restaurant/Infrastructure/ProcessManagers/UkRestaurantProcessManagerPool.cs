using Restaurant.Messages.Events;

namespace Restaurant.Infrastructure.ProcessManagers
{
    public class UkRestaurantProcessManagerPool : IHandle<OrderPlaced>
    {
        private readonly Bus _bus;

        public UkRestaurantProcessManagerPool(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(OrderPlaced message)
        {
            var processManager = new UkRestaurantProcessManager(_bus, message);
        }
    }
}