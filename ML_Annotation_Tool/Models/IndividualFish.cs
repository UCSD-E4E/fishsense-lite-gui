using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Annotation_Tool.Models
{
    public class IndividualFish
    {
        private string fileName;
        private int numAnnotations;
        private List<List<double>> annotations;    
        public IndividualFish(string fileName)
        {
            this.fileName = fileName;
            numAnnotations = 0;
        }
        public bool compareName(string nameToTest)
        {
            return this.fileName == nameToTest;
        }
        public void addAnnotations(List<double> newAnnotation)
        {
            annotations[numAnnotations] = newAnnotation;
            numAnnotations++;
        }

    }
}
