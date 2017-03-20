using System;
using System.Windows;
using System.Windows.Data;

namespace InteractiveScheduler.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public BooleanToVisibilityConverter() { }
        
        public bool Reverse { get; set; }
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var booleanValue = value != null && (bool)value;

            return booleanValue != Reverse 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
    }
}
