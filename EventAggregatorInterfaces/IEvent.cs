using System;

namespace EventAggregatorInterfaces
{
    /// <summary>
    /// Implement this interface to specify a specific event. e.g. ItemChangedEvent.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets or sets the sender of the event.
        /// </summary>
        /// <value>The sender.</value>
        object Sender { get; set; }
    }
}
