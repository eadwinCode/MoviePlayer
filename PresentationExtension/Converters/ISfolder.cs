using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace PresentationExtension.Converters
{
   public class ISfolder : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CollectionViewGroup crw = (CollectionViewGroup)value;
            if (crw.Name.ToString() == "Folders")
            {
                return Visibility.Visible;
            }
            else
            {
                if (!crw.IsBottomLevel)
                {
                    return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
