using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restaurant
{
    class Program
    {
        private static List<QueuedHandler> queues; 

        static void Main(string[] args)
        {
            Menu menu = new Menu();

            IHandleOrder printer = new Printer();
            QueuedHandler printerQueue = new QueuedHandler(printer, "Printer Queue");
            Cashier cashier = new Cashier(printerQueue);
            QueuedHandler cashierQueue = new QueuedHandler(cashier, "Cashier Queue");
            IHandleOrder assistantManager = new AssistantManager(cashierQueue);
            QueuedHandler assistantManagerQueue = new QueuedHandler(assistantManager, "Assistant Manager Queue");
            IHandleOrder cook = new Cook(assistantManagerQueue);
            QueuedHandler cookQueue = new QueuedHandler(cook, "Cook Queue");
            Waiter waiter = new Waiter(cookQueue, menu);

            List<IStartable> startable = new List<IStartable>() {printerQueue, cashierQueue, assistantManagerQueue, cookQueue};
            queues = new List<QueuedHandler>() { printerQueue, cashierQueue, assistantManagerQueue, cookQueue };
            startable.ForEach(s => s.Start());

            Thread monitor = new Thread(Printout) { IsBackground = true };
            monitor.Start();


            var orders = Enumerable.Range(0, 100).Select(CreateOrder).ToArray();
            foreach (var order in orders)
            {
                waiter.PlaceOrder(order.Item2, order.Item1);
            }

            int paidOrders = 0;
            while (paidOrders < 100)
            {
                try
                {
                    List<OrderDocument> toPayList = cashier.GetAvailableOrders();
                    toPayList.ForEach(o =>
                    {
                        cashier.PayForOrder(o.Id);
                        paidOrders++;
                    });
                }
                catch
                {
                }
            }

            Console.ReadLine();
        }

        private static void Printout()
        {
            while (true)
            {
                queues.ForEach(q => Console.WriteLine("{0}: {1}", q.Name, q.Count));
                Thread.Sleep(1000);
            }
        }


        private static Tuple<Guid, List<Tuple<int,int>>> CreateOrder(int i)
        {
            List<Tuple<int, int>> items = new List<Tuple<int, int>>();
            items.Add(new Tuple<int, int>(1, 2));
            items.Add(new Tuple<int, int>(2, 1));
            items.Add(new Tuple<int, int>(3, 2));
            items.Add(new Tuple<int, int>(4, 2));
            Guid orderId = Guid.NewGuid();
            return new Tuple<Guid, List<Tuple<int,int>>>(orderId, items);
        }
    }
}