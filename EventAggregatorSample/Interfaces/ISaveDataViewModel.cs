using System.Windows.Input;

namespace EventAggregatorSample.Interfaces
{
    public interface ISaveDataViewModel
    {
        ICommand SaveDataLocalCommand { get; }
        ICommand SendDataToServerCommand { get; }
        string Message { get; set; }
    }
}