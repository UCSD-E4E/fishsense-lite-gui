using FishSenseLiteGUI.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

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
    /// Purpose: Model layer, which connects the data layer to the View Model and stores image paths.
    /// </summary>
    public class DataAccessorModel
    {
        // Private instance variables that store the necessary connections to access data and invoke methods.
        private Database db;
        private ObservableCollection<string> fullPaths;

        // Creates database object using passed in path, and stores view model to access data.
        public DataAccessorModel(string directory)
        {
            db = new Database(directory);
            fullPaths = new ObservableCollection<string>();
        }

        public void AddPath(string path)
        {
            fullPaths.Add(path);
        }

        public ObservableCollection<string> getFileNames()
        {
            return fullPaths;
        }

        public IEnumerable<string[]> RequestAnnotationsFromIndex(int imageIndex)
        {
            foreach (string[] data in db.RequestAnnotationsForPath(Path.GetFileName(fullPaths[imageIndex])))
            {
                yield return data;
            }
        }

        internal void AddAnnotation(int annotationDescriptor, string path, int startPointXPixelValue, int startPointYPixelValue, int endPointXPixelValue, int endPointYPixelValue)
        {
            db.InsertData(annotationDescriptor.ToString(), path,
                          startPointXPixelValue, startPointYPixelValue, 
                          endPointXPixelValue, endPointYPixelValue);
        }
    }
}
