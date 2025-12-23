using System;
using System.Globalization;
using System.Windows.Data;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует bool в "На дежурстве"/"Вне дежурства"
    /// </summary>
    public class BooleanToOnDutyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "На дежурстве" : "Вне дежурства";
            }
            return "Вне дежурства";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
