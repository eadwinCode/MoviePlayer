using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VirtualizingListView
{
    public class TextContexter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((parameter as string)== "Text")
            {
                string input = (value as string) == string.Empty ? null : value as string;
                return input == null ? "" : input + "/";
            }
            if ((parameter as string) == "Visibility")
            {
                string input = (value as string) == string.Empty ? null : value as string;
                return input == null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }
            return value;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
