using FishSenseLiteGUI.ViewModels;
using System;
using System.Windows.Input;

namespace FishSenseLiteGUI.Commands
{
    internal class PageSwitcherCommand : ICommand
    {
        private MainWindowViewModel source;
        private int PageNumber;

        public PageSwitcherCommand(MainWindowViewModel source, int pageNumber)
        {
            this.source = source;
            this.PageNumber = pageNumber;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            this.source.SelectedTabIndex = this.PageNumber;
        }
    }
}
