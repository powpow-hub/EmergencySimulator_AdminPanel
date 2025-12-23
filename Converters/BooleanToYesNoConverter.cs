using System;
using System.Globalization;
using System.Windows.Data;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует bool в "Да"/"Нет"
    /// </summary>
    public class BooleanToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Да" : "Нет";
            }
            return "Нет";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue == "Да";
            }
            return false;
        }
    }
}
