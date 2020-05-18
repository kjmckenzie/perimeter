using System;
using System.Globalization;
using Xamarin.Forms;

namespace Perimeter.Converters
{
    /// <summary>
    /// Value converter that translates true to false and vice versa.
    /// </summary>
    public sealed class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            return !(bool)value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            throw new NotSupportedException();

        }
    }
}
