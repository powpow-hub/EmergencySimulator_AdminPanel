using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует статус в цвет (Завершено - зеленый, В процессе - синий, Прервано - красный)
    /// </summary>
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                switch (status)
                {
                    case "Завершено":
                        return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Зеленый
                    case "В процессе":
                        return new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Синий
                    case "Прервано":
                        return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Красный
                    default:
                        return new SolidColorBrush(Colors.Gray);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
