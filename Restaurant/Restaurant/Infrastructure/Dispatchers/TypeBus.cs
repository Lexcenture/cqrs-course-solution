using System;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Messages;

namespace Restaurant.Infrastructure
{
    public class TypeBus
    {
        private readonly Object synclock = new object();
        private int subCount = 0;
        private Dictionary<RuntimeTypeHandle, Dictionary<int, Action<Message>>> subscriptions = new Dictionary<RuntimeTypeHandle, Dictionary<int, Action<Message>>>();
        public void Publish<T>(T message) where T : Message
        {
            Dictionary<int, Action<Message>> subscribers;
            if (subscriptions.TryGetValue(typeof(T).TypeHandle, out subscribers))
            {
                foreach (var pair in subscribers)
                {
                    pair.Value(message);
                }
            }
        }

        public void Unsubscribe<T>(int token) where T : Message
        {
            RuntimeTypeHandle handle = typeof(T).TypeHandle;
            Dictionary<int, Action<Message>> subscribers;
            //if (subscriptions.TryGetValue(handle, out subscribers) && subscribers.ContainsKey(token))
            //{
            lock (synclock)
            {
                var clonedSubscriptions = CloneSubscriptions();
                if (clonedSubscriptions.TryGetValue(handle, out subscribers))
                {
                    if (subscribers.Remove(token))
                    {
                        subscriptions = clonedSubscriptions;
                    }
                }
            }
            //}

        }
        public int Subscribe<T>(IHandle<T> handler) where T : Message
        {
            RuntimeTypeHandle handle = typeof(T).TypeHandle;
            Dictionary<int, Action<Message>> subscribers;
            lock (synclock)
            {
                var clonedSubscriptions = CloneSubscriptions();
                if (!clonedSubscriptions.TryGetValue(handle, out subscribers))
                {
                    subscribers = new Dictionary<int, Action<Message>>();
                    subscriptions.Add(handle, subscribers);
                }

                Action<Message> action = x =>
                {
                    T message = x as T;
                    if (message != null) handler.Handle(message);
                };

                subscribers.Add(++subCount, action);
                subscriptions = clonedSubscriptions;
            }
            return subCount;
        }

        private Dictionary<RuntimeTypeHandle, Dictionary<int, Action<Message>>> CloneSubscriptions()
        {
            return subscriptions.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToDictionary(p2 => p2.Key, p2 => p2.Value));
        }
    }
}