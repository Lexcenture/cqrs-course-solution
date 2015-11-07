using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using NUnit.Framework;


namespace ProcessManager2.Tests
{
    [TestFixture]
    public class ProcessManagerTests
    {
        //given positionaquired event is rased 
        //

        private FakeEventBus bus;
        private TrailingStopLossProcessManager processManager;
        private Decimal aquirePrice;

        [SetUp]
        public void Setup()
        {
            bus = new FakeEventBus();
            aquirePrice = 100;
            processManager = new TrailingStopLossProcessManager(bus, aquirePrice);
        }

        [Test]
        public void ProcessManager_Receives_Price_Updated_Event_Publishes_Reminder_Events()
        {
            //given initialised Process Manager 
            //when Price Updated event arrices
            //then  - Remindme(13, droppricefromsellwindow)
            //      - Remindme(11, droppricefrommoveupwindow)

            // arrange
            var message = new PriceUpdatedEvent(105);


            // act
            processManager.Handle(message);

            // assert
            Assert.That(bus.PublishedEvents.Count, Is.EqualTo(3));

                // this is really part of the arrangement for this test. we should clear it before acting
            var RemoveSellThresholdSetEvent =
                (SellThresholdSetEvent)
                    bus.PublishedEvents.Single(evt => evt.GetType() == typeof (SellThresholdSetEvent));

            Assert.That(RemoveSellThresholdSetEvent.Price, Is.EqualTo(90));

            var RemoveFromHighWindowReminderEvent =
                (ReminderEvent<RemoveFromHighWindow>)
                    bus.PublishedEvents.Single(evt => evt.GetType() == typeof (ReminderEvent<RemoveFromHighWindow>));

            Assert.That(RemoveFromHighWindowReminderEvent.Message, Is.TypeOf<RemoveFromHighWindow>());
            Assert.That(RemoveFromHighWindowReminderEvent.Message.Price, Is.EqualTo(105));
            Assert.That(RemoveFromHighWindowReminderEvent.Seconds, Is.EqualTo(11));

            var RemoveFromLowWindowReminderEvent =
                (ReminderEvent<RemoveFromLowWindow>)
                    bus.PublishedEvents.Single(evt => evt.GetType() == typeof (ReminderEvent<RemoveFromLowWindow>));

            Assert.That(RemoveFromLowWindowReminderEvent.Message, Is.TypeOf<RemoveFromLowWindow>());
            Assert.That(RemoveFromLowWindowReminderEvent.Message.Price, Is.EqualTo(105));
            Assert.That(RemoveFromLowWindowReminderEvent.Seconds, Is.EqualTo(13));


        }

        [Test]
        public void ProcessManager_Recieves_Reminder_Event_Publishes_SellEvent()
        {
            //arrange 
            var priceUpdatedMessage = new PriceUpdatedEvent(80);
            processManager.Handle(priceUpdatedMessage);
            var RemoveFromLowWindow = new RemoveFromLowWindow(80);
            bus.ClearEvents();

            //act 
            processManager.Handle(RemoveFromLowWindow);

            // assert
            Assert.That(bus.PublishedEvents.Count, Is.EqualTo(1));
            var sellEvent =
                (SellEvent)
                    bus.PublishedEvents.SingleOrDefault(evt => evt.GetType() == typeof(SellEvent));
            Assert.That(sellEvent, Is.Not.Null);

        }

        [Test]
        public void ProcessManager_Recieves_Reminder_Event_Does_Not_Publishe_SellEvent()
        {
            //arrange 
            var priceUpdatedMessage = new PriceUpdatedEvent(99);
            processManager.Handle(priceUpdatedMessage);
            var RemoveFromLowWindow = new RemoveFromLowWindow(99);
            bus.ClearEvents();

            //act 
            processManager.Handle(RemoveFromLowWindow);

            // assert
            Assert.That(bus.PublishedEvents.Count, Is.EqualTo(0));

        }

        public class FakeEventBus : IEventBus
        {
            private readonly List<IEvent> events;

            public List<IEvent> PublishedEvents { get { return events;  } } 

            public FakeEventBus()
            {
                events = new List<IEvent>();
            }

            public void ClearEvents()
            {
                PublishedEvents.Clear();
            }

            public void Publish(IEvent @event)
            {
                events.Add(@event);
            }
        }



    }
}
