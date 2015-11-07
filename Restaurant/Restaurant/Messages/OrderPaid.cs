using Restaurant.DomainModel;

namespace Restaurant.Messages
{
    public class OrderPaid : Message
    {
        private readonly OrderDocument _order;
        public OrderPaid(OrderDocument order)
        {
            _order = order;
        }

        public OrderDocument Order { get { return _order; } }
    }
}