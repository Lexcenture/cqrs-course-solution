using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    public class Printer : IHandleOrder
    {        
        public void Handle(OrderDocument order)
        {
            Console.WriteLine(order.Serialize());
        }
    }

    public class PassthroughPrinter : IHandleOrder
    {
        private readonly IHandleOrder _next;
        public PassthroughPrinter(IHandleOrder next)
        {
            _next = next;
        }
        public void Handle(OrderDocument order)
        {
            Console.WriteLine(order.Serialize());
            _next.Handle(order);
        }

    }
}
