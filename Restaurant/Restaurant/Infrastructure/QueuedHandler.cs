using System.Collections.Concurrent;
using System.Threading;
using Restaurant.Messages;

namespace Restaurant.Infrastructure
{

    public interface IStartable
    {
        void Start();
    }

    public interface IMonitorableQueue
    {
        int Count { get; }
        string Name { get; }
    }

    public class QueuedHandler<T> : IHandle<T>, IStartable, IMonitorableQueue where T : Message
    {
        private BlockingCollection<T> queue = new BlockingCollection<T>();


        private readonly IHandle<T> _handler;
        private readonly string _name;

        public QueuedHandler(IHandle<T> handler, string name)
        {
            _handler = handler;
            _name = name;
        }


        private void ProcessQueue()
        {
            while (true)
            {
                T next;
                if (queue.TryTake(out next))
                {
                    _handler.Handle(next);
                }
                Thread.Sleep(1);
            }
        }


        public void Start()
        {
            Thread thread = new Thread(ProcessQueue);
            thread.IsBackground = true;
            thread.Start();
        }

        public int Count { get { return queue.Count; } }
        public string Name { get { return _name; } }

        public void Handle(T message)
        {
            queue.Add(message);
        }
    }
}
