using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EventAggregatorSample.Data;
using EventAggregatorSample.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace EventAggregatorSample.ViewModels
{
    public class SaveDataViewModel : ViewModelBase, ISaveDataViewModel
    {
        private IMessageDataAccess _messageDataAccess;
        private IMessageClient _messageClient;
        private string _message;

        public SaveDataViewModel(IMessageDataAccess messageDataAccess, IMessageClient messageClient)
        {
            this._messageDataAccess = messageDataAccess;
            this._messageClient = messageClient;
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

        public ICommand SaveDataLocalCommand => new RelayCommand(() => this._messageDataAccess?.SaveMessage(this.Message));

        public ICommand SendDataToServerCommand => new RelayCommand(() => this._messageClient?.SendMessage(this.Message));
    }
}
