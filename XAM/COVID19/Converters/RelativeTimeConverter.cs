using System;
using System.Globalization;
using Xamarin.Forms;

namespace Perimeter.Converters
{
    /// <summary>
    /// Given a DateTime, return a string representing its relative value, like "10 minutes ago".
    /// </summary>
    public class RelativeTimeConverter : IValueConverter
    {
        string YearAgo = "y";
        string YearsAgo = "y";
        string MonthAgo = "m";
        string MonthsAgo = "m";
        string WeekAgo = "w";
        string WeeksAgo = "w";
        string DayAgo = "d";
        string DaysAgo = "d";
        string HourAgo = "h";
        string HoursAgo = "h";
        string MinuteAgo = "m";
        string MinutesAgo = "m";
        string SecondAgo = "s";
        string SecondsAgo = "s";
        string JustNow = "just now";
        string Coming = "Coming";

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
        public object Convert(object value, Type targetType=null, object parameter=null, CultureInfo language=null)
        {
            if (!(value is DateTime))
            {
                return string.Empty;
            }

            DateTime date = (DateTime)value;
            TimeSpan diff = DateTime.Now - date;
            string suffix = string.Empty;
            int numeral = 0;

            if (diff.TotalDays >= 365)
            {
                numeral = (int)Math.Floor(diff.TotalDays / 365);
                suffix = numeral == 1 ? this.YearAgo : this.YearsAgo;
            }
            else if (diff.TotalDays >= 31)
            {
                numeral = (int)Math.Floor(diff.TotalDays / 31);
                suffix = numeral == 1 ? this.MonthAgo : this.MonthsAgo;
            }
            else if (diff.TotalDays >= 7)
            {
                numeral = (int)Math.Floor(diff.TotalDays / 7);
                suffix = numeral == 1 ? this.WeekAgo : this.WeeksAgo;
            }
            else if (diff.TotalDays >= 1)
            {
                numeral = (int)Math.Floor(diff.TotalDays);
                suffix = numeral == 1 ? this.DayAgo : this.DaysAgo;
            }
            else if (diff.TotalHours >= 1)
            {
                numeral = (int)Math.Floor(diff.TotalHours);
                suffix = numeral == 1 ? this.HourAgo : this.HoursAgo;
            }
            else if (diff.TotalMinutes >= 1)
            {
                numeral = (int)Math.Floor(diff.TotalMinutes);
                suffix = numeral == 1 ? this.MinuteAgo : this.MinutesAgo;
            }
            else if (diff.TotalSeconds >= 1)
            {
                numeral = (int)Math.Floor(diff.TotalSeconds);
                suffix = numeral == 1 ? this.SecondAgo : this.SecondsAgo;
            }
            else
            {
                if(date.Date>DateTime.Now.AddDays(1))
                {
                    suffix = string.Format("{0} {1}",this.Coming,date.ToString("MMM dd, yyyy"));

                } else
                {
                    suffix = this.JustNow;
                }
                
            }

            string output = numeral == 0 ? suffix : string.Format(CultureInfo.InvariantCulture, "{0}{1}", numeral, suffix);
            return output;
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
