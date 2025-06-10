using System;
using System.Globalization;
using System.Windows.Data;

namespace ControlOfComputerClub.View.Converters
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool status ? (status ? "Занято" : "Не занято") : "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == "Занято";
        }
    }
}