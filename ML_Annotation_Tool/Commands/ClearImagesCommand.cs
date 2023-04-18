using ML_Annotation_Tool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ML_Annotation_Tool.Commands
{
    public class ClearImagesCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            source.selectedTabIndex = 0;
            source.ImageIndex = 0;
            source.NumImages = 0;
            source.fileNames.Clear();

            source.secondPageEnabled = false;
            source.thirdPageEnabled = false;
        }

        MainWindowViewModel source;
        public ClearImagesCommand(MainWindowViewModel source)
        {
            this.source = source;
        }
    }
}
