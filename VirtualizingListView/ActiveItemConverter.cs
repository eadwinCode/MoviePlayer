using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace VirtualizingListView
{
    public class ActiveItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            if ((parameter as string) == "Foreground")
            {
                return val == true ? Brushes.WhiteSmoke : Brushes.White;
            }
          
            return val == true ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF364E6F")) : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
