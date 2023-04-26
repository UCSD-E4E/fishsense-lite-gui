using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Annotation_Tool.Models
{
    public class Annotation
    {
        private string FileName { get; set; }
        private string AnnotationDescriptor { get; set; }
        private Point TopLeft { get; set; }
        private Point BottomRight { get; set; }

        public Annotation(string fileName, string annotationDescriptor, Point topLeft, Point bottomRight)
        {
            this.FileName = fileName;
            this.AnnotationDescriptor = annotationDescriptor;
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public double getTopLeftX() { return this.TopLeft.X; }
        public double getTopLeftY() { return this.TopLeft.Y; }
        public double getBottomRightX() { return this.BottomRight.X; }
        public double getBottomRightY() { return this.BottomRight.Y; }
        public string getPath() { return this.FileName; }
        public string getAnnotationDescriptor() { return this.AnnotationDescriptor; }
    }
}
