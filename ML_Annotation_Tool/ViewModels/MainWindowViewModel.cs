using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using FishSenseLiteGUI.Commands;
using FishSenseLiteGUI.Models;
using FishSenseLiteGUI.SupplementaryClasses;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using static FishSenseLiteGUI.ViewModels.AnnotationIndices;

namespace FishSenseLiteGUI.ViewModels
{
    // Enum to better describe indices to the Annotation Data stored in the database.
    enum AnnotationIndices
    {
        AnnotationDescriptorIndex = 0,
        ImagePathIndex = 1,
        TopLeftXIndex = 2,
        TopLeftYIndex = 3,
        BottomRightXIndex = 4,
        BottomRightYIndex = 5,
    };

    /// <summary>
    /// Purpose: MainWindowViewModel is the ViewModel/Controller of the application. This class manages the model (which in turn 
    ///          reads/writes from the SQLite Database), and creates bindings for the View to bind to. Users can choose images to
    ///          display, create bounding boxes of said images of 3 colors (for the Head/Body/Tail of the fish), and store these 
    ///          in a local SQLite Database.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        public string FileExplorerButtonText => "Choose directory";
        private const int HeightOfTabControl = 100;

        // Navigation is streamlined through use of a Tab Control, the tab number is binded to SelectedTabIndex
        private int selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set
            {
                selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
                OnPropertyChanged(nameof(ImageToShow));
            }
        }

        private int selectedImageIndex;
        public int SelectedImageIndex
        {
            get { return selectedImageIndex; }
            set
            {
                selectedImageIndex = value;
                UpdateAnnotations();
                OnPropertyChanged(nameof(ImageToShow));
                OnPropertyChanged(nameof(DisplayWidth));
                OnPropertyChanged(nameof(DisplayHeight));
            }
        }

        // Filenames are stored in the model.
        public ObservableCollection<string> FileNames
        {
            get
            {
                if (DataAccessor != null)
                {
                    return DataAccessor.getFileNames();
                }
                else
                {
                    return new ObservableCollection<String>();
                }
            }
        }

        // On Startup of the second page, PathSelectedByBox is passed in a null value, hence the necessity for the FileNames.Contains line.
        public string PathSelectedByListBox
        {
            set
            {
                if (FileNames.Contains(value))
                {
                    ThirdPageEnabled = true;
                    SelectedTabIndex++;
                    SelectedImageIndex = FileNames.IndexOf(value);
                }
            }
        }

        /// <summary>
        /// Purpose: Returns the image that will actually be displayed.
        /// 
        /// Note: Only Avalonia.Media.Imaging.Bitmap can be displayed in the Image Control. System.Drawing.Bitmap cannot be displayed, 
        ///       but does contain important image data (width/height in pixels) which is not available in the Avalonia.Media.Imaging.Bitmap
        ///       class. This code will return the necessary bitmap when the xaml will query the viewmodel for it. On startup, the xaml will 
        ///       query ImageToShow and SelectedImageIndex will still be out of bounds. Therefore, we return a dummy image of Fred the Fish, 
        ///       with the understanding that this image should not be visible to the user upon choosing their image to be viewed on Tab 2.
        /// </summary>
        public Avalonia.Media.Imaging.Bitmap ImageToShow
        {
            get
            {
                if (SelectedImageIndex != -1)
                {
                    return new Avalonia.Media.Imaging.Bitmap(FileNames[SelectedImageIndex]);
                }
                else
                {
                    return new Avalonia.Media.Imaging.Bitmap("..\\..\\..\\Assets\\FredTheFish.ico");
                }
            }
        }

        // System.Drawing.Bitmap cannot be displayed (Only Avalonia.Media.Imaging.Bitmap can be displayed). Read the above summary for more info.
        private ObservableCollection<System.Drawing.Bitmap> bitmaps;

        /// <summary>
        /// Purpose: Two Way Binded with the Window's height, and will adjust the size of the image upon window resizing.
        /// 
        /// Note: The Additional OnPropertyChanged calls are added because upon resizing of the height, the size of the Image (both height and width)
        ///       are subject to change. Additionally, if the AnnotationCanvas is created (basically, if the user is on Page 3), the Annotations will 
        ///       be updated to accompany the image resizing.
        /// </summary>
        private double windowHeight;
        public double WindowHeight
        {
            get => windowHeight;
            set
            {
                if (windowHeight != value)
                {
                    windowHeight = value;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DisplayHeight));
                    OnPropertyChanged(nameof(DisplayWidth));

                    if (AnnotationCanvas != null)
                    {
                        UpdateAnnotations();
                    }
                }
            }
        }

        // Read summary above for WindowHeight for more information.
        private double windowWidth;
        public double WindowWidth
        {
            get => windowWidth;
            set
            {
                if (windowWidth != value)
                {
                    windowWidth = value;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DisplayHeight));
                    OnPropertyChanged(nameof(DisplayWidth));

                    if (AnnotationCanvas != null)
                    {
                        UpdateAnnotations();
                    }
                }
            }
        }


        /// <summary>
        /// Purpose: To store and dictate the size of the image (in pixels) as displayed on screen. This is not the same as the Bitmap height 
        ///          of the image, which we refer to as the size of the data stored in the image itself (again in pixels).
        /// </summary>
        public double DisplayHeight
        {
            get
            {
                if (DataAccessor != null)
                {
                    return getDisplayHeight();
                }
                else
                {
                    return 0;
                }
            }
        }

        public double DisplayWidth
        {
            get
            {
                if (DataAccessor != null)
                {
                    return getDisplayWidth();
                }
                else
                {
                    return 0;
                }
            }
        }

        // Constant strings that describe the purpose of the app. Will be expanded in the future when a new GUI is added.
        private const string descriptionLineOne = "This app is designed to allow you to annotate image features and annotate them in a";
        private const string descriptionLineTwo = " YOLO format. Press the button below to input the directory that contains the files";
        private const string descriptionLineThree = " which you want to display. Later, you will have an option to choose where you wish";
        private const string descriptionLineFour = " to start viewing the files, and which files to display from the directory itself.";


        /// <summary>
        /// Purpose: C# Documentation suggests using a Stringbuilder object to represent larger strings. This is initialized
        ///          in the constructor. Description provides a short explanation of the purpose behind the app.
        /// </summary>
        public StringBuilder Description { get; set; }

        /// <summary>
        /// Purpose: Both SecondPageEnabled and ThirdPageEnabled are booleans that dictate whether users can use the Tab buttons in the View
        ///          to display the Second and Third Pages. If images have not been loaded in from a directory (this can mean choosing a 
        ///          directory with invalid/corrupted images), users will not be able to move on to the next page. If the user chooses to clear
        ///          the images (Page 2), they will be unable to access the third page until a new directory is chosen.
        /// </summary>
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
                OnPropertyChanged(nameof(ImageToShow));
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

        /// <summary>
        /// Purpose: To bind button presses on the UI to actions defined in the Commands folder.
        /// 
        /// Note: Please read the summary and implementation of the individual commands for more information.
        ///       FileExplorer             : Opens File Explorer, allows user to choose a directory, uploads valid images, moves user to page 2.
        ///       ClearImages              : Button on Page 2, clears images and resets application back to start page.
        ///       SwitchImage              : Keybinding on page 3, allows user to press 'A' and 'D' to move between images.
        ///       HeadAnnotationDescriptor : Keybinding for 'H' for Head Annotations, and Red is the display color of the box. 
        ///       BodyAnnotationDescriptor : Keybinding for 'T' for Tail Annotations, and Green is the display color of the box. 
        ///       TailAnnotationDescriptor : Keybinding for 'B' for Body Annotations, and Black is the display color of the box. 
        /// </summary>
        public ICommand FileExplorer { get; }

        public ICommand SwitchImage { get; }

        public ICommand ClearImages { get; }

        public ICommand HeadAnnotationDescriptor { get; }

        public ICommand TailAnnotationDescriptor { get; }

        public ICommand BodyAnnotationDescriptor { get; }

        /// <summary>
        /// Purpose: Model layer of the application, stores data and interfaces with Data Layer (Which manages the SQLite Database for the application).
        /// 
        /// Note: Initialized only when a directory is chosen for the database. The Model is reset upon choosing a new directory.
        /// </summary>
        public DataAccessorModel DataAccessor;

        /// <summary>
        /// Purpose: Avalonia's Canvas Control is the chosen method for displaying the Annotations themselves.
        /// 
        /// Note: AnnotationCanvas is undefined until the user reaches Page 3. Upon reaching Page 3:
        ///                 (a) An image is chosen.
        ///                 (b) The canvas over the image is initialized.
        ///                 (c) The canvas is passed into the CanvasInitialized method in the ViewModel and stored in AnnotationCanvas.
        ///                 (d) Future annotations are added as children to AnnotationCanvas.
        /// </summary>
        public Canvas AnnotationCanvas;

        public MainWindowViewModel()
        {
            // Initialize Window Size
            WindowWidth = 1280;
            WindowHeight = 720;

            SelectedImageIndex = -1;

            // Create Description String.
            Description = new StringBuilder();
            Description.Append(descriptionLineOne);
            Description.Append(descriptionLineTwo);
            Description.Append(descriptionLineThree);
            Description.Append(descriptionLineFour);

            // Initializes Bitmaps
            bitmaps = new ObservableCollection<System.Drawing.Bitmap>();

            // Initialize commands.
            FileExplorer = new FileExplorerCommand(this);
            SwitchImage = new SwitchingImagesCommand(this);
            ClearImages = new ClearImagesCommand(this);
            HeadAnnotationDescriptor = new SwitchAnnotationDescriptorCommand(this, "H");
            TailAnnotationDescriptor = new SwitchAnnotationDescriptorCommand(this, "T");
            BodyAnnotationDescriptor = new SwitchAnnotationDescriptorCommand(this, "B");
        }

        // Model is recreated upon switching to new directory.
        public void DirectoryChosen(string directory)
        {
            DataAccessor = new DataAccessorModel(directory);
        }
        
        public void AddImage(string path)
        {
            DataAccessor.AddPath(path);
            bitmaps.Add(new System.Drawing.Bitmap(path));
            OnPropertyChanged(nameof(FileNames));
        }
        /// <summary>
        /// Purpose: Updates Annotations upon drawing a new bounding box, and loading a new image.
        /// 
        /// Note: 
        /// </summary>
        private void UpdateAnnotations()
        {
            // If Statement will execute only if switching between directories and images.
            if (AnnotationCanvas != null)
            {
                AnnotationCanvas.Children.Clear();
                AlignCanvas();
            }

            if (SelectedImageIndex != -1)
            {
                foreach (string[] data in DataAccessor.RequestAnnotationsFromIndex(SelectedImageIndex))
                {
                    int startPointX = (int)Math.Round((Convert.ToDouble(data[(int)TopLeftXIndex]) / getBitmapWidth() * getDisplayWidth()));
                    int startPointY = (int)Math.Round((Convert.ToDouble(data[(int)TopLeftYIndex]) / getBitmapHeight() * getDisplayHeight()));
                    Point startPoint = new Avalonia.Point(startPointX, startPointY);

                    int endPointX = (int)Math.Round((Convert.ToDouble(data[(int)BottomRightXIndex]) / getBitmapWidth() * getDisplayWidth()));
                    int endPointY = (int)Math.Round((Convert.ToDouble(data[(int)BottomRightYIndex]) / getBitmapHeight() * getDisplayHeight()));

                    Point endPoint = new Avalonia.Point(endPointX, endPointY);

                    string AnnotationDescriptor = data[(int)AnnotationDescriptorIndex];

                    if (AnnotationDescriptor == "0")
                    {
                        AddRectangle(startPoint, endPoint, Avalonia.Media.Brushes.Red);
                    }
                    else if (AnnotationDescriptor == "1")
                    {
                        AddRectangle(startPoint, endPoint, Avalonia.Media.Brushes.Green);
                    }
                    else if (AnnotationDescriptor == "2")
                    {
                        AddRectangle(startPoint, endPoint, Avalonia.Media.Brushes.Black);
                    }

                }
            }

        }

        /// <summary>
        /// Purpose: To display bounding boxes using AnnotationCanvas control.
        /// 
        /// Note: Function name is a misnomer, although we are adding a rectangular bounding box, Avalonia Rectangle Controls are
        ///       note defined my a start and endpoint in any way that is feasible to implement. Instead, the solution is to pass 
        ///       around the first and the second point of the rectangle, and create the individual lines between the points.
        /// </summary>
        private void AddRectangle(Point startPoint, Point endPoint, ISolidColorBrush brush)
        {
            Point intermediatePointOne = new Avalonia.Point((int)startPoint.X, (int)endPoint.Y);
            Point intermediatePointTwo = new Avalonia.Point((int)endPoint.X, (int)startPoint.Y);

            Avalonia.Controls.Shapes.Line lineOne = new Avalonia.Controls.Shapes.Line();
            Avalonia.Controls.Shapes.Line lineTwo = new Avalonia.Controls.Shapes.Line();
            Avalonia.Controls.Shapes.Line lineThree = new Avalonia.Controls.Shapes.Line();
            Avalonia.Controls.Shapes.Line lineFour = new Avalonia.Controls.Shapes.Line();

            // Define line data
            lineOne.StrokeThickness = 4;
            lineTwo.StrokeThickness = 4;
            lineThree.StrokeThickness = 4;
            lineFour.StrokeThickness = 4;

            lineOne.Stroke = brush;
            lineTwo.Stroke = brush;
            lineThree.Stroke = brush;
            lineFour.Stroke = brush;

            // Define start and end points for the lines.
            lineOne.StartPoint = startPoint;
            lineOne.EndPoint = intermediatePointOne;

            lineTwo.StartPoint = intermediatePointOne;
            lineTwo.EndPoint = endPoint;

            lineThree.StartPoint = endPoint;
            lineThree.EndPoint = intermediatePointTwo;

            lineFour.StartPoint = intermediatePointTwo;
            lineFour.EndPoint = startPoint;

            // Add lines to Rectangles collection
            this.AnnotationCanvas.Children.Add(lineOne);
            this.AnnotationCanvas.Children.Add(lineTwo);
            this.AnnotationCanvas.Children.Add(lineThree);
            this.AnnotationCanvas.Children.Add(lineFour);
        }

        // Ensures canvas dimensions are that of the image's.
        private void AlignCanvas()
        {
            AnnotationCanvas.Width = getDisplayWidth();
            AnnotationCanvas.Height = getDisplayHeight();
        }

        // As stated above, DisplayHeight/Width refers to the height of the image as displayed.
        private double getDisplayHeight()
        {
            return getBitmapHeight() * GetProportionToDisplay();
        }

        private double getDisplayWidth()
        {
            return getBitmapWidth() * GetProportionToDisplay();
        }

        private double getBitmapHeight()
        {
            if (SelectedImageIndex != -1)
            {
                return bitmaps[selectedImageIndex].Height;
            }
            else
            {
                return 1;
            }
        }

        private double getBitmapWidth()
        {
            if (SelectedImageIndex != -1)
            {
                return bitmaps[selectedImageIndex].Width;
            } 
            else
            {
                return 1;
            }
        }

        private double GetProportionToDisplay()
        {
            double ProportionToDisplay = 0.0;
            if (getBitmapHeight() > (WindowHeight - HeightOfTabControl) || getBitmapWidth() > WindowWidth)
            {
                if (getBitmapHeight() / (WindowHeight - 100) > getBitmapWidth() / windowWidth)
                {
                    ProportionToDisplay = (double)(WindowHeight - 100) / getBitmapHeight();
                }
                else
                {
                    ProportionToDisplay = (double)WindowWidth / getBitmapWidth();
                }
                return ProportionToDisplay;
            }
            else
            {
                return 1;
            }
        }

        // Called on creation of the canvas in MainWindow.axaml.cs.
        public void InitializeCanvas(Canvas annotationCanvas)
        {
            this.AnnotationCanvas = annotationCanvas;
        }

        /// <summary>
        /// Purpose: Translate location of bounding box from displayed pixels (startPoint/endPoint) to the bitmap location of the 
        ///          bounding boxes (regarding the data contained in the image file itself). If the image is not contained within 
        ///          the bounds of the image, an ErrorMessageBox will be displayed notifying the user that their annotation wwas not stored.
        ///          If annotation was successful, Annotation added to Database and UpdateAnnotations is called.
        /// </summary>
        public void AddAnnotation(Point startPoint, Point endPoint)
        {
            int startPointXPixelValue = (int)Math.Round(startPoint.X / AnnotationCanvas.Width * getBitmapWidth());
            int startPointYPixelValue = (int)Math.Round(startPoint.Y / AnnotationCanvas.Height * getBitmapHeight());

            int endPointXPixelValue = (int)Math.Round(endPoint.X / AnnotationCanvas.Width * getBitmapWidth());
            int endPointYPixelValue = (int)Math.Round(endPoint.Y / AnnotationCanvas.Height * getBitmapHeight());

            if ((endPointXPixelValue > getBitmapWidth() || endPointYPixelValue > getBitmapHeight()) || (endPointXPixelValue < 0 || endPointYPixelValue < 0))
            {
                ErrorMessageBox.Show("Attempted to draw a bounding box not located on the image. Please ensure your box begins and ends on the image " +
                    "itself, and not on the surrounding whitespace.");
            }
            else
            {
               DataAccessor.AddAnnotation(AnnotationDescriptor,
                                          System.IO.Path.GetFileName(FileNames[SelectedImageIndex]),
                                          startPointXPixelValue,
                                          startPointYPixelValue,
                                          endPointXPixelValue,
                                          endPointYPixelValue);
                UpdateAnnotations();
            }
        }
    }
}