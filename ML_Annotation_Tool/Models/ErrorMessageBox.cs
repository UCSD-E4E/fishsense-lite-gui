using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Annotation_Tool.Models
{
    public class ErrorMessageBox : Window
    {
        public ErrorMessageBox(string message)
        {
            var w = new Window();
            w.Content= message;
            w.Show();
        }
    }
}
