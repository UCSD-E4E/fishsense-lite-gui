using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ML_Annotation_Tool.Commands;
using ML_Annotation_Tool.Data;
using ML_Annotation_Tool.Models;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ML_Annotation_Tool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string FileExplorerButtonText => "Choose directory";

        private int _selectedTabIndex;
        public ObservableCollection<string> fileNames
        {
            get => accessor.FileNames;
        }
        public ObservableCollection<Rectangle> annotations
        {
            get => accessor.CurrentAnnotations();
            set
            {
                accessor.annotations = value;
                OnPropertyChanged(nameof(annotations));
            }
        }
        public int selectedTabIndex
        {
            get { return _selectedTabIndex; }
            set { _selectedTabIndex = value;
                OnPropertyChanged(nameof(selectedTabIndex));
            }
        }

        private readonly string _partOne = "This app is designed to allow you to annotate image features" +
            " and annotate them in a YOLO format. ";
        private readonly string _partTwo = "Press the button below to input the directory that contains" +
            " the files which you want to display. ";
        private readonly string _partThree = "Later, you will have an option to choose where you wish to" +
            " start viewing the files, and which files to display from the directory itself.";

        public StringBuilder description { get; set; }

        private bool _secondPageEnabled = false;
        public bool secondPageEnabled
        {
            get { return _secondPageEnabled; }
            set { 
                 _secondPageEnabled = value; 
                OnPropertyChanged(nameof(secondPageEnabled));
            }  
        }

        private bool _thirdPageEnabled = false;
        public bool thirdPageEnabled
        {
            get { return _thirdPageEnabled; }
            set
            {
                _thirdPageEnabled = value;
                OnPropertyChanged(nameof(thirdPageEnabled));
            }
        }

        private bool _displaySelectedImages = true;
        public bool displaySelectedImages
        {
            get { return _displaySelectedImages; }
            set
            {
                _displaySelectedImages = value;
                OnPropertyChanged(nameof(displaySelectedImages));
                OnPropertyChanged(nameof(removingSelectedImages));
            }
        }
        public bool removingSelectedImages
        {
            get { return !_displaySelectedImages; }
            set
            {
                _displaySelectedImages = value;
            }
        }
        
        public Bitmap ImageToShow => 
            accessor.CurrentBitmap();
        public int ImageIndex
        {
            get { return accessor.ImageIndex; }
            set
            {
                OnPropertyChanged(nameof(ImageToShow));
                OnPropertyChanged(nameof(ImageIndex));
            }
        }
        public ICommand fileExplorer { get; }
        public ICommand nextPage { get; }
        public ICommand showNextImage { get; }
        public ICommand showPreviousImage { get; }
        public ICommand clearImages { get; }
        public ICommand CanvasPointerPressed { get; }

        public DatabaseAccessor accessor;

        public MainWindowViewModel()
        {
            // Create Description String.
            description = new StringBuilder();
            description.Append(_partOne);
            description.Append(_partTwo);
            description.Append(_partThree);

            // Create indexes.
            selectedTabIndex = 0;

            // Initialize commands.
            fileExplorer = new FileExplorerCommand(this);
            nextPage = new NextPageCommand(this);
            showNextImage = new SwitchingImagesCommand(this, "D");
            showPreviousImage = new SwitchingImagesCommand(this, "A");
            clearImages = new ClearImagesCommand(this);
            CanvasPointerPressed = new CanvasPointerPressedCommand(this);

            // Model layer.
            accessor = new DatabaseAccessor(this);

            // Dummy annotations.
            //Rectangle rect1 = new Rectangle()
            //{
            //    Width = 100,
            //    Height = 200,
            //    Stroke = Brushes.Red,
            //    StrokeThickness = 3,
            //    Fill = Brushes.Transparent,
            //};
            //
            //Rectangle rect2 = new Rectangle()
            //{
            //    Width = 300,
            //    Height = 200,
            //    Stroke = Brushes.Red,
            //    StrokeThickness = 3,
            //    Fill = Brushes.Transparent,
            //};
            //
            //Rectangle rect3 = new Rectangle()
            //{
            //    Width = 100,
            //    Height = 500,
            //    Stroke = Brushes.Red,
            //    StrokeThickness = 3,
            //    Fill = Brushes.Transparent,
            //};
            annotations = new ObservableCollection<Rectangle>();
        }

        public void InitializeConnection(string path)
        {
            accessor.SetDatabaseConnection(path);
        }

        public void AddAnnotation(Point topLeft, Point bottomRight, Canvas canvas)
        {
            Rectangle rect = accessor.MakeAnnotation("key", topLeft, bottomRight, canvas);
            annotations.Add(rect);
            Canvas.SetLeft(rect, Math.Min(topLeft.X, bottomRight.X));
            Canvas.SetTop(rect, Math.Min(topLeft.Y, bottomRight.Y));

        }
        public void AddFileName(string path)
        {
            accessor.AddPath(path);
        }

        public void UpdateImageIndex(int newIndex)
        {

        }
        public int ImageCount() { return accessor.Images.Count; }

        public void FilesChosen() { accessor.filesChosen = true; }
    }
}