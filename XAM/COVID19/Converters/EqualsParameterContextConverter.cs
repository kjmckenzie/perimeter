using System;
using Xamarin.Forms;
using System.Globalization;
using System.Collections;

namespace Perimeter.Converters
{
    public class EqualsParameterContextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == ((View)parameter).BindingContext;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}