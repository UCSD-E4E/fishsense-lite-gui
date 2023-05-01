using ML_Annotation_Tool.Commands;
using ML_Annotation_Tool.Models;
using System.Collections.ObjectModel;
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
                OnPropertyChanged(nameof(FileNames));
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
                ImageHeight = accessor.getHeight();
                ImageWidth = accessor.getWidth();
            }
        }

        // Another method to display images properly in xaml. Height and Width is necessary to add annotations using the values of 
        // the exact pixels of the image.

        private int _imageHeight;
        public int ImageHeight
        {
            get => _imageHeight;
            set
            {
                _imageHeight = value;
                OnPropertyChanged(nameof(ImageHeight));
            }
        }
        private int _imageWidth;
        public int ImageWidth
        {
            get => _imageWidth;
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

        private bool _displaySelectedImages = true;
        public bool DisplaySelectedImages
        {
            get { return _displaySelectedImages; }
            set
            {
                _displaySelectedImages = value;
                OnPropertyChanged(nameof(DisplaySelectedImages));
                OnPropertyChanged(nameof(RemovingSelectedImages));
            }
        }
        public bool RemovingSelectedImages
        {
            get { return !_displaySelectedImages; }
            set
            {
                _displaySelectedImages = value;
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

        public ICommand HeadAnnotationDescriptor { get; }
        public ICommand TailAnnotationDescriptor { get; }
        public ICommand BodyAnnotationDescriptor { get; }

        public DB_Accessor accessor;
        public MainWindowViewModel()
        {
            // Create Description String.
            Description = new StringBuilder();
            Description.Append(_sentenceOne);
            Description.Append(_sentenceTwo);
            Description.Append(_sentenceThree);

            // Initialize the filenames collection.
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
        // This method is called from the code behind, and this method just routes the data from the code behind to the model.
        public void AddAnnotation(Avalonia.Point startPoint, Avalonia.Point endPoint, int width, int height)
        {
            accessor.AddAnnotation(AnnotationDescriptor, startPoint, endPoint, width, height);
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
    }
}