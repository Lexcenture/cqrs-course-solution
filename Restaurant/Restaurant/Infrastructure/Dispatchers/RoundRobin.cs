using System.Collections.Generic;
using Restaurant.Messages;

namespace Restaurant.Infrastructure
{
    public class RoundRobin<T> : IHandle<T> where T : Message
    {

        private readonly Queue<IHandle<T>> handlerQueue = new Queue<IHandle<T>>();
        public RoundRobin(IEnumerable<IHandle<T>> handlers)
        {
            foreach (var handler in handlers)
            {
                handlerQueue.Enqueue(handler);
            }
        }

        public void Handle(T message)
        {
            IHandle<T> next = handlerQueue.Dequeue();
            next.Handle(message);
            handlerQueue.Enqueue(next);
        }
    }
}
