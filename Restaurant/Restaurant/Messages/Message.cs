using System;

namespace Restaurant.Messages
{
    public abstract class Message
    {
        public Guid MessageId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid CausationId { get; set; }
    }
}