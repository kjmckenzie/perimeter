using System;
using System.Globalization;
using Xamarin.Forms;

namespace Perimeter.Converters
{
    /// <summary>
    /// return an image reflecting a stock change
    /// </summary>
    public class InfectionImageConverter : IValueConverter
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

            if (value == null)
            {
                
            }
            else
            {
                bool infected;

                bool bSuccess = bool.TryParse(value.ToString(), out infected);

                if (bSuccess)
                {
                    //if (input.StartsWith("-"))
                    //    result= "ms-appx:///assets/stockdown.png";
                    //else if (input.StartsWith("+"))
                    //    result= "ms-appx:///assets/stockup.png";

                    if (infected)
                        result = "assets_infectionon.png";
                    else
                        result = "assets_infectionoff.png";   

                } 
                
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
