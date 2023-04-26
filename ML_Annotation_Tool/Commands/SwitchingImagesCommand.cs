using DynamicData;
using ML_Annotation_Tool.ViewModels;
using ML_Annotation_Tool.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ML_Annotation_Tool.Commands
{
    public class SwitchingImagesCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (keyPressed != null)
            {
                if (keyPressed == "D")
                {
                    source.accessor.NextImage();
                } else if (keyPressed == "A")
                {
                    source.accessor.PreviousImage();                   
                }
            }
        }

        private string keyPressed = null;
        private MainWindowViewModel source;
        public SwitchingImagesCommand(MainWindowViewModel source, string keyPressed)
        {
            this.keyPressed = keyPressed;
            this.source = source;
        }
    }
}
