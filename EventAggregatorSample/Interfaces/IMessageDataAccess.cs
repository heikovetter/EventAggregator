using System;

namespace EventAggregatorSample.Interfaces
{
    public interface IMessageDataAccess
    {
        void SaveMessage(string message);
    }
}