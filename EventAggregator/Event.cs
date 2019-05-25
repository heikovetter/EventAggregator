using System;
using System.Collections.Generic;
using System.Text;
using EventAggregatorInterfaces;

namespace EventAggregator
{
    /// <summary>
    /// Base class of an event.
    /// Derive from this class to specify a specific event. e.g. ItemChangedEvent.
    /// </summary>
    public abstract class Event : IEvent
    {
        /// <summary>
        /// Gets or sets the sender of the event.
        /// </summary>
        /// <value>The sender.</value>
        public object Sender { get; set; }
    }
}
