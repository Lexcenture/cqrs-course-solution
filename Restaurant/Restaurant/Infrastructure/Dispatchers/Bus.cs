using System;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Messages;

namespace Restaurant.Infrastructure.Dispatchers
{
    public class Bus
    {
        private Dictionary<string, Dictionary<int,Action<Message>>> _subscription;
        private int _subscriptionCount;
        private readonly Object _synclock = new object();

        private const string AllTopic = "All";

        public Bus()
        {
            _subscription = new Dictionary<string, Dictionary<int,Action<Message>>>();
        }

        public void Publish<T>(T message) where T : Message
        {
            var topic = message.GetType().Name;
            Publish(topic, message);

            var correlationId = message.CorrelationId.ToString();
            Publish(correlationId, message);
            Publish(AllTopic, message);
        }

        private void Publish<T>(string topic, T message) where T : Message
        {
            Dictionary<int,Action<Message>> subscribers;
            if (_subscription.TryGetValue(topic, out subscribers))
            {
                subscribers.Values.ToList().ForEach(sub => sub(message));
            }
        }

        public void UnsubscribeFromCorrelationId(Guid correlationId, int token)
        {
            lock (_synclock)
            {
                var clonedSubscribers = CloneSubscriptions();
                Dictionary<int,Action<Message>> currentSubscriptions;
                if (clonedSubscribers.TryGetValue(correlationId.ToString(), out currentSubscriptions))
                {
                    currentSubscriptions.Remove(token);
                    if (currentSubscriptions.Count == 0)
                    {
                        clonedSubscribers.Remove(correlationId.ToString());
                    }
                }
            }
        }

        public int SubscribleToCorrelationId<T>(Guid correlationId, IHandle<T> handler) where T : Message
        {
            return Subscribe(correlationId.ToString(), handler);
        }

        public void Subscribe<T>(IHandle<T> handler) where T : Message
        {
            Subscribe(typeof(T).Name, handler);
        }

        public int SubscribeToAll<T>(IHandle<T> handler) where T : Message
        {
            return Subscribe(AllTopic, handler);
        }

        private int Subscribe<T>(string topic, IHandle<T> handler) where T : Message
        {
            lock (_synclock)
            {
                var clonedSubscribers = CloneSubscriptions();
                Dictionary<int,Action<Message>> currentSubscriptions;
                _subscriptionCount++;
                if (!clonedSubscribers.TryGetValue(topic, out currentSubscriptions))
                {
                    currentSubscriptions = new Dictionary<int, Action<Message>>();
                    clonedSubscribers[topic] = currentSubscriptions;
                }

                Action<Message> action = x =>
                {
                    T message = x as T;
                    if (message != null) handler.Handle(message);
                };

                currentSubscriptions.Add(_subscriptionCount, action);
                _subscription = clonedSubscribers;
            }
            return _subscriptionCount;
        }

        private Dictionary<string, Dictionary<int,Action<Message>>> CloneSubscriptions()
        {
            return _subscription.ToDictionary(pair => pair.Key, pair => pair.Value.ToDictionary(ia => ia.Key, ia=>ia.Value));
        }
    }
}