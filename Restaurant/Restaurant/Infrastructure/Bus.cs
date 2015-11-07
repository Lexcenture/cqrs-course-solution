using System;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Messages;

namespace Restaurant.Infrastructure
{
    public class Bus
    {
        //private Dictionary<string, List<IHandleOrder>> subscriptions = new Dictionary<string, List<IHandleOrder>>();
        private Dictionary<string, List<Action<Message>>> _subscription; 
        private Object synclock = new object();

        public Bus()
        {
            _subscription = new Dictionary<string, List<Action<Message>>>();
        }

        public void Publish<T>(T message) where T : Message
        {
            var topic = message.GetType().Name;
            Publish(topic, message);

            var correlationId = message.CorrelationId.ToString();
            Publish(correlationId, message);
        }

        private void Publish<T>(string topic, T message) where T : Message
        {
            List<Action<Message>> subscribers;
            if (_subscription.TryGetValue(topic, out subscribers))
            {
                subscribers.ForEach(subscriber => subscriber(message));
            }
        }

        public void SubscribleToCorrelationId<T>(Guid correlationId, IHandle<T> handler) where T : Message
        {
            Subscribe(correlationId.ToString(), handler);
        }

        public void Subscribe<T>(IHandle<T> handler) where T : Message
        {
            Subscribe(typeof(T).Name, handler);
        }

        private void Subscribe<T>(string topic, IHandle<T> handler) where T : Message
        {
            lock (synclock)
            {
                var clonedSubscribers = CloneSubscriptions();
                List<Action<Message>> currentSubscriptions;
                if (!clonedSubscribers.TryGetValue(topic, out currentSubscriptions))
                {
                    currentSubscriptions = new List<Action<Message>>();
                    clonedSubscribers[topic] = currentSubscriptions;
                }

                Action<Message> action = x =>
                {
                    T message = x as T;
                    if (message != null) handler.Handle(message);
                };

                currentSubscriptions.Add(action);
                _subscription = clonedSubscribers;
            }
        }

        private Dictionary<string, List<Action<Message>>> CloneSubscriptions()
        {
            return _subscription.ToDictionary(pair => pair.Key, pair => new List<Action<Message>>(pair.Value));
        }
    }
}