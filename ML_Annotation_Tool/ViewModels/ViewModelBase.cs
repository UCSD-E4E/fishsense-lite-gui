using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FishSenseLiteGUI.ViewModels
{
    /// <summary>
    /// Purpose: Implements INotifyPropertyChanged interface, allowing children classes to call the
    ///          OnPropertyChanged() event for their instance variables.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
