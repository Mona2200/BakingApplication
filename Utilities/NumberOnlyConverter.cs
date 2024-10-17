using System.Globalization;
using System.Windows.Data;

namespace BakingApplication.Utilities;

public class NumberOnlyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || value.ToString().All(char.IsDigit))
        {
            return value;
        }
        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(int) && int.TryParse(value?.ToString(), out int result))
        {
            return result;
        }
        return Binding.DoNothing;
    }
}
