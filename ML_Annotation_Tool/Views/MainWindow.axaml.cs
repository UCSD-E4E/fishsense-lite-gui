using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using JetBrains.Annotations;
using ML_Annotation_Tool.ViewModels;
using System;
using System.Security;

namespace ML_Annotation_Tool.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnImagePressed(object sender, PointerPressedEventArgs e)
        {
            double[] data = new double[7];
            int LEFT = 0;
            int RIGHT = 1;
            if (sender is Image myImage)
            {
                if (myImage.DataContext is MainWindowViewModel vm) {
                    var visual = (IVisual)myImage;

                    data[1] = e.GetPosition(myImage).X;
                    data[2] = e.GetPosition(myImage).Y;
                    data[3] = visual.Bounds.Width;
                    data[4] = visual.Bounds.Height;
                    data[5] = ((Bitmap)myImage.Source).PixelSize.Width;
                    data[6] = ((Bitmap)myImage.Source).PixelSize.Height;
                    if (e.MouseButton == MouseButton.Left)
                    {
                        data[0] = LEFT;
                        vm.MousePress.Execute(data);
                    }
                    else if (e.MouseButton == MouseButton.Right)
                    {
                        data[0] = RIGHT;
                        vm.MousePress.Execute(data);
                    }
                }
            }

        }
        Point startPoint;
        private void OnCanvasPressed(object sender, PointerPressedEventArgs e)
        {
            startPoint = e.GetPosition((IVisual?)sender);
        }

        Point endPoint;
        private void OnCanvasReleased(object sender, PointerReleasedEventArgs e)
        {
            endPoint = e.GetPosition((IVisual?)sender);
            double width = Math.Abs(endPoint.X - startPoint.X);
            double height = Math.Abs(endPoint.Y - startPoint.Y);

            Rectangle rect = new Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = Brushes.Red,
                StrokeThickness = 3,
                Fill = Brushes.Transparent,
            };

            Canvas.SetLeft(rect, Math.Min(startPoint.X, endPoint.X));
            Canvas.SetTop(rect, Math.Min(startPoint.Y, endPoint.Y));
            AnnotationCanvas.Children.Add(rect);
        }
    }
}
