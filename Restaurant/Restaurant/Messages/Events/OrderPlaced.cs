using Restaurant.DomainModel;

namespace Restaurant.Messages.Events
{
    public class OrderPlaced : Message
    {
        private readonly OrderDocument _order;
        
        public OrderPlaced(OrderDocument order)
        {
            _order = order;
        }

        public OrderDocument Order { get { return _order; } }
    }
}