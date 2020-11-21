using System;
using System.Globalization;
using System.Windows.Data;

namespace Youtube_DL.Helps
{
    public class InserveBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value != null && System.Convert.ToBoolean(value);

            return !result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value != null && System.Convert.ToBoolean(value);

            return !result;
        }
    }
}