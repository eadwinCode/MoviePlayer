
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VideoPlayerControl
{
    public class TextConverter: IValueConverter{
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           
            double input = (double)value;

            return TimeSpan.FromSeconds(input).ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
