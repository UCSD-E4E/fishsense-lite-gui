using ML_Annotation_Tool.Data;
using ML_Annotation_Tool.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static ML_Annotation_Tool.Models.indices;

namespace ML_Annotation_Tool.Models
{
    // Enum to better describe indices to the Annotation Data stored in the database.
    enum indices {
        ANNOTATIONDESCRIPTOR = 0,
        IMAGEPATH = 1,
        TOPLEFTX = 2,
        TOPLEFTY = 3,
        BOTTOMRIGHTX = 4,
        BOTTOMRIGHTY = 5,
    };
    /* This class is the bulk of the model layer, and it connects the data layer to the viewmodel
     * layer. It stores a Database object, the bitmaps of the images themselves (using a custom
     * EditableBitmap class), and allows users to update the UI with new images, add annotations,
     * add images, delete images, and move between images.
     * 
     */
    public class DB_Accessor
    {
        // Private instance variables that store the necessary connections to access data and invoke methods.
        private Database db;
        private MainWindowViewModel source;
        private Bitmap OriginalImage;
        private Bitmap EditedImage;

        // Contains custom EditableBitmap class that contains method to add annotations, pixel by pixel to the bitmaps
        private ObservableCollection<EditableBitmap> bitmaps;
        private ObservableCollection<string> fullPaths;

        private int _imageIndex;
        public int ImageIndex
        {
            get { return _imageIndex; }
            set 
            { 
                _imageIndex = value;
                // Notifies UI that the image index has been updated.
                ImageUpdated();                
            }
        }
        // Creates database object using passed in path, and stores view model to access data.
        public DB_Accessor(string path, MainWindowViewModel source)
        {
            db = new Database(path);
            this.source = source;
            bitmaps = new ObservableCollection<EditableBitmap>();
            fullPaths = new ObservableCollection<string>();

            // For now, initializes image index to 0. Will later order list in alphabetical order.
            _imageIndex = 0;   
        }
        public void ImageUpdated()
        {
            // Adds annnotations that were previously stored in database.
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
                EditedImage.Save(memory, ImageFormat.Png);

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
            foreach (string[] data in db.RequestAnnotationsForPath(Path.GetFileName(fullPaths[ImageIndex])))
            {
                // Enums are indexes 0-4, just added for extra readability. Could remove later if too verbose.
                bitmaps[ImageIndex].AddAnnotation(Convert.ToInt32(data[(int)ANNOTATIONDESCRIPTOR]), 
                                                  Convert.ToInt32(data[(int)TOPLEFTX]), 
                                                  Convert.ToInt32(data[(int)TOPLEFTY]),
                                                  Convert.ToInt32(data[(int)BOTTOMRIGHTX]), 
                                                  Convert.ToInt32(data[(int)BOTTOMRIGHTY]), 
                                                  getWidth(), 
                                                  getHeight());
            }
        }
        
        // Added path to choose beginning image. Not used currently, but
        // added for future implementation. If no proper filename is passed in,
        // just displays first image.
        public void ChooseFirstImage(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                foreach (EditableBitmap image in bitmaps)
                {
                    if (image.Equals(fileName))
                    {
                        OriginalImage = image.getOriginalBitmap();
                        EditedImage = image.getEditedBitmap();
                        ImageIndex = bitmaps.IndexOf(image);
                    }
                }
            } else
            {
                OriginalImage = bitmaps[0].getOriginalBitmap();
                EditedImage = bitmaps[0].getEditedBitmap();
                ImageIndex = 0;
            }
        }

        public void AddImage(string fullPath)
        {
            bitmaps.Add(new EditableBitmap(fullPath));
            fullPaths.Add(fullPath);
        }

        // Adds the annotation to the database and updates the UI with newly annotated image.
        public void AddAnnotation(int AnnotationDescriptor, Avalonia.Point firstPoint, Avalonia.Point secondPoint, int width, int height)
        {
            // Finds min and max to see which is the top left and which is the bottom right.
            int topLeftX = Math.Min((int)firstPoint.X, (int)secondPoint.X);
            int topLeftY = Math.Min((int)firstPoint.Y, (int)secondPoint.Y);
            int bottomRightX = Math.Max((int)firstPoint.X, (int)secondPoint.X); 
            int bottomRightY = Math.Max((int)firstPoint.Y, (int)secondPoint.Y);

            // Adds data to database.
            db.InsertData(AnnotationDescriptor.ToString(), System.IO.Path.GetFileName(fullPaths[ImageIndex]), topLeftX, topLeftY, bottomRightX, bottomRightY);
            
            // Edits image's bitmap directly.
            bitmaps[ImageIndex].AddAnnotation(AnnotationDescriptor, topLeftX, topLeftY, bottomRightX, bottomRightY, width, height);

            // Adds edited image to model.
            EditedImage = bitmaps[ImageIndex].getEditedBitmap();

            // Notifies UI that the Image has been upadted.
            ImageUpdated();
        }

        public Bitmap getCurrentImage() 
        {  
            if (EditedImage == null)
            {
                if (bitmaps.Count > 0)
                {
                    EditedImage = bitmaps[0].getEditedBitmap();
                }
            }
            return EditedImage;
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
            EditedImage = bitmaps[ind].getEditedBitmap();
            OriginalImage = bitmaps[ind].getOriginalBitmap();
            ImageIndex = ind;

        }

        internal void PreviousImage()
        {
            int ind = (ImageIndex - 1);
            if (ind < 0)
            {
                ind = bitmaps.Count - 1;
            }
            EditedImage = bitmaps[ind].getEditedBitmap();
            OriginalImage = bitmaps[ind].getOriginalBitmap();

            ImageIndex = ind;
        }

        public int getHeight()
        {
            return EditedImage.Height;
        }

        public int getWidth()
        {
            return EditedImage.Width;
        }
    }
}
