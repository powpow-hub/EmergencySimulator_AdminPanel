using System;
using System.Globalization;
using System.Windows.Data;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует DateTime в строку с форматом
    /// </summary>
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? dateTime = value as DateTime?;

            if (dateTime.HasValue)
            {
                string format = parameter as string ?? "dd.MM.yyyy HH:mm";
                return dateTime.Value.ToString(format);
            }

            return "Не указано";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && DateTime.TryParse(stringValue, out DateTime result))
            {
                return result;
            }
            return null;
        }
    }
}
