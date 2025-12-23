using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует null в Visibility (null = Collapsed, not null = Visible)
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
