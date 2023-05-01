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

        public async void Execute(object? parameter)
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
