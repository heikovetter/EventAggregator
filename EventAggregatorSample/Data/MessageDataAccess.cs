using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventAggregatorInterfaces;
using EventAggregatorSample.Interfaces;

namespace EventAggregatorSample.Data
{
    public class MessageDataAccess : IMessageDataAccess
    {
        public IEventHub EventHub { get; set; }

        public void SaveMessage(string message)
        {
            // ToDo: Save the message
            // ....

            var messageEvent = new MessageAddedEvent()
            {
                Message = message,
                Sender = this
            };

            this.EventHub?.Publish(messageEvent);
        }
    }
}
