using Restaurant.DomainModel;

namespace Restaurant.Messages.Commands
{
    public class PlaceOrder : Message
    {
        private readonly OrderDocument _order;
        public PlaceOrder(OrderDocument order)
        {
            _order = order;
        }

        public OrderDocument Order { get { return _order; } }
    }
}