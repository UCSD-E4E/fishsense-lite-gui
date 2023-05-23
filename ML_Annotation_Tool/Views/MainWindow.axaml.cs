using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using FishSenseLiteGUI.ViewModels;

namespace FishSenseLiteGUI.Views
{
    public partial class MainWindow : Window
    {
        Point startPoint;
        Point endPoint;
        Canvas? myCanvas;
        MainWindowViewModel? myCanvasDataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

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
                    startPoint = e.GetPosition(myCanvas);
                }
            }
        }

        private void OnCanvasPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            myCanvas = sender as Canvas;
            myCanvasDataContext = myCanvas.DataContext as MainWindowViewModel;

            //startPoint was defined in OnCanvasPointerPressed
            endPoint = e.GetPosition(myCanvas);

            myCanvasDataContext.AddAnnotation(startPoint, endPoint);
        }

    }
}
