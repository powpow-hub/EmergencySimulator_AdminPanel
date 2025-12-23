using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует IsCritical в цвет (true = красный, false = черный)
    /// </summary>
    public class CriticalToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCritical)
            {
                return isCritical
                    ? new SolidColorBrush(Color.FromRgb(211, 47, 47))  // Красный
                    : new SolidColorBrush(Colors.Black);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
