using System;
using System.Globalization;
using System.Windows.Data;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует отдельные поля в полное ФИО
    /// </summary>
    public class FullNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2)
            {
                string surname = values[0] as string ?? "";
                string name = values[1] as string ?? "";
                string middleName = values.Length > 2 ? values[2] as string : "";

                if (string.IsNullOrWhiteSpace(middleName))
                {
                    return $"{surname} {name}".Trim();
                }
                return $"{surname} {name} {middleName}".Trim();
            }
            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
