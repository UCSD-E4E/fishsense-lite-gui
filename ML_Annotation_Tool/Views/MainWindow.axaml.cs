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
            Window.WindowState = WindowState.Maximized;
        }

        Point startPoint;
        Point endPoint;

        private void CanvasInitialized(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is Canvas annotationCanvas)
            {
                if (annotationCanvas.DataContext is MainWindowViewModel vm)
                {
                    vm.InitializeCanvas(annotationCanvas);
                }
            }
        }
        private void OnCanvasPointerPressed(object sender, PointerPressedEventArgs e)
        { 
            if (sender is Canvas myCanvas)
            {
                if (myCanvas.DataContext is MainWindowViewModel vm) {
                    var visual = (IVisual)myCanvas;

                    startPoint = e.GetPosition(myCanvas);
                }
            }
        }
        private void OnCanvasPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (sender is Canvas myCanvas)
            {
                if (myCanvas.DataContext is MainWindowViewModel vm)
                {
                    var visual = (IVisual)myCanvas;
                    endPoint = e.GetPosition(myCanvas);

                    int height = (int)myCanvas.Height;
                    int width = (int)myCanvas.Width;
                    
                    if (endPoint.X >= 0 && endPoint.Y >= 0 && endPoint.X <= width && endPoint.Y <= height)
                    {
                        vm.AddAnnotation(startPoint, endPoint);

                    } else
                    {
                        var k = new ErrorMessageBox("Please draw the box on the image itself. Releasing the mouse off of the box will not draw an image.");
                    }
                }
            }
        }

    }
}
