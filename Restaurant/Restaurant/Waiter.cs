using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    class Waiter
    {
        private readonly IHandleOrder _next;
        private readonly Menu _menu;

        public Waiter(IHandleOrder next, Menu menu)
        {
            _next = next;
            _menu = menu;
        }

        public void PlaceOrder(IList<Tuple<int,int>> items, Guid id)
        {
            OrderDocument newOrder = new OrderDocument();
            newOrder.Id = id;
            foreach (var item in items)
            {
                MenuItem mItem = _menu.GetItem(item.Item1);
                newOrder.AddItem(new Item() {Id = item.Item1, Description = mItem.Description, Quantity = item.Item2, Price = mItem.Price});
            }
            _next.Handle(newOrder);
        }
    }
}
