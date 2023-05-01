using Avalonia.Data.Converters;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Annotation_Tool.Models
{
    /* Value Converter to subtract 200 from the passed in Xaml value.
     * Still a work in progress and is not used in the current implementation. This seems like the best solution moving forward to 
     * ensure the image will fit in the bounds of the window itself.
     */
    public class SubtractionConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string equation = parameter as string;
            if (equation != null)
            {
                var k = new ErrorMessageBox("Value is: " + value.ToString());
                var w = new ErrorMessageBox("Parameter is: " + parameter.ToString());
                var j = new ErrorMessageBox("Culture is: " + culture.ToString());
            }
            return 3;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
