using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using ML_Annotation_Tool.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ML_Annotation_Tool.Commands
{
    public class MousePressCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
          // // ind 0 LEFT or RIGHT (0 OR 1) 
          // // ind 1 MOUSE X COORD
          // // ind 2 MOUSE Y COORD
          // // ind 3 IMAGE WIDTH
          // // ind 4 IMAGE HEIGHT
          // double[] data = (double[])parameter;
          // double x_coord = data[1];
          // double y_coord = data[2];
          // double rendered_width = data[3];
          // double rendered_height = data[4];
          // double pixel_width = data[5];
          // double pixel_height = data[6];
          //
          // //Bitmap baseImage = new Bitmap(source.fileNames[source.ImageIndex]);
        }

        private MainWindowViewModel source;
        public MousePressCommand(MainWindowViewModel source)
        {
            this.source = source;
        }
    }
}
