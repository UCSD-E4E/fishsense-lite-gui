using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ML_Annotation_Tool.Data;
using ML_Annotation_Tool.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Tmds.DBus;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;

namespace ML_Annotation_Tool.Models
{
    public class DatabaseAccessor :INotifyPropertyChanged
    {
        private Database db;
        public bool filesChosen;
        public ObservableCollection<IndividualImage> Images { get; set; }
        public ObservableCollection<string> FileNames { get; set; }
        public ObservableCollection<Rectangle> annotations { get; set; }
        private int _imageIndex;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int ImageIndex
        {
            get { return _imageIndex; }
            set
            {
                _imageIndex = value;
                source.ImageIndex = value;
            }
        }
        MainWindowViewModel source;
        public DatabaseAccessor(MainWindowViewModel source)
        {
            db = new Database();
            this.Images = new ObservableCollection<IndividualImage>();
            this.FileNames = new ObservableCollection<string>();
            this.filesChosen = false;
            this._imageIndex = 0;
            this.source = source;
        }
        public void SetDatabaseConnection(string directoryPath)
        {
            db.SetDatabaseConnection(directoryPath);
        }
        public Rectangle MakeAnnotation(string annotationKey, Point topLeft, Point bottomRight, Canvas canvas)
        {
            double width = Math.Abs(bottomRight.X - topLeft.X);
            double height = Math.Abs(bottomRight.Y - topLeft.Y);
            RectangleGeometry rectangleGeometry = new RectangleGeometry();
            Rectangle rect = new Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = Brushes.Red,
                StrokeThickness = 3,
                Fill = Brushes.Transparent
            };


            db.InsertData(annotationKey, Images[ImageIndex].getPath(), topLeft, bottomRight);
            return rect;
        }
        public ObservableCollection<Rectangle> CurrentAnnotations()
        {
            return annotations;
        }
        public string GetCurrentImage() { return FileNames[ImageIndex]; }

        public void AddPath(string path)
        {
            Images.Add(new IndividualImage(path));
            FileNames.Add(path);
        }
        public void NextImage()
        {
            ImageIndex = (ImageIndex + 1) % Images.Count;
        }
        public void PreviousImage()
        {
            if (ImageIndex == 0)
            {
                ImageIndex = Images.Count - 1;
            }
            else
            {
                ImageIndex = (ImageIndex - 1) % Images.Count;
            }
        }

        public Bitmap CurrentBitmap() 
        {
            // This code has to test whether or not the files have been initialized yet.
            if (filesChosen)
            {
                return new Bitmap(FileNames[ImageIndex]);
            } else
            {
                return null;
            }
        }

        public void ClearFileNames()
        {
            Images.Clear();
            FileNames.Clear();
        }
    }
}
