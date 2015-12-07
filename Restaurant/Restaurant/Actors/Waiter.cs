using System;
using System.Collections.Generic;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Messages.Events;

namespace Restaurant.Actors
{
    public class Waiter
    {
        private readonly Bus _bus;
        private readonly Menu _menu;

        public Waiter(Bus bus, Menu menu)
        {
            _bus = bus;
            _menu = menu;
        }

        public void PlaceOrder(IList<Tuple<int,int>> items, Guid orderId)
        {
            OrderDocument newOrder = new OrderDocument {Id = orderId};
            foreach (var item in items)
            {
                MenuItem mItem = _menu.GetItem(item.Item1);
                newOrder.AddItem(new OrderItem {Id = item.Item1, Description = mItem.Description, Quantity = item.Item2, Price = mItem.Price});
            }
            
            _bus.Publish(new OrderPlaced(newOrder) { MessageId = Guid.NewGuid(), CorrelationId = orderId, CausationId = Guid.Empty });
        }
    }
}