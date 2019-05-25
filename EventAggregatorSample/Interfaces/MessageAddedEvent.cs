using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventAggregator;

namespace EventAggregatorSample.Interfaces
{
    public class MessageAddedEvent : Event
    {
        public string Message { get; set; }
    }
}
