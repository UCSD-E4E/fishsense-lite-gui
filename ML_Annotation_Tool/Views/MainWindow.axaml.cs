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
                        //data[0] = LEFT;
                        //vm.MousePress.Execute(data);
                        var k = new ErrorMessageBox("Testing!");
                    }
                    else if (e.MouseButton == MouseButton.Right)
                    {
                        //data[0] = RIGHT;
                        //vm.MousePress.Execute(data);
                    }
                }
            }

        }
        Point startPoint;
        Point endPoint;
        private void CanvasPointerPressed(object sender, PointerPressedEventArgs e)
        {
            startPoint = e.GetPosition((IVisual?)sender);
        }
        private void OnCanvasReleased(object sender, PointerReleasedEventArgs e)
        {
            var k = new ErrorMessageBox("Testing!");
            endPoint = e.GetPosition((IVisual?)sender);
            if (sender is Canvas canvas)
            {
                if (canvas.DataContext is MainWindowViewModel vm)
                {
                    vm.AddAnnotation(startPoint, endPoint, canvas);
                }
            }
            Point topRight = new Point(endPoint.X, startPoint.Y);
            Point bottomLeft = new Point(startPoint.X, endPoint.Y);
            Line l1 = new Line();
            Line l2 = new Line();
            Line l3 = new Line();
            Line l4 = new Line(); // make a new rectangle class of my own that returns 4 line objects when queried

            l1.StrokeThickness = 4;
            l1.Stroke = new SolidColorBrush(Colors.Blue);
            l2.StrokeThickness = 4;
            l2.Stroke = new SolidColorBrush(Colors.Blue);
            l3.StrokeThickness = 4;
            l3.Stroke = new SolidColorBrush(Colors.Blue);
            l4.StrokeThickness = 4;
            l4.Stroke = new SolidColorBrush(Colors.Blue);

            l1.StartPoint = startPoint;
            l1.EndPoint = topRight;

            l2.StartPoint = topRight;
            l2.EndPoint = endPoint;

            l3.StartPoint = startPoint;
            l3.EndPoint = bottomLeft;

            l4.StartPoint = bottomLeft;
            l4.EndPoint = endPoint;

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
            AnnotationCanvas.Children.Add(l1);
            AnnotationCanvas.Children.Add(l2);
            AnnotationCanvas.Children.Add(l3);
            AnnotationCanvas.Children.Add(l4);
        }
        private void ClearAnnotations(object sender, RoutedEventArgs e)
        {
            AnnotationCanvas.Children.Clear();
        }

    }
}
