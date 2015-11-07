using System;
using System.Collections.Generic;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Messages;

namespace Restaurant.Actors
{
    public class Waiter
    {
        private readonly Bus _bus;
        private readonly string _publishOn;
        private readonly Menu _menu;

        public Waiter(Bus bus, string publishOn, Menu menu)
        {
            _bus = bus;
            _publishOn = publishOn;
            _menu = menu;
        }

        public void PlaceOrder(IList<Tuple<int,int>> items, Guid orderId)
        {
            OrderDocument newOrder = new OrderDocument();
            newOrder.Id = orderId;
            foreach (var item in items)
            {
                MenuItem mItem = _menu.GetItem(item.Item1);
                newOrder.AddItem(new OrderItem() {Id = item.Item1, Description = mItem.Description, Quantity = item.Item2, Price = mItem.Price});
            }

            var messageId = Guid.NewGuid();
            var causationid = Guid.Empty;
            _bus.Publish(new OrderPlaced(newOrder, DateTime.Now.AddSeconds(50)) { MessageId = messageId, CorrelationId = orderId, CausationId = causationid });
        }
    }
}
