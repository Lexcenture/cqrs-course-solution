using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessManager2
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public interface IHandle<in TEvent> where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }

    public interface IEvent { }


    public class PriceEvent : IEvent
    {
        public PriceEvent(decimal price)
        {
            Price = price;
        }
        public Decimal Price { get; private set; }
    }

    public class PositionAcquiredEvent : PriceEvent
    {
        public PositionAcquiredEvent(decimal price) : base(price) { }
    }

    public class SellThresholdSetEvent : PriceEvent
    {
        public SellThresholdSetEvent(decimal price) : base(price) { }
    }

    public class PriceUpdatedEvent : PriceEvent
    {
        public PriceUpdatedEvent(decimal price) : base(price) { }
    }

    public class RemoveFromHighWindow : PriceEvent
    {
        public RemoveFromHighWindow(decimal price) : base(price) { }
    }

    public class RemoveFromLowWindow : PriceEvent
    {
        public RemoveFromLowWindow(decimal price) : base(price) { }
    }

    public class ReminderEvent<TEvent> : IEvent where TEvent : IEvent
    {
        public int Seconds { get; private set; }
        public TEvent Message { get; private set; }

        public ReminderEvent(int milliseconds, TEvent message)
        {
            Seconds = milliseconds;
            Message = message;
        }
    }

    public class SellEvent : IEvent { }

    public interface IEventBus
    {
        void Publish(IEvent @event);
    }






}
