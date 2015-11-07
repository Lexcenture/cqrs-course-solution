using System;

namespace Restaurant.Messages
{
    public abstract class TimeToLiveMessage : Message
    {
        public DateTime Expire { get; protected set; }
    }
}