using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TitheSync.UI.Resources
{
    public class InverseBooleanToVisibilityConverter:IValueConverter
    {
        /// <summary>
        ///     Converts a boolean value to a <see cref="Visibility" /> value, inverting the logic.
        /// </summary>
        /// <param name="value" >The boolean value to convert.</param>
        /// <param name="targetType" >The target type of the conversion (unused).</param>
        /// <param name="parameter" >An optional parameter (unused).</param>
        /// <param name="culture" >The culture information (unused).</param>
        /// <returns>
        ///     Returns <see cref="Visibility.Collapsed" /> if the value is true; otherwise, <see cref="Visibility.Visible" />.
        /// </returns>
        public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture ) => (bool)value! ? Visibility.Collapsed : Visibility.Visible;

        public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture ) => throw new NotImplementedException();
    }
}
