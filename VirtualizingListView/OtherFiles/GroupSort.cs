using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using VideoComponent.BaseClass;

namespace VirtualizingListView
{
    public class GroupSort : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            VideoFolder obj = value as VideoFolder;
            if (obj == null)
            {
                return value;
            }
            if (obj.Directory == null) 
            { return string.Empty; }
            if (obj.Exists)
            {
                return "Folders";
            }
            else
            {
                return "Videos";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
