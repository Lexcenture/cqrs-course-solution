using Restaurant.DomainModel;

namespace Restaurant.Messages.Commands
{
    public class PriceOrder : Message
    {
        private readonly OrderDocument _order;
        public PriceOrder(OrderDocument order)
        {
            _order = order;
        }

        public OrderDocument Order { get { return _order; } }
    }
}