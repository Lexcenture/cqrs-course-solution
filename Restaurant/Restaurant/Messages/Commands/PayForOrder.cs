using Restaurant.DomainModel;

namespace Restaurant.Messages.Commands
{
    public class PayForOrder : Message
    {
        private readonly OrderDocument _order;
        public PayForOrder(OrderDocument order)
        {
            _order = order;
        }

        public OrderDocument Order { get { return _order; } }
    }
}