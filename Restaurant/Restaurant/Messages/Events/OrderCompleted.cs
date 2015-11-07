using Restaurant.DomainModel;

namespace Restaurant.Messages.Events
{
    public class OrderCompleted : Message
    {
        private readonly OrderDocument _order;
        public OrderCompleted(OrderDocument order)
        {
            _order = order;
        }

        public OrderDocument Order { get { return _order; } }
    }
}