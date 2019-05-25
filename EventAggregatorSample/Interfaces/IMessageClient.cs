using System;

namespace EventAggregatorSample.Interfaces
{
    public interface IMessageClient
    {
        void SendMessage(string message);
    }
}