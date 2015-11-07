using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessManager2
{
    public class TrailingStopLossProcessManager: IHandle<PriceUpdatedEvent>, IHandle<RemoveFromLowWindow>
    {
        private readonly IEventBus Bus;
        private readonly decimal _aquirePrice; // should be redundant

        private Decimal sellThreshold;
        private List<Decimal> sellThresholdWindow = new List<decimal>();
        private Decimal highWatermark;
        private List<Decimal> highWatermarkWindow = new List<decimal>(); 

        public TrailingStopLossProcessManager(IEventBus bus, Decimal aquirePrice)
        {
            Bus = bus;
            _aquirePrice = aquirePrice;
            highWatermark = aquirePrice;
            sellThreshold = highWatermark - 10;
            bus.Publish(new SellThresholdSetEvent(sellThreshold));
        }

        public void Handle(PriceUpdatedEvent priceUpdatedEvent)
        {
            sellThresholdWindow.Add(priceUpdatedEvent.Price);
            Bus.Publish(new ReminderEvent<RemoveFromLowWindow>(13, new RemoveFromLowWindow(priceUpdatedEvent.Price)));
            highWatermarkWindow.Add(priceUpdatedEvent.Price);
            Bus.Publish(new ReminderEvent<RemoveFromHighWindow>(11, new RemoveFromHighWindow(priceUpdatedEvent.Price)));
        }

        public void Handle(RemoveFromLowWindow removeFromLowWindowEvent)
        {
            if (sellThresholdWindow.Max() <= sellThreshold)
            {
                Bus.Publish(new SellEvent());
                // need to wind yourself up now
            }
            else
            {
                sellThresholdWindow.RemoveAt(0);
            }
        }

        public void Handle(RemoveFromHighWindow removeFromHighWindowEvent)
        {
            if (highWatermarkWindow.Min() >= highWatermark)
            {
                highWatermark = highWatermarkWindow.Min();
                sellThreshold = highWatermark - 10;
                Bus.Publish(new SellThresholdSetEvent(sellThreshold));
            }
            else
            {
                highWatermarkWindow.RemoveAt(0);
            }
        }

    }
}
