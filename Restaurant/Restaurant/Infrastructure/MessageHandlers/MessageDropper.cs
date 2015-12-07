using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Messages;

namespace Restaurant.Infrastructure.MessageHandlers
{
    public class MessageDropper<T> : IHandle<T> where T : Message
    {
        private readonly IHandle<T> _next;
        private static Random rnd = new Random();

        public MessageDropper(IHandle<T> next)
        {
            _next = next;
        }

        public void Handle(T message)
        {
            if (rnd.Next(0,100) > 50) _next.Handle(message);
        }
    }
}
