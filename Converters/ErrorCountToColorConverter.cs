using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует количество ошибок в цвет (0 - зеленый, много - красный)
    /// </summary>
    public class ErrorCountToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int errorCount)
            {
                if (errorCount == 0)
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Зеленый
                else if (errorCount <= 2)
                    return new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Желтый
                else if (errorCount <= 5)
                    return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Оранжевый
                else
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Красный
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
