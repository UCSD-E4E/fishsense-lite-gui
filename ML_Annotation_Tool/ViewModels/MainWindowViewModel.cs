using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ML_Annotation_Tool.Commands;
using ML_Annotation_Tool.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace ML_Annotation_Tool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string FileExplorerButtonText => "Choose directory";

        // Index for the tab of the UI that is displayed. 
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        private ObservableCollection<string> _fileNames;
        public ObservableCollection<string> FileNames
        {
            get => _fileNames;
            set
            {
                _fileNames = value;
                OnPropertyChanged();
            }
        }
        // Contains Avalonia.Media.Imaging.Bitmap that will be displayed in UI
        private Avalonia.Media.Imaging.Bitmap _imageToShow;
        public Avalonia.Media.Imaging.Bitmap ImageToShow
        {
            get => _imageToShow;
            set
            {
                _imageToShow = value;
                OnPropertyChanged(nameof(ImageToShow));
                OnPropertyChanged(nameof(ImageHeight));
                OnPropertyChanged(nameof(ImageWidth));
            }
        }

        // Another method to display images properly in xaml. Height and Width is necessary to add annotations using the values of 
        // the exact pixels of the image. Figure out why the WindowState.Maximized binding doesn't work (Window doesn't start maximized)

        private WindowState _state;
        public WindowState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _windowHeight;
        public double WindowHeight
       {
           get => _windowHeight;
           set
           {
               if (_windowHeight != value )
               {
                   _windowHeight = value;
                   OnPropertyChanged();
                   OnPropertyChanged(nameof(ImageHeight));
                   OnPropertyChanged(nameof(ImageWidth));
               }
           }
       }
        private int _imageHeight;
        public int ImageHeight
        {
            get
            {
                if (accessor != null)
                {
                    return accessor.getHeight(WindowHeight, WindowWidth);
                } 
                else
                {
                    return 0;
                }
            } 
            set
            {
                _imageHeight = value;
                OnPropertyChanged();
            }
        }
        private double _windowWidth;
        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                if (_windowWidth != value )
                {
                    _windowWidth = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ImageHeight));
                    OnPropertyChanged(nameof(ImageWidth));
                }
            }
        }
        private int _imageWidth;
        public int ImageWidth
        {
            get
            {
                if (accessor != null)
                {
                    return accessor.getWidth(WindowHeight, WindowWidth);
                } 
                else
                {
                    return 0;
                }
            }
            set
            {
                _imageWidth = value;
                OnPropertyChanged(nameof(ImageWidth));
            }
        }

        private readonly string _sentenceOne = "This app is designed to allow you to annotate image features" +
            " and annotate them in a YOLO format. ";
        private readonly string _sentenceTwo = "Press the button below to input the directory that contains" +
            " the files which you want to display. ";
        private readonly string _sentenceThree = "Later, you will have an option to choose where you wish to" +
            " start viewing the files, and which files to display from the directory itself.";

        // C# documentation recommended using StringBuilder to represent larger strings, and this is initialized in the 
        // constructor. This data is to add a small description in the beginning of the app.
        public StringBuilder Description { get; set; }

        // Booleans that control the use of certain buttons and whether or not users can view the 2nd or 3rd tabs in the app.

        private bool _secondPageEnabled = false;
        public bool SecondPageEnabled
        {
            get { return _secondPageEnabled; }
            set
            {
                _secondPageEnabled = value;
                OnPropertyChanged(nameof(SecondPageEnabled));
            }
        }

        private bool _thirdPageEnabled = false;
        public bool ThirdPageEnabled
        {
            get { return _thirdPageEnabled; }
            set
            {
                _thirdPageEnabled = value;
                OnPropertyChanged(nameof(ThirdPageEnabled));
            }
        }

        // Descriptor that is updated by the SwitchAnntationDescriptor.cs class.
        private int _annotationDescriptor;
        public int AnnotationDescriptor
        {
            get { return _annotationDescriptor; }
            set
            {
                _annotationDescriptor= value;
                OnPropertyChanged(nameof(AnnotationDescriptor));
            }
        }
        // Each of these commands are binded to buttons and events in the UI and forms a way to pass data between the layers and the UI.
        public ICommand FileExplorer { get; }
        public ICommand NextPage { get; }
        public ICommand SwitchImage { get; }
        public ICommand ClearImages { get; }

        // Individual commands that switch the Annotation Descriptor for following annotations.
        // H = Head = Red, B = Body = Black, T = Tail = Green.
        public ICommand HeadAnnotationDescriptor { get; }
        public ICommand TailAnnotationDescriptor { get; }
        public ICommand BodyAnnotationDescriptor { get; }

        public DB_Accessor accessor;
        public MainWindowViewModel()
        {
            // Initialize Window Size
            WindowWidth = 1280;
            WindowHeight = 720;
            State = WindowState.Maximized; // Does nothing to change size of window after the values above.

            // Create Description String.
            Description = new StringBuilder();
            Description.Append(_sentenceOne);
            Description.Append(_sentenceTwo);
            Description.Append(_sentenceThree);

            // Initialize the private Observable Collections
            _fileNames = new ObservableCollection<string>();
            
            // Create indexes.
            SelectedTabIndex = 0;

            // Initialize commands.
            FileExplorer = new FileExplorerCommand(this);
            NextPage = new MoveToThirdPageCommand(this);
            SwitchImage = new SwitchingImagesCommand(this);
            ClearImages = new ClearImagesCommand(this);
            HeadAnnotationDescriptor = new SwitchAnnotationDescriptorCommand(this, "H");
            TailAnnotationDescriptor = new SwitchAnnotationDescriptorCommand(this, "T");
            BodyAnnotationDescriptor = new SwitchAnnotationDescriptorCommand(this, "B");
        }

        public void InitializeConnection(string path)
        {
            // Model layer.
            accessor = new DB_Accessor(path, this);
        }
        
        public void AddAnnotation(Avalonia.Point startPoint, Avalonia.Point endPoint)
        {
            // This method is called from the code behind, and this method just routes the data from the code behind to the model.
            accessor.AddAnnotation(AnnotationDescriptor, startPoint, endPoint, ImageWidth, ImageHeight);
        }
        public void AddFileName(string imagePath)
        {
            FileNames.Add(imagePath);
            accessor.AddImage(imagePath);
        }
        internal void ImageToShowEdited(Avalonia.Media.Imaging.Bitmap newImage)
        {
            ImageToShow = newImage; 
        }

        internal void InitializeImage()
        {
            // Currently using a "" just because no initial image feature has been created, but this will 
            // will be added in the future. "" doesn't share any of the same image path names and so it will
            // choose the first image for its 
            accessor.ChooseFirstImage("");
        }

        internal void InitializeCanvas(Canvas annotationCanvas)
        {
            accessor.AddCanvas(annotationCanvas, (int)WindowHeight, (int)WindowWidth);
        }
    }
}