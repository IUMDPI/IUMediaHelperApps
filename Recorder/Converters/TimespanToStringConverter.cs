using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Recorder.Converters
{
    public class TimespanToStringConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                return string.Empty;
            }

            var timespan = (TimeSpan) value;
            return $@"{timespan:hh\:mm\:ss\.ff}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
