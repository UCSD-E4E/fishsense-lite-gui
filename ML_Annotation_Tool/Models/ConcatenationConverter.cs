using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Annotation_Tool.Models
{
    public class ConcatenationConverter : IMultiValueConverter
    {
        public static ConcatenationConverter Instance { get; } = new ConcatenationConverter();
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null || values.Count == 0)
            {
                return null;
            }
            return string.Join(" ", values.Select(v => v?.ToString()).Where(v => v != null));
        }
    }
}
