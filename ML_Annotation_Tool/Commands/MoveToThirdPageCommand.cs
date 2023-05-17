using FishSenseLiteGUI.ViewModels;
using System;
using System.Windows.Input;

namespace FishSenseLiteGUI.Commands
{
    /* Command to move to the next page. 
     * Currently only linked to a button on the xaml of Page 2 to move to page 3.
     */
    public class MoveToThirdPageCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            source.SelectedTabIndex += 1;
            source.SelectedTabIndex %= 3;
            source.ThirdPageEnabled = true;

            source.InitializeImage();
        }

        private MainWindowViewModel source;
        public MoveToThirdPageCommand(MainWindowViewModel source)
        {
            this.source = source;
        }
    }
}
