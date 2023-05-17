using Avalonia.Controls;
using FishSenseLiteGUI.Commands;
using FishSenseLiteGUI.Models;
using FishSenseLiteGUI.SupplementaryClasses;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace FishSenseLiteGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string FileExplorerButtonText => "Choose directory";

        // Index for the tab of the UI that is displayed. 
        //[ObservableObject]
        //[NotifyPropertyChangedFor(nameof(selectedTabIndex))]
        private int selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set
            {
                selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        private ObservableCollection<string> fileNames;
        public ObservableCollection<string> FileNames
        {
            get => fileNames;
            set
            {
                fileNames = value;
                OnPropertyChanged();
            }
        }

        private string pathForImageToShow;
        public string PathForImageToShow
        {
            get => pathForImageToShow;
            set
            {
                pathForImageToShow = value;
                if (NextPage != null)
                {
                    NextPage.Execute("");
                }
            }
        }

        // Contains Avalonia.Media.Imaging.Bitmap that will be displayed in UI
        private Avalonia.Media.Imaging.Bitmap imageToShow;
        public Avalonia.Media.Imaging.Bitmap ImageToShow
        {
            get => imageToShow;
            set
            {
                imageToShow = value;
                OnPropertyChanged(nameof(ImageToShow));
                OnPropertyChanged(nameof(ImageHeight));
                OnPropertyChanged(nameof(ImageWidth));
            }
        }

        private WindowState state;
        public WindowState State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged();
                }
            }
        }

        private double windowHeight;
        public double WindowHeight
        {
            get => windowHeight;
            set
            {
                if (windowHeight != value )
                {
                    windowHeight = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ImageHeight));
                    OnPropertyChanged(nameof(ImageWidth));
                }
            }
        }

        public int ImageHeight
        {
            get
            {
                if (databaseModel != null)
                {
                    return databaseModel.getHeight(WindowHeight, WindowWidth);
                } 
                else
                {
                    return 0;
                }
            }
        }

        private double windowWidth;
        public double WindowWidth
        {
            get => windowWidth;
            set
            {
                if (windowWidth != value )
                {
                    windowWidth = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ImageHeight));
                    OnPropertyChanged(nameof(ImageWidth));
                }
            }
        }

        public int ImageWidth
        {
            get
            {
                if (databaseModel != null)
                {
                    return databaseModel.getWidth(WindowHeight, WindowWidth);
                } 
                else
                {
                    return 0;
                }
            }
        }

        private const string sentenceOne = "This app is designed to allow you to annotate image features" +
            " and annotate them in a YOLO format. ";
        private const string sentenceTwo = "Press the button below to input the directory that contains" +
            " the files which you want to display. ";
        private const string sentenceThree = "Later, you will have an option to choose where you wish to" +
            " start viewing the files, and which files to display from the directory itself.";

        // C# documentation recommended using StringBuilder to represent larger strings, and this is initialized in the 
        // constructor. This data is to add a small description in the beginning of the app.
        public StringBuilder Description { get; set; }

        // Booleans that control the use of certain buttons and whether or not users can view the 2nd or 3rd tabs in the app.
        private bool secondPageEnabled = false;
        public bool SecondPageEnabled
        {
            get { return secondPageEnabled; }
            set
            {
                secondPageEnabled = value;
                OnPropertyChanged(nameof(SecondPageEnabled));
            }
        }

        private bool thirdPageEnabled = false;
        public bool ThirdPageEnabled
        {
            get { return thirdPageEnabled; }
            set
            {
                thirdPageEnabled = value;
                OnPropertyChanged(nameof(ThirdPageEnabled));
            }
        }

        // Descriptor that is updated by the SwitchAnntationDescriptor.cs class.
        private int annotationDescriptor;
        public int AnnotationDescriptor
        {
            get { return annotationDescriptor; }
            set
            {
                annotationDescriptor = value;
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

        public DatabaseModel databaseModel;

        public MainWindowViewModel()
        {
            // Initialize Window Size
            WindowWidth = 1280;
            WindowHeight = 720;
            // GitHub issue with this bug: https://github.com/AvaloniaUI/Avalonia/pull/9221. 
            // Using maximized in the code behind instead.
            State = WindowState.Maximized; 

            // Create Description String.
            Description = new StringBuilder();
            Description.Append(sentenceOne);
            Description.Append(sentenceTwo);
            Description.Append(sentenceThree);

            // Initialize the private Observable Collections
            fileNames = new ObservableCollection<string>();
            
            // Create indexes.
            SelectedTabIndex = 0;
            PathForImageToShow = String.Empty;
                
            // Initialize commands.
            FileExplorer = new FileExplorerCommand(this);
            NextPage = new MoveToThirdPageCommand(this);
            SwitchImage = new SwitchingImagesCommand(this);
            ClearImages = new ClearImagesCommand(this);
            HeadAnnotationDescriptor = new SwitchAnnotationDescriptorCommand(this, "H");
            TailAnnotationDescriptor = new SwitchAnnotationDescriptorCommand(this, "T");
            BodyAnnotationDescriptor = new SwitchAnnotationDescriptorCommand(this, "B");
        }

        public void InitializeModelLayer(string path)
        {
            // Model layer.
            databaseModel = new DatabaseModel(path, this);
        }
        
        public void AddAnnotation(Avalonia.Point startPoint, Avalonia.Point endPoint, double canvasHeight, double canvasWidth)
        {
            // This method is called from the code behind, and this method just routes the data from the code behind to the model.
            if (endPoint.X >= 0 && endPoint.Y >= 0 && endPoint.X <= canvasWidth && endPoint.Y <= canvasHeight)
            {
                databaseModel.AddAnnotation(AnnotationDescriptor, 
                                       startPoint, 
                                       endPoint, 
                                       Convert.ToInt32(Math.Round(canvasWidth)), 
                                       Convert.ToInt32(Math.Round(canvasHeight)));
            } 
            else
            {
                ErrorMessageBox.Show("Please draw the box on the image itself. Releasing the mouse off of the box will not draw an image.");
            }
            
        }
        public void AddImage(string imagePath)
        {
            FileNames.Add(imagePath);
            databaseModel.AddImage(imagePath);
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

            databaseModel.ChooseFirstImage(PathForImageToShow);
        }

        public void InitializeCanvas(Canvas annotationCanvas)
        {
            databaseModel.AddCanvas(annotationCanvas, (int)WindowHeight, (int)WindowWidth);
        }
    }
}