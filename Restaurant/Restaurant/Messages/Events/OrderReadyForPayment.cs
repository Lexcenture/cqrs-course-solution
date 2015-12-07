using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.DomainModel;

namespace Restaurant.Messages.Events
{
    public class OrderReadyForPayment : Message
    {
        private readonly OrderDocument _order;
        public OrderReadyForPayment(OrderDocument order)
        {
            _order = order;
        }

        public OrderDocument Order { get { return _order; } }
    }
}
