using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace EmergencySimulator.AdminPanel.Converters
{
    /// <summary>
    /// Конвертирует секунды в минуты (например, 120 -> "2 мин") и обратно
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
            if (value is string str)
            {
                try
                {
                    str = str.Trim();

                    // Паттерны для различных форматов

                    int totalSeconds = 0;

                    // Ищем минуты
                    var minutesMatch = Regex.Match(str, @"(\d+)\s*мин", RegexOptions.IgnoreCase);
                    if (minutesMatch.Success)
                    {
                        totalSeconds += int.Parse(minutesMatch.Groups[1].Value) * 60;
                    }

                    // Ищем секунды
                    var secondsMatch = Regex.Match(str, @"(\d+)\s*сек", RegexOptions.IgnoreCase);
                    if (secondsMatch.Success)
                    {
                        totalSeconds += int.Parse(secondsMatch.Groups[1].Value);
                    }

                    // Если ничего не найдено, пробуем просто число
                    if (totalSeconds == 0 && int.TryParse(str, out int parsedValue))
                    {
                        totalSeconds = parsedValue;
                    }

                    // Проверяем, что значение неотрицательное
                    if (totalSeconds < 0)
                        return 0;

                    return totalSeconds;
                }
                catch
                {
                    // В случае любой ошибки возвращаем 0
                    return 0;
                }
            }

            return 0;
        }
    }
    public class NumericValidationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Преобразование из int в string для отображения
            if (value == null)
                return "0";

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Преобразование из string в int при вводе
            if (value is string str)
            {
                // Если строка пустая, возвращаем 0
                if (string.IsNullOrWhiteSpace(str))
                    return 0;

                // Пытаемся распарсить число
                if (int.TryParse(str, out int result))
                {
                    // Проверяем, что число неотрицательное
                    if (result < 0)
                        return 0;

                    return result;
                }

                // Если не удалось распарсить, возвращаем 0
                return 0;
            }

            return 0;
        }
    }

    /// <summary>
    /// Конвертер для работы со StepOrder
    /// </summary>
    public class StepOrderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "1";

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                    return 1;

                if (int.TryParse(str, out int result))
                {
                    // StepOrder должен быть минимум 1
                    if (result < 1)
                        return 1;

                    return result;
                }

                return 1;
            }

            return 1;
        }
    }
}
