using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует балл в цвет (высокий балл - зеленый, низкий - красный)
    /// </summary>
    public class ScoreToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int score)
            {
                if (score >= 80)
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Зеленый
                else if (score >= 60)
                    return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Оранжевый
                else if (score >= 40)
                    return new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Желтый
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
