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
    /* This command binds to 2 hotkeys on the third page. The user can move to the 
     * next image using 'A' and 'D' to move to the previous image and next image respectively
     * The command obviously cycles, but this behavior is taken care of at the DB_Accessor class
     * This command simply notifies the model that it should move to the next class rather than 
     * containing any real logic.
     * 
     * Both the A and D hotkeys link to the same command.
     */
    public class SwitchingImagesCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        // keyPressed is a CommandParameter supplied from the XAMl
        public void Execute(object? keyPressed)
        {
            if (keyPressed.ToString() != null)
            {
                if (keyPressed.ToString() == "D")
                {
                    source.accessor.NextImage();
                } else if (keyPressed.ToString() == "A")
                {
                    source.accessor.PreviousImage();                   
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
