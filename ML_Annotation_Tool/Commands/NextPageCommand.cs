using Avalonia.Controls;
using ML_Annotation_Tool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ML_Annotation_Tool.Commands
{
    public class NextPageCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            source.selectedIndex += 1;
            source.selectedIndex %= 3;
            source.thirdPageEnabled = true;
        }

        private MainWindowViewModel source;
        public NextPageCommand(MainWindowViewModel source)
        {
            this.source = source;
        }
    }
}
