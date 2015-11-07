using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restaurant
{
    class Cook : IHandleOrder
    {
        private readonly IHandleOrder _next;
        public Cook(IHandleOrder next)
        {
            _next = next;
        }

        public void Handle(OrderDocument order)
        {
            Thread.Sleep(500);
            order.AddIngredient("Salt");
            order.AddIngredient("Pepper");
            order.AddIngredient("Tomatoes");
            order.AddIngredient("Chorizo");
            order.AddIngredient("Sugar");
            _next.Handle(order);
        }
    }
}
