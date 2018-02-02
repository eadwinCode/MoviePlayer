using Common.Interfaces;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace VideoPlayer
{
    public class ViewConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           
            string par = parameter as string;
            bool input = (bool)value;
            if (input && par == "OtherControl")
            {
                return Visibility.Visible;
            }
            else if (!input && par == "OtherControl2")
            {
                return Visibility.Visible;
            }
            if (!input)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
          //  return TimeSpan.FromSeconds(input).ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
