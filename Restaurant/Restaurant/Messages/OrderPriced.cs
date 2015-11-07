using Restaurant.DomainModel;

namespace Restaurant.Messages
{
    public class OrderPriced : Message
    {
        private readonly OrderDocument _order;
        public OrderPriced(OrderDocument order)
        {
            _order = order;
        }

        public OrderDocument Order { get { return _order; } }
    }
}