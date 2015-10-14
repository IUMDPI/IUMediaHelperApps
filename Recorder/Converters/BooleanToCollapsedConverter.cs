using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Recorder.Converters
{
    internal class BooleanToCollapsedConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //reverse conversion (false=>Visible, true=>collapsed) on any given parameter
            var input = (null == parameter) ? (bool) value : !((bool) value);
            return (input) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}