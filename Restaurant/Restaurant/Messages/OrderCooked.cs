using Restaurant.DomainModel;

namespace Restaurant.Messages
{
    public class OrderCooked : Message
    {
        private readonly OrderDocument _order;
        public OrderCooked(OrderDocument order)
        {
            _order = order;
        }

        public OrderDocument Order { get { return _order; } }
    }
}