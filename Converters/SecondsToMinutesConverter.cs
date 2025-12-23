using System;
using System.Globalization;
using System.Windows.Data;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует секунды в минуты (например, 120 -> "2 мин")
    /// </summary>
    public class SecondsToMinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int seconds)
            {
                if (seconds < 60)
                {
                    return $"{seconds} сек";
                }
                else if (seconds % 60 == 0)
                {
                    return $"{seconds / 60} мин";
                }
                else
                {
                    int minutes = seconds / 60;
                    int remainingSeconds = seconds % 60;
                    return $"{minutes} мин {remainingSeconds} сек";
                }
            }
            return "0 сек";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
