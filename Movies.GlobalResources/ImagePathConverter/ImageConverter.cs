using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Movies.GlobalResources.ImagePathConverter
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return null;

            string ResPath = "pack://application:,,,/Movies.GlobalResources;component/Images/";
            string imageName = value.ToString();
            string imgPath = string.Format("{0}{1}", ResPath, imageName);

            return imgPath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
