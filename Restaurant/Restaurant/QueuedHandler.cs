using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restaurant
{

    public interface IStartable
    {
        void Start();
    }

    public class QueuedHandler : IHandleOrder, IStartable
    {
        private BlockingCollection<OrderDocument> queue = new BlockingCollection<OrderDocument>();


        private readonly IHandleOrder _next;
        private readonly string _name;

        public QueuedHandler(IHandleOrder next, string name)
        {
            _next = next;
            _name = name;
        }


        public void ProcessQueue()
        {
            while (true)
            {
                OrderDocument next;
                if (queue.TryTake(out next))
                {
                    _next.Handle(next);
                }
                Thread.Sleep(1);
            }
        }

        public void Handle(OrderDocument order)
        {
            queue.Add(order);
        }

        public void Start()
        {
            Thread thread = new Thread(ProcessQueue);
            thread.IsBackground = true;
            thread.Start();
        }

        public int Count { get { return queue.Count; } }
        public string Name { get { return _name; } }

    }
}
