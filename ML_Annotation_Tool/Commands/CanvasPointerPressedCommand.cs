using ML_Annotation_Tool.ViewModels;
using System;
using System.Windows.Input;

namespace ML_Annotation_Tool.Commands
{
    public class CanvasPointerPressedCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
          
        }

        private MainWindowViewModel source;
        public CanvasPointerPressedCommand(MainWindowViewModel source)
        {
            this.source = source;
        }
    }
}
