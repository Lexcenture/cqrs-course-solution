using System;
using System.Collections.Generic;

namespace DocumentEvent
{
    public class Waiter
    {
        private readonly IHandle _handle;

        public Waiter(IHandle handle)
        {
            _handle = handle;
        }

        public void PlaceOrder(List<Item> items, Guid orderId)
        {
            var order = new Order {Id = orderId};

            items.ForEach(x => order.AddItem(x));

            _handle.Handle(order);
        }
    }
}
