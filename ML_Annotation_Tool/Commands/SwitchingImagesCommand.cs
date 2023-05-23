using Avalonia.Controls.Primitives;
using FishSenseLiteGUI.ViewModels;
using System;
using System.Windows.Input;

namespace FishSenseLiteGUI.Commands
{
    /// <summary>
    /// Purpose: This command allows users to switch between images using the 'A' and 'D' hotkeys for 
    ///          left and right respectively. 
    ///          
    /// Note: This command can only be used from the third page. Upon moving right or left, the Viewmodel's
    ///       SelectedImageIndex property is incremented, which updates the Image displayed and all relevant
    ///       properties. To avoid creating multiple commands, both 'A' and 'D' hotkeys access the same instance,
    ///       where they differ only by the keyPressed command parameter passed in from the xaml.
    /// </summary>
    public class SwitchingImagesCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? keyPressed)
        {
            if (!String.IsNullOrEmpty(keyPressed.ToString()))
            {
                if (keyPressed.ToString() == "D")
                {
                    source.SelectedImageIndex = (source.SelectedImageIndex + 1) % source.FileNames.Count;
                } 
                else if (keyPressed.ToString() == "A")
                {
                    if (source.SelectedImageIndex == 0)
                    {
                        source.SelectedImageIndex = source.FileNames.Count - 1;
                    }                  
                    else
                    {
                        source.SelectedImageIndex--;
                    }
                }
            }
        }

        private MainWindowViewModel source;
        public SwitchingImagesCommand(MainWindowViewModel source)
        {
            this.source = source;
        }
    }
}
