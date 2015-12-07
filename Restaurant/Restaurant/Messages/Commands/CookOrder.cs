using System;
using Restaurant.DomainModel;

namespace Restaurant.Messages.Commands
{
    public class CookOrder : TimeToLiveMessage
    {
        private readonly OrderDocument _order;
        public CookOrder(OrderDocument order, DateTime expires)
        {
            _order = order;
            Expire = expires;
        }

        public OrderDocument Order { get { return _order; } }
    }
}
