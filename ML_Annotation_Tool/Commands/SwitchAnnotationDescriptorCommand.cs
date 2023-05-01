using ML_Annotation_Tool.ViewModels;
using System;
using System.ComponentModel.Design;
using System.Windows.Input;

namespace ML_Annotation_Tool.Commands
{ 
    /* Annotation Descriptor: The first line stored in the SQLite database itself.
     * Denotes whether the annotation is a (0) Head Annotation, (1) Tail Annotation,
     * or (2) Body Annotation, each corresponding to their respective initials.
     * 
     * This command is tied to hotkeys on Page 3. Pressing each initial will immediately 
     * execute its command and ensure users can draw their rectangles.
     */
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
