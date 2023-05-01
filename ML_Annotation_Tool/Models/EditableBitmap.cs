using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Annotation_Tool.Models
{
    /* Custom bitmap class that contains 2 bitmaps.One for the Original Bitmap, one for the 
     * edited image. Allows user to add annotations directly to the bitmap itself to display 
     * the Annotation boxes on the UI itself.
     */
    public class EditableBitmap
    {
        private Bitmap original;
        private Bitmap edited;
        private string ImagePath;
        public EditableBitmap(string path)
        {
            original = new Bitmap(path);
            edited = new Bitmap(path);
            ImagePath = path;
        }

        public Bitmap getEditedBitmap() { return edited; }
        public Bitmap getOriginalBitmap() { return original; }

        // Adds annotation directly to the Edited image bitmap.
        public Bitmap AddAnnotation(int AnnotationDescriptor, int firstPointX, int firstPointY, int secondPointX, int secondPointY, int width, int height)
        {
            // Chose color of the box based on the AnnotationDescriptor.
            Color color = Color.White; 

            if (AnnotationDescriptor == 0) 
            { 
                color = Color.Red; 
            } 
            else if (AnnotationDescriptor == 1) 
            { 
                color = Color.Green; 
            } 
            else if (AnnotationDescriptor == 2) 
            { 
                color = Color.Blue; 
            }

            for (int x = (int)((double)firstPointX / width * edited.Width); x < (int)((double)secondPointX / width * edited.Width); x++)
            {
                int firstAnnotationYValue = (int)((double)firstPointY / height * edited.Height);
                int secondAnnotationYValue = (int)((double)secondPointY / height * edited.Height);
                edited.SetPixel(x, firstAnnotationYValue, color);
                edited.SetPixel(x, firstAnnotationYValue - 1, color);
                edited.SetPixel(x, firstAnnotationYValue - 2, color);

                edited.SetPixel(x, secondAnnotationYValue, color);
                edited.SetPixel(x, secondAnnotationYValue + 1, color);
                edited.SetPixel(x, secondAnnotationYValue + 2, color);
            }

            for (int y = (int)((double)firstPointY / height * edited.Height); y < (int)((double)secondPointY / height * edited.Height); y++)
            {
                int firstAnnotationXValue = (int)((double)firstPointX / width * edited.Width);
                int secondAnnotationXValue = (int)((double)secondPointX / width * edited.Width);
                edited.SetPixel(firstAnnotationXValue + 2, y, color);
                edited.SetPixel(firstAnnotationXValue + 1, y, color);
                edited.SetPixel(firstAnnotationXValue, y, color);

                edited.SetPixel(secondAnnotationXValue - 2, y, color);
                edited.SetPixel(secondAnnotationXValue - 1, y, color);
                edited.SetPixel(secondAnnotationXValue , y, color);
            }
            return edited;
        }
        public bool Equals(string path) { return ImagePath == path; }
    }
}
