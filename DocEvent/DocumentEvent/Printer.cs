using System;

namespace DocumentEvent
{
    public class Printer : IHandle
    {
        public void Handle(Order order)
        {
            Console.WriteLine(order.Serialize());
        }
    }
}