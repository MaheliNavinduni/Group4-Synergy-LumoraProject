using System.Globalization;

namespace LumoraAcademy.Converters;

// Returns true when a text value has something in it.
// Used in XAML to hide labels whose text is empty:
//   IsVisible="{Binding Detail, Converter={StaticResource NotEmptyConverter}}"
public class NotEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrWhiteSpace(value as string);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
