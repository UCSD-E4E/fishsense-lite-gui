using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Annotation_Tool.Models
{
    public class FishAnnotationsModel
    {
        private List<IndividualFish> allFish;
        private int numFish;
        public FishAnnotationsModel()
        {
            numFish = 0;
            allFish = new List<IndividualFish>();
        }   
        
        public void addFish(string filePath)
        {
            allFish[numFish] = new IndividualFish(filePath);
        }

        public void addAnnotation(string filePath, List<double> data)
        {
            for (int i = 0; i < numFish; i++)
            {
                if (allFish[i].compareName(filePath))
                {
                    allFish[i].addAnnotations(data);
                }
            }
        }

        public void removeFish(string filePath)
        {
            for (int i = 0; i < numFish; i++)
            {
                if (allFish[i].compareName(filePath))
                {
                    allFish.RemoveAt(i);
                    numFish--;
                }
            }
        }
        
    }
}
