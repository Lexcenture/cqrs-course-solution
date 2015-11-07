using Restaurant.DomainModel;
using Restaurant.Infrastructure;
using Restaurant.Messages;

namespace Restaurant.Actors
{
    public class Manager : IHandle<OrderPaid>
    {
        private int count = 0;
        private decimal total = 0m;

        public void Handle(OrderPaid orderPaid)
        {
            OrderDocument order = orderPaid.Order;
            count++;
            total += order.Total;
        }

        public int Count { get { return count; } }
        public decimal Total { get { return total; } }        
    }

    

    //public class Printer : IHandleOrder
    //{
    //    public void Handle(OrderDocument order)
    //    {
    //        Console.WriteLine(order.Serialize());
    //    }
    
}