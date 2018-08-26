using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using VideoComponent.BaseClass;
using Movies.Enums;
using Movies.Models.Model;

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
            if (obj.FileType == FileType.Folder)
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
