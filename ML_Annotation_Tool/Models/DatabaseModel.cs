using Avalonia.Controls;
using Avalonia.Media;
using FishSenseLiteGUI.Data;
using FishSenseLiteGUI.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static FishSenseLiteGUI.Models.AnnotationIndices;

namespace FishSenseLiteGUI.Models
{
    // Enum to better describe indices to the Annotation Data stored in the database.
    enum AnnotationIndices {
        AnnotationDescriptorIndex = 0,
        ImagePathIndex = 1,
        TopLeftXIndex = 2,
        TopLeftYIndex = 3,
        BottomRightXIndex = 4,
        BottomRightYIndex = 5,
    };

    /// <summary>
    /// This class is the bulk of the model layer, and it connects the data layer to the viewmodel
    /// layer. It stores a Database object, the bitmaps of the images themselves (using a custom
    /// EditableBitmap class), and allows users to update the UI with new images, add annotations,
    /// add images, delete images, and move between images
    /// </summary>

    public class DatabaseModel
    {
        // Private instance variables that store the necessary connections to access data and invoke methods.
        private Database db;
        private MainWindowViewModel source;
        private Bitmap OriginalImage;

        // Contains custom EditableBitmap class that contains method to add annotations, pixel by pixel to the bitmaps
        private ObservableCollection<Bitmap> bitmaps;
        private ObservableCollection<string> fullPaths;

        private int windowHeight;
        private int windowWidth;

        private const int HeightOfTabControl = 100;

        private int imageIndex;
        public int ImageIndex
        {
            get { return imageIndex; }
            set 
            { 
                imageIndex = value;
                // Notifies UI that the image index has been updated.
                ImageUpdated();                
            }
        }

        public Canvas AnnotationCanvas { get; set; }

        // Creates database object using passed in path, and stores view model to access data.
        public DatabaseModel(string path, MainWindowViewModel source)
        {
            db = new Database(path);
            this.source = source;
            bitmaps = new ObservableCollection<Bitmap>();
            fullPaths = new ObservableCollection<string>();

            // For now, initializes image index to 0. Will later order list in alphabetical order.
            imageIndex = 0;   
        }
        public void ImageUpdated()
        {
            // Adds annnotations that were previously stored in database, and clear previous image's annotations.
            AnnotationCanvas.Children.Clear();
            AddPreviousAnnotations();

             /* Converts image from System.Drawing.Bitmap to Avalonia.Media.Imaging.Bitmap.
             *  System.Drawing.Bitmap allows for pixel by pixel editing, but only Avalonia.Media.Imaging.Bitmap
             *  Can be displayed using the Image Control, so the image is pre-converted.
             */
            using (MemoryStream memory = new MemoryStream())
            {
                //https://www.appsloveworld.com/opencv/100/26/is-it-possible-to-create-avalonia-media-imaging-bitmap-from-system-drawing-bitmap
                // Loads image into memory stream. This is done to store the data because you can't just cast
                // from one type to the other.
                OriginalImage.Save(memory, ImageFormat.Png);

                // Could be unnecessary line, should research the purpose of it further, but the tutorial used it.
                memory.Position = 0;
                
                // Simple method that just updates the binded image in the UI
                source.ImageToShowEdited(new Avalonia.Media.Imaging.Bitmap(memory));
            }
        }

        // Called in ImageUpdated, just queries database for past annotations and adds them to the 
        // bitmap to display in the UI.
        private void AddPreviousAnnotations()
        {
            string AnnotationDescriptor;
            foreach (string[] data in db.RequestAnnotationsForPath(Path.GetFileName(fullPaths[ImageIndex])))
            {
                int startPointX = (int)(Convert.ToDouble(data[(int)TopLeftXIndex]) / OriginalImage.Width * getWidth(windowHeight, windowWidth));
                int startPointY = (int)(Convert.ToDouble(data[(int)TopLeftYIndex]) / OriginalImage.Height * getHeight(windowHeight, windowWidth));
                Avalonia.Point startPoint = new Avalonia.Point(startPointX, startPointY);

                int endPointX = (int)(Convert.ToDouble(data[(int)BottomRightXIndex]) /  OriginalImage.Width * getWidth(windowHeight, windowWidth));
                int endPointY = (int)(Convert.ToDouble(data[(int)BottomRightYIndex]) / OriginalImage.Height * getHeight(windowHeight, windowWidth));
                Avalonia.Point endPoint = new Avalonia.Point(endPointX, endPointY);

                AnnotationDescriptor = data[(int)AnnotationDescriptorIndex];
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
        
        // Added path to choose beginning image. Not used currently, but
        // added for future implementation. If no proper filename is passed in,
        // just displays first image.
        public void ChooseFirstImage(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                foreach (string ImageFileName in getFileNames())
                {
                    if (ImageFileName.Equals(fileName))
                    {
                        OriginalImage = bitmaps[getFileNames().IndexOf(ImageFileName)]; 
                        ImageIndex = getFileNames().IndexOf(ImageFileName);
                    }
                }
            } else
            {
                OriginalImage = bitmaps[0];
                ImageIndex = 0;
            }
        }

        public void AddImage(string fullPath)
        {
            bitmaps.Add(new Bitmap(fullPath));
            fullPaths.Add(fullPath);
        }

        // Adds the annotation to the database and updates the UI with newly annotated image.
        public void AddAnnotation(int AnnotationDescriptor, Avalonia.Point firstPoint, Avalonia.Point secondPoint, int width, int height)
        {
            //AddRectangle(firstPoint, secondPoint);

            // Finds min and max to see which is the top left and which is the bottom right.
            double topLeftX = Math.Min(firstPoint.X, secondPoint.X);
            double topLeftY = Math.Min(firstPoint.Y, secondPoint.Y);
            double bottomRightX = Math.Max(firstPoint.X, secondPoint.X); 
            double bottomRightY = Math.Max(firstPoint.Y, secondPoint.Y);

            // Adds data to database.
            db.InsertData(AnnotationDescriptor.ToString(), System.IO.Path.GetFileName(fullPaths[ImageIndex]),
                Convert.ToInt32(topLeftX / width * OriginalImage.Width), 
                Convert.ToInt32(topLeftY / height * OriginalImage.Height), 
                Convert.ToInt32(bottomRightX / width * OriginalImage.Width), 
                Convert.ToInt32(bottomRightY / height * OriginalImage.Height));

            // Notifies UI that the Image has been upadted.
            ImageUpdated();
        }

        public Bitmap getCurrentImage() 
        {  
            if (OriginalImage == null)
            {
                if (bitmaps.Count > 0)
                {
                    OriginalImage = bitmaps[0];
                }
            }
            return OriginalImage;
        }
        public ObservableCollection<string> getFileNames()
        {
            return fullPaths;
        }

        public void ClearImages()
        {
            bitmaps.Clear();
            fullPaths.Clear();
        }

        internal void NextImage()
        {
            int ind = (ImageIndex + 1) % bitmaps.Count;
            OriginalImage = bitmaps[ind];
            ImageIndex = ind;

        }

        internal void PreviousImage()
        {
            int ind = (ImageIndex - 1);
            if (ind < 0)
            {
                ind = bitmaps.Count - 1;
            }
            OriginalImage = bitmaps[ind];

            ImageIndex = ind;
        }

        public int getHeight(double windowHeight, double windowWidth)
        {
            return (int)(OriginalImage.Height * GetProportionToDisplay(windowHeight, windowWidth));
        }

        public int getWidth(double windowHeight, double windowWidth)
        {
            return (int)(OriginalImage.Width * GetProportionToDisplay(windowHeight, windowWidth) );
        }

        private double GetProportionToDisplay(double windowHeight, double windowWidth)
        {
            double ProportionToDisplay = 0.0;
            if (OriginalImage.Height > (windowHeight - HeightOfTabControl) || OriginalImage.Width > windowWidth)
            {
                if ((double)OriginalImage.Height / (windowHeight - 100) > (double)OriginalImage.Width / windowWidth)
                {
                    ProportionToDisplay = (double)(windowHeight - 100) / OriginalImage.Height;
                }
                else
                {
                    ProportionToDisplay = (double)windowWidth / OriginalImage.Width;
                }
                return ProportionToDisplay;
            }
            else
            {
                return 1;
            }
        }

        internal Avalonia.Media.Imaging.Bitmap getCurrentImageToShow()
        {
            throw new NotImplementedException();
        }

        public void AddRectangle(Avalonia.Point startPoint, Avalonia.Point endPoint, ISolidColorBrush brush)
        {
            Avalonia.Point intermediatePointOne = new Avalonia.Point((int)startPoint.X, (int)endPoint.Y);
            Avalonia.Point intermediatePointTwo = new Avalonia.Point((int)endPoint.X, (int)startPoint.Y);
            
            Avalonia.Controls.Shapes.Line lineOne = new Avalonia.Controls.Shapes.Line();
            Avalonia.Controls.Shapes.Line lineTwo = new   Avalonia.Controls.Shapes.Line();
            Avalonia.Controls.Shapes.Line lineThree = new Avalonia.Controls.Shapes.Line();
            Avalonia.Controls.Shapes.Line lineFour = new  Avalonia.Controls.Shapes.Line();

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

        internal void AddCanvas(Canvas annotationCanvas, int windowHeight, int windowWidth)
        {
            this.AnnotationCanvas = annotationCanvas;
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
        }
    }
}
