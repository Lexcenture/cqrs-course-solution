using System;
using Restaurant.DomainModel;
using Restaurant.Messages;

namespace Restaurant.Infrastructure
{
    public class TimeToLiveHandler<T> : IHandle<T> where T : TimeToLiveMessage
    {
        private readonly IHandle<T> _next;

        public TimeToLiveHandler(IHandle<T> next)
        {
            _next = next;
        }

        public void Handle(T message)
        {
            if (message.Expire > DateTime.Now) _next.Handle(message);
        }
    }
}
