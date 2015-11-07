using System;
using System.Collections.Generic;

namespace DocumentEvent
{
    class Program
    {
        static void Main(string[] args)
        {
            var cashier = new Cashier(new Printer());
            var waiter = new Waiter(new Cook(new AssistantManager(cashier)));
            var orderId = Guid.NewGuid();
            waiter.PlaceOrder(new List<Item> { new Item(1, "Humous", 2, 2.95m), new Item(5, "Shish Kebab", 1, 10.95m) }, orderId);
            cashier.PayFor(orderId);
            Console.ReadKey();
        }
    }
}
