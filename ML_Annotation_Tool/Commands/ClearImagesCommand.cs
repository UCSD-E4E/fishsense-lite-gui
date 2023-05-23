using FishSenseLiteGUI.ViewModels;
using System;
using System.Windows.Input;

namespace FishSenseLiteGUI.Commands
{
    /// <summary>
    /// Purpose: Command that clears images. Accessed by XAML Button "Delete Images" on Page 2 of the Tab Control.
    /// 
    /// Note: Will also disable pages 2 and 3, clears the past images, and will force the user to choose another 
    ///       directory before using the rest of the application.
    /// </summary>
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
            source.SelectedImageIndex = -1;
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
