using System;
using System.Activities.Statements;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using EventAggregator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventAggregatorTests
{
    /// <summary>
    /// Event hub tests.
    /// </summary>
    [TestClass]
    public class EventHabTest
    {
        /// <summary>
        /// Test a simple post and subscribe of a event.
        /// </summary>
        [TestMethod]
        public void EventHub_PostAndSubscribe_EventHandled()
        {
            // arrange
            var eventHub = new EventHub();
            var subscriber1 = new object();
            var subscriber1Triggered = false;
            var subscriber1Action = new Action<ItemChangedEvent>(itemChangedEvent => { subscriber1Triggered = true; });

            // act
            eventHub.Subscribe(subscriber1, subscriber1Action);
            eventHub.Publish(new ItemChangedEvent());

            // assert
            Assert.AreEqual(true, subscriber1Triggered);
        }

        /// <summary>
        /// Test a simple post and subscribe of a event over different threads.
        /// </summary>
        [TestMethod]
        public void EventHub_PostAndSubscribeOverThread_EventHandled()
        {
            // arrange
            var publishThreadId = -1;

            var eventHub = new EventHub();
            var subscriber1 = new object();
            var subscriberThreadId = Thread.CurrentThread.ManagedThreadId;
            var subscribedThreadId = -2;
            var subscriber1Triggered = false;
            var subscriber1Action = new Action<ItemChangedEvent>(itemChangedEvent =>
            {
                subscribedThreadId = Thread.CurrentThread.ManagedThreadId;
                subscriber1Triggered = true;
            });


            // act
            eventHub.Subscribe(subscriber1, subscriber1Action);

            Task.Run(() =>
            {
                publishThreadId = Thread.CurrentThread.ManagedThreadId;
                eventHub.Publish(new ItemChangedEvent());
            }).Wait();


            // assert
            Assert.AreEqual(true, subscriber1Triggered);
            Assert.AreEqual(publishThreadId, subscribedThreadId);
            Assert.AreNotEqual(publishThreadId, subscriberThreadId);
        }

        /// <summary>
        /// Sample Event for unit test.
        /// </summary>
        /// <seealso cref="EventAggregator.Event" />
        private class ItemChangedEvent : Event
        {

        }
    }
}
