using System.Collections.Generic;
using Restaurant.Messages;

namespace Restaurant.Infrastructure
{
    public class QueueDispatcher<T> : IHandle<T> where T : Message
    {
        private readonly Queue<QueuedHandler<T>> handlerQueue = new Queue<QueuedHandler<T>>();

        public QueueDispatcher(IEnumerable<QueuedHandler<T>> handlers)
        {
            foreach (var handler in handlers)
            {
                handlerQueue.Enqueue(handler);
            }
        }

        private bool InternalHandle(T order)
        {
            for (int i = 0; i < handlerQueue.Count; i++)
            {
                QueuedHandler<T> next = handlerQueue.Dequeue();
                if (next.Count < 5)
                {
                    next.Handle(order);
                    handlerQueue.Enqueue(next);
                    return true;
                }
                handlerQueue.Enqueue(next);
            }
            return false;
        }

        public void Handle(T order)
        {
            while (!InternalHandle(order)){ }
        }
    }
}