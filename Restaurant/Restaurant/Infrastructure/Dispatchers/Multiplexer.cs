using System.Collections.Generic;
using Restaurant.Messages;

namespace Restaurant.Infrastructure
{
    public class Multiplexer<T> : IHandle<T> where T : Message
    {
        private readonly IEnumerable<IHandle<T>> _handlers;

        public Multiplexer(IEnumerable<IHandle<T>> handlers)
        {
            _handlers = handlers;
        }

        public void Handle(T order)
        {
            foreach (var handler in _handlers)
            {
                handler.Handle(order);
            }
        }
    }
}