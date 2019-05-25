using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventAggregatorInterfaces;
using EventAggregatorSample.Interfaces;
using GalaSoft.MvvmLight;

namespace EventAggregatorSample.ViewModels
{
    public class WhatsUpViewModel : ViewModelBase, IWhatsUpViewModel
    {
        private string _message;
        private IEventHub _eventHub;

        public WhatsUpViewModel(IEventHub eventHub)
        {
            this._eventHub = eventHub;
            this._eventHub.Subscribe<MessageAddedEvent>(this, MessageAddedHandler);
            this._eventHub.Exists<MessageAddedEvent>(this);
        }

        private void MessageAddedHandler(MessageAddedEvent messageAddedEvent)
        {
            this.Message += messageAddedEvent.Message + Environment.NewLine;
        }

        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                if (this._message != value)
                {
                    this._message = value;
                    this.RaisePropertyChanged(nameof(this.Message));
                }
            }
        }


    }
}
