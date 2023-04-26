using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Annotation_Tool.Models
{
    public class IndividualImage
    {
        private List<Annotation> annotations;

        private string _name;
        public string Name
        {
            get { return _name; }
            set => throw new NotImplementedException();
        }
        public IndividualImage(string filename)
        {
            this._name = filename;
        }
        public void AddAnnotation(Annotation annotation) { annotations.Add(annotation); }
        public string getPath() { return _name; }
    }
}
