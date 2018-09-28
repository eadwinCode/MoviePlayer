using Movies.Enums;
using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PresentationExtension.Converters
{
    public class GroupSort : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IItemSort obj = value as IItemSort;
            if (obj == null)
            {
                return value;
            }
            if (obj.FileType == GroupCatergory.Grouped)
            {
                return "Folders";
            }
            else
            {
                return "File";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
