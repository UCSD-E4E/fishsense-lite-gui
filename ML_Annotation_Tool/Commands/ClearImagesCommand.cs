using FishSenseLiteGUI.ViewModels;
using System;
using System.Windows.Input;

namespace FishSenseLiteGUI.Commands
{
    /* Command to clear images. Linked to XAML button on page 2.
    *  1) Clears images from ObservableCollection.
    *  2) Disables pages 2 and 3.
    *  3) Allows user to rechoose directory and add new directory.
    */
    public class ClearImagesCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }
        public void Execute(object? parameter)
        {
            source.SelectedTabIndex = 0;
            source.databaseModel.ClearImages();

            source.SecondPageEnabled = false;
            source.ThirdPageEnabled = false;
        }

        MainWindowViewModel source;
        public ClearImagesCommand(MainWindowViewModel source)
        {
            this.source = source;
        }
    }
}
