using System;
using System.Threading.Tasks;
using Restaurant.Actors;
using Restaurant.Messages;

namespace Restaurant.Infrastructure
{
    public class AlarmClock<T> : IHandle<RemindmeCommand<T>> where T : Message
    {
        private readonly Bus _bus;

        public AlarmClock(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(RemindmeCommand<T> message)
        {
            Task.Delay(message.Seconds*1000).ContinueWith(t =>
            {
                _bus.Publish(new RememberEvent<T>(message.Message)
                {
                    MessageId = Guid.NewGuid(),
                    CorrelationId = message.Message.CorrelationId,
                    CausationId = message.Message.MessageId
                });
            }); 
        }
    }
}