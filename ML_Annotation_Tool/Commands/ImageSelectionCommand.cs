using ML_Annotation_Tool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ML_Annotation_Tool.Commands
{
    public class ImageSelectionCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if ((string)(parameter) == "display_images")
            {
                source.displaySelectedImages = true;
            } else if ((string)(parameter) == "remove_images")
            {
                source.displaySelectedImages = false;
            }
        }

        MainWindowViewModel source;
        public ImageSelectionCommand(MainWindowViewModel source)
        {
            this.source = source;
        }
    }
}
