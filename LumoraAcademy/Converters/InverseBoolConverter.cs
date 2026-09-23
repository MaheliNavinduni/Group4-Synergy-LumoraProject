using System.Globalization;

namespace LumoraAcademy.Converters;

// Turns true into false and false into true.
// Used in XAML to show a control only when something is NOT true:
//   IsVisible="{Binding IsPaid, Converter={StaticResource InverseBool}}"
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }
}
