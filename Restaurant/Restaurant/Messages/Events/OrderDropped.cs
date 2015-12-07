using Restaurant.DomainModel;

namespace Restaurant.Messages.Events
{
    public class OrderDropped : Message
    {
        private readonly OrderDocument _order;
        public OrderDropped(OrderDocument order)
        {
            _order = order;
        }

        public OrderDocument Order { get { return _order; } }
    }
}