using FishSenseLiteGUI.ViewModels;
using System;
using System.Windows.Input;

namespace FishSenseLiteGUI.Commands
{
    /* This command binds to 2 hotkeys on the third page. The user can move to the 
     * next image using 'A' and 'D' to move to the previous image and next image respectively
     * The command obviously cycles, but this behavior is taken care of at the DatabaseModel class
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
            if (!String.IsNullOrEmpty(keyPressed.ToString()))
            {
                if (keyPressed.ToString() == "D")
                {
                    source.databaseModel.NextImage();
                } else if (keyPressed.ToString() == "A")
                {
                    source.databaseModel.PreviousImage();                   
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
