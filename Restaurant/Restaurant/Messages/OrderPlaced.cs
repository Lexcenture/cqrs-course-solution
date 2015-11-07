using System;
using Restaurant.DomainModel;

namespace Restaurant.Messages
{
    public class OrderPlaced : TimeToLiveMessage
    {
        private readonly OrderDocument _order;
        
        public OrderPlaced(OrderDocument order, DateTime expires)
        {
            _order = order;
            Expire = expires;
        }

        public OrderDocument Order { get { return _order; } }
    }
}