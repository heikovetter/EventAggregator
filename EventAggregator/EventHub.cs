using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using EventAggregatorInterfaces;

namespace EventAggregator
{
    /// <summary>
    /// Class to manage the publication and subscription of event.
    /// Represents the event aggregator pattern.
    /// </summary>
    public class EventHub : IEventHub
    {
        #region private fields

        /// <summary>
        /// The synchronize object
        /// </summary>
        private readonly object _syncObject = new object();

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets a list of all registered event subscribers.
        /// </summary>
        /// <value>The event subscribers.</value>
        protected virtual List<EventHandler> Subscribers { get; set; } = new List<EventHandler>();

        #endregion


        /// <summary>
        /// Publishes the specified event data to all subscribers.
        /// </summary>
        /// <typeparam name="T">Type of the event data. All subscribers of this type gets triggered.</typeparam>
        /// <param name="eventData">The event data to publish.</param>
        public virtual void Publish<T>(T eventData)
        {
            List<EventHandler> subscribersToCall;

            lock (_syncObject)
            {
                subscribersToCall = new List<EventHandler>();
                var subscribersToRemove = new List<EventHandler>();

                // find all subscribers to trigger and all subscriber to remove.
                foreach (var subscriber in this.Subscribers)
                {
                    if (!subscriber.Subscriber.IsAlive)
                    {
                        // add the subscriber to remove, because the instance is not alive anymore.
                        subscribersToRemove.Add(subscriber);
                    }
                    else if (subscriber.Type.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
                    {
                        // find subscribers to call.
                        subscribersToCall.Add(subscriber);
                    }
                }

                this.RemoveHandlers(subscribersToRemove);
            }

            this.RaiseEvent(subscribersToCall, eventData);
        }

        /// <summary>
        /// Subscribe to a specified event type.
        /// </summary>
        /// <typeparam name="T">The event type, on which you want to register.</typeparam>
        /// <param name="subscriber">The subscriber, usually the calling instance ("this").</param>
        /// <param name="handler">Delegate to handle the event.</param>
        /// <param name="useSynchronizationContext">if set to <c>true</c> and if there is a current synchronization context, the handler gets called via the synchronization context of the subscribe time.</param>
        public virtual void Subscribe<T>(object subscriber, Action<T> handler, bool useSynchronizationContext = false)
        {
            lock (_syncObject)
            {
                var item = new EventHandler
                {
                    Action = handler,
                    Subscriber = new WeakReference(subscriber),
                    Type = typeof(T),
                    SynchronizationContext = (useSynchronizationContext) ? SynchronizationContext.Current : null
                };

                this.Subscribers.Add(item);
            }
        }

        /// <summary>
        /// Unsubscribe to all events of the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        public virtual void Unsubscribe(object subscriber)
        {
            lock (_syncObject)
            {
                var query = this.Subscribers.Where(a => !a.Subscriber.IsAlive || a.Subscriber.Target.Equals(subscriber));

                this.RemoveHandlers(query.ToList());
            }
        }

        /// <summary>
        /// Unsubscribe the specified handler.
        /// </summary>
        /// <typeparam name="T">The event type, on which you want to unregister.</typeparam>
        /// <param name="subscriber">The subscriber, which is registered.</param>
        /// <param name="handler">The handler to unsubscribe.</param>
        public virtual void Unsubscribe<T>(object subscriber, Action<T> handler = null)
        {
            lock (_syncObject)
            {
                var query = this.Subscribers.Where(a => !a.Subscriber.IsAlive || (a.Subscriber.Target.Equals(subscriber) && a.Type == typeof(T)));

                if (handler != null)
                {
                    query = query.Where(a => a.Action.Equals(handler));
                }

                this.RemoveHandlers(query.ToList());
            }
        }

        /// <summary>
        /// Checks if there are handlers register for the specified subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <returns><c>true</c> if there are handlers registered, <c>false</c> otherwise.</returns>
        public virtual bool Exists(object subscriber)
        {
            lock (_syncObject)
            {
                return this.Subscribers.Any(handlerToCompare =>
                    Equals(handlerToCompare.Subscriber.Target, subscriber));
            }
        }

        /// <summary>
        /// Checks if there are handlers register for the specified event type and subscriber.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <returns><c>true</c> if there are handlers registered, <c>false</c> otherwise.</returns>
        public virtual bool Exists<T>(object subscriber)
        {
            lock (_syncObject)
            {
                return this.Subscribers.Any(handlerToCompare =>
                    Equals(handlerToCompare.Subscriber.Target, subscriber)
                    && typeof(T) == handlerToCompare.Type);
            }
        }

        /// <summary>
        /// Checks if there are handlers register for the specified event type, subscriber and handler delegate.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="handler">The handler.</param>
        /// <returns><c>true</c> if there are handlers registered, <c>false</c> otherwise.</returns>
        public virtual bool Exists<T>(object subscriber, Action<T> handler)
        {
            lock (_syncObject)
            {
                return this.Subscribers.Any(handlerToCompare =>
                     Equals(handlerToCompare.Subscriber.Target, subscriber)
                     && typeof(T) == handlerToCompare.Type
                     && handlerToCompare.Action.Equals(handler));
            }
        }

        /// <summary>
        /// Raises the event to the specified handler list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlerList">The handler list.</param>
        /// <param name="eventData">The event data.</param>
        protected virtual void RaiseEvent<T>(List<EventHandler> handlerList, T eventData)
        {
            if (handlerList == null)
                return;

            foreach (var handler in handlerList)
            {
                if (handler.SynchronizationContext != null)
                {
                    handler.SynchronizationContext.Post(s => ((Action<T>)handler.Action)(eventData), null);
                }
                else
                {
                    ((Action<T>)handler.Action)(eventData);
                }
            }
        }

        /// <summary>
        /// Removes the specified handlers from the handlers list.
        /// </summary>
        /// <param name="handlersToRemove">The handlers to remove.</param>
        protected virtual void RemoveHandlers(ICollection<EventHandler> handlersToRemove)
        {
            if(handlersToRemove == null)
                return;

            foreach (var handler in handlersToRemove)
            {
                this.Subscribers.Remove(handler);
            }
        }
    }
}