using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Restaurant.Actors;
using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.MessageHandlers;
using Restaurant.Infrastructure.ProcessManagers;
using Restaurant.Messages.Commands;
using Restaurant.Messages.Events;

namespace Restaurant.Host
{
    class Program
    {
        private static List<IMonitorableQueue> queues;
        private static volatile bool KeepOnMonitoring;
        private static int paidOrders = 0;
        private static Cashier cashier;

        static void Main(string[] args)
        {
            Menu menu = new Menu();
            Bus bus = new Bus();

            Manager manager = new Manager();
            var managerQueue = new QueuedHandler<OrderCompleted>(manager, "Manager Queue");

            cashier = new Cashier(bus);
            var cashierQueue = new QueuedHandler<PayForOrder>(cashier, "Cashier Queue");

            AssistantManager assistantManager = new AssistantManager(bus);
            var assistantManagerQueue = new QueuedHandler<PriceOrder>(assistantManager, "Assistant Manager Queue");

            Cook cook1 = new Cook(bus, 200);
            var cook1Queue = new QueuedHandler<CookOrder>(cook1, "Cook Queue1");

            Cook cook2 = new Cook(bus, 400);
            var cook2Queue = new QueuedHandler<CookOrder>(cook2, "Cook Queue2");

            Cook cook3 = new Cook(bus, 600);
            var cook3Queue = new QueuedHandler<CookOrder>(cook3, "Cook Queue3");

            EvilActor evilActor = new EvilActor(bus);
            
            var cookdispatcher = new QueueDispatcher<CookOrder>(new List<QueuedHandler<CookOrder>> { cook1Queue, cook2Queue, cook3Queue });
            //var dropper = new MessageDropper<CookOrder>(cookdispatcher);
            //var ttlh = new TimeToLiveHandler<CookOrder>(dropper);
            var cookDispatcherQueue = new QueuedHandler<CookOrder>(cookdispatcher, "Cook Dispatcher");

            UkRestaurantProcessManagerPool processManagerPool = new UkRestaurantProcessManagerPool(bus);
            bus.Subscribe(processManagerPool);

            AlarmClock<OrderPlaced> orderPlacedReminder = new AlarmClock<OrderPlaced>(bus);
            AlarmClock<CookOrder> cookReminder = new AlarmClock<CookOrder>(bus);

            Waiter waiter = new Waiter(bus, menu);

            var orderStatusBoard = new OrderStatusBoard();
            
            bus.Subscribe(orderPlacedReminder);
            bus.Subscribe(cookReminder);
            bus.Subscribe(cookDispatcherQueue);
            bus.Subscribe(assistantManagerQueue);
            bus.Subscribe(cashierQueue);
            bus.Subscribe(managerQueue);
            bus.Subscribe(evilActor);
            bus.Subscribe<HumanInterventionRequired>(manager);

            List<IStartable> startable = new List<IStartable> { managerQueue, cashierQueue, assistantManagerQueue, cook1Queue, cook2Queue, cook3Queue, cookDispatcherQueue };
            queues = new List<IMonitorableQueue>() { managerQueue, cashierQueue, assistantManagerQueue, cook1Queue, cook2Queue, cook3Queue, cookDispatcherQueue };
            startable.ForEach(s => s.Start());

            //Thread monitor = new Thread(Printout) { IsBackground = true };
            KeepOnMonitoring = true;
            //monitor.Start();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i <= 20; i++)
            {
                Customer cust = new Customer(bus, waiter, cashier);
                var orderId = cust.PlaceOrder();
                //bus.SubscribleToCorrelationId<OrderPlaced>(orderId, orderStatusBoard);
                //bus.SubscribleToCorrelationId<OrderCooked>(orderId, orderStatusBoard);
                //bus.SubscribleToCorrelationId<OrderPriced>(orderId, orderStatusBoard);
                //bus.SubscribleToCorrelationId<OrderPaid>(orderId, orderStatusBoard);
            }

            Console.ReadLine();
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
    }
}