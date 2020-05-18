using System;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using Xamarin.Forms;

namespace Perimeter.Converters
{
    /// <summary>
    /// return an image reflecting a stock change
    /// </summary>
    public class DistanceConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            string result = "";

            if (value is double)          
            {
                double distance=(double)value;

                if (distance >= 8)
                    result = "Far";
                else if (distance > 0.5 && Device.RuntimePlatform == Device.iOS)
                    result = "Near";
                else if (distance > 1.5 && Device.RuntimePlatform == Device.Android)
                    result = "Near";
                else
                    result = "Close";
                
            }

            return result;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            throw new NotSupportedException();
        }
    }
}
