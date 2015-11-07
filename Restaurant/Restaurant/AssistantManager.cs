using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    public class AssistantManager : IHandleOrder
    {
        private readonly IHandleOrder _next;
        public AssistantManager(IHandleOrder next)
        {
            _next = next;
        }

        public void Handle(OrderDocument order)
        {
            order.SubTotal = order.GetItems().Sum(item => item.Price * item.Quantity);
            order.Tax = order.SubTotal*0.2m;
            order.Total = order.Tax + order.SubTotal;
            _next.Handle(order);
        }
    }
}
