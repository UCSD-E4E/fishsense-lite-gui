using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using ML_Annotation_Tool.Models;
using ML_Annotation_Tool.ViewModels;
using System;

namespace ML_Annotation_Tool.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
        }

        Point startPoint;
        Point endPoint;
        private void OnImagePointerPressed(object sender, PointerPressedEventArgs e)
        { 
            if (sender is Image myImage)
            {
                if (myImage.DataContext is MainWindowViewModel vm) {
                    var visual = (IVisual)myImage;

                    startPoint = e.GetPosition(myImage);
                }
            }

        }
        private void OnImagePointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (sender is Image myImage)
            {
                if (myImage.DataContext is MainWindowViewModel vm)
                {
                    var visual = (IVisual)myImage;
                    endPoint = e.GetPosition(myImage);
                    var transformedBounds = DisplayedImage.TransformedBounds.Value;
                    int height = (int)DisplayedImage.Height;
                    int width = (int)DisplayedImage.Width;
                    //int height = (int)transformedBounds.Bounds.Height;
                    //int width = (int)transformedBounds.Bounds.Height;
                    var h = new ErrorMessageBox("Width: " + width + "\nHeight: " + height + " \n X: " + endPoint.X.ToString() + " Y: " + endPoint.Y.ToString());
                    if (endPoint.X >= 0 && endPoint.Y >= 0 && endPoint.X <= width && endPoint.Y <= height)
                    {
                        vm.AddAnnotation(startPoint, endPoint, width, height);
                    } else
                    {
                        var k = new ErrorMessageBox("Please draw the box on the image itself. Releasing the mouse off of the box will not draw an image.");
                    }
                }
            }
        }

    }
}
