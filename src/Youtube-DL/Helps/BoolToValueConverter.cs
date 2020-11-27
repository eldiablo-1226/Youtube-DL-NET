using System;
using System.Globalization;
using System.Windows.Data;

namespace Youtube_DL.Helps
{
    internal class BoolToValueConverter : IValueConverter
    {
        public object NullOrFalseValue { get; set; }
        public object TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || !System.Convert.ToBoolean(value, CultureInfo.InvariantCulture)
                ? NullOrFalseValue
                : TrueValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TrueValue != null
                ? value.Equals(TrueValue)
                : !value.Equals(NullOrFalseValue);
        }
    }
}