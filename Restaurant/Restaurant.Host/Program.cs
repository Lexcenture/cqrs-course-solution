using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Restaurant.Actors;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Messages.Commands;
using Restaurant.Messages.Events;

namespace Restaurant.Host
{
    class Program
    {
        private static List<IMonitorableQueue> queues;
        private static volatile bool KeepOnMonitoring;

        static void Main(string[] args)
        {
            Menu menu = new Menu();
            Bus bus = new Bus();

            Manager manager = new Manager();
            var managerQueue = new QueuedHandler<OrderCompleted>(manager, "Manager Queue");

            Cashier cashier = new Cashier(bus);
            var cashierQueue = new QueuedHandler<PayForOrder>(cashier, "Cashier Queue");

            AssistantManager assistantManager = new AssistantManager(bus);
            var assistantManagerQueue = new QueuedHandler<PriceOrder>(assistantManager, "Assistant Manager Queue");

            Cook cook1 = new Cook(bus, 200);
            var cook1Queue = new QueuedHandler<CookOrder>(cook1, "Cook Queue1");

            Cook cook2 = new Cook(bus, 400);
            var cook2Queue = new QueuedHandler<CookOrder>(cook2, "Cook Queue2");

            Cook cook3 = new Cook(bus, 600);
            var cook3Queue = new QueuedHandler<CookOrder>(cook3, "Cook Queue3");

            var cookdispatcher = new QueueDispatcher<CookOrder>(new List<QueuedHandler<CookOrder>> { cook1Queue, cook2Queue, cook3Queue });
            var ttlh = new TimeToLiveHandler<CookOrder>(cookdispatcher);
            var cookDispatcherQueue = new QueuedHandler<CookOrder>(ttlh, "Cook Dispatcher");

            Waiter waiter = new Waiter(bus, menu);

            var orderStatusBoard = new OrderStatusBoard();

            bus.Subscribe(cookDispatcherQueue);
            bus.Subscribe(assistantManagerQueue);
            bus.Subscribe(cashierQueue);
            bus.Subscribe(managerQueue);

            var orders = Enumerable.Range(0, 100).Select(CreateOrder).ToArray();
            var orderIds = orders.Select(order => order.Item1);

            foreach (var orderId in orderIds.Take(1))
            {
                bus.SubscribleToCorrelationId<OrderPlaced>(orderId, orderStatusBoard);
                bus.SubscribleToCorrelationId<OrderCooked>(orderId, orderStatusBoard);
                bus.SubscribleToCorrelationId<OrderPriced>(orderId, orderStatusBoard);
                bus.SubscribleToCorrelationId<OrderPaid>(orderId, orderStatusBoard);
            }


            List<IStartable> startable = new List<IStartable> { managerQueue, cashierQueue, assistantManagerQueue, cook1Queue, cook2Queue, cook3Queue, cookDispatcherQueue };
            queues = new List<IMonitorableQueue>() { managerQueue, cashierQueue, assistantManagerQueue, cook1Queue, cook2Queue, cook3Queue, cookDispatcherQueue };
            startable.ForEach(s => s.Start());

            Thread monitor = new Thread(Printout) { IsBackground = true };
            KeepOnMonitoring = true;
            monitor.Start();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            foreach (var order in orders)
            {
                waiter.PlaceOrder(order.Item2, order.Item1);
            }

            int paidOrders = 0;
            while (queues.Sum(q => q.Count) > 0)
            {
                try
                {
                    List<OrderDocument> toPayList = cashier.GetAvailableOrders();
                    toPayList.ForEach(o =>
                    {
                        cashier.PayForOrder(o.Id);
                        ++paidOrders;
                    });
                }
                catch {}
            }

            KeepOnMonitoring = false;

            sw.Stop();
            Console.WriteLine("Completed in {0} milliseconds", sw.ElapsedMilliseconds);
            Console.WriteLine("Paid Orders: {0}, Completed Orders: {1}, Total: {2}", paidOrders, manager.Count, manager.Total);

            Console.ReadLine();
        }

        private static void Printout()
        {
            while (KeepOnMonitoring)
            {
                queues.ForEach(q => Console.WriteLine("{0}: {1}", q.Name, q.Count));
                Thread.Sleep(1000);
            }
        }


        private static Tuple<Guid, List<Tuple<int, int>>> CreateOrder(int i)
        {
            List<Tuple<int, int>> items = new List<Tuple<int, int>>();
            items.Add(new Tuple<int, int>(1, 2));
            items.Add(new Tuple<int, int>(2, 1));
            items.Add(new Tuple<int, int>(3, 2));
            items.Add(new Tuple<int, int>(4, 2));
            Guid orderId = Guid.NewGuid();
            return new Tuple<Guid, List<Tuple<int, int>>>(orderId, items);
        }
    }
}