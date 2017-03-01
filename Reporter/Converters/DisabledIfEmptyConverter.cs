using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Reporter.Models;

namespace Reporter.Converters
{
    class DisabledIfEmptyConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = value as BindingList<AbstractReportEntry>;
            if (list == null)
            {
                return false;
            }

            return list.Any();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
