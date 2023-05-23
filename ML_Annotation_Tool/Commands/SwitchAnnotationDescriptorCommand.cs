using FishSenseLiteGUI.ViewModels;
using System;
using System.Windows.Input;

namespace FishSenseLiteGUI.Commands
{
    /// <summary>
    /// Purpose: Allows user to choose different colors based on segment of the object being 
    ///          annotated. 
    ///  
    /// Note: AnnotationDescriptor is the first column of the SQLite Database, and is described by
    ///       the following relationship:
    ///             0 -- Head Annotation -- Red
    ///             1 -- Tail Annotation -- Green
    ///             2 -- Body Annotation -- Black
    ///       This command can only be used from page 3 on the 'H', 'T', and 'B' hotkeys. If the user
    ///       does not choose a specific annotation, Head/Red/0 will be chosen as the default descriptor.
    /// </summary>
    public class SwitchAnnotationDescriptorCommand :ICommand
    {
        MainWindowViewModel source;
        private int AnnotationDescriptor;
        public SwitchAnnotationDescriptorCommand(MainWindowViewModel source, string task)
        {
            this.source = source;
            if (task == "H")
            {
                AnnotationDescriptor = 0;
            } 
            else if (task == "T")
            {
                AnnotationDescriptor = 1;
            }
            else if (task == "B")
            {
                AnnotationDescriptor = 2;
            }
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            source.AnnotationDescriptor = AnnotationDescriptor;
        }
    }
}
