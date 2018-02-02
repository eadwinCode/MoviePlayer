using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.WindowsAPICodePack.Shell;

namespace VirtualizingListView
{
    [ValueConversion(typeof(string), typeof(bool))]


    public class HeaderImageConverter : IValueConverter
    {
        static Image Desktop;
        static Image Video;
       // static string[] drives;

        static HeaderImageConverter()
        {
            Desktop = new Image();
            Video = new Image();
            using (ShellObject shell = ShellObject.FromParsingName(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)))
            {
                Video.Source = shell.Thumbnail.LargeBitmapSource;
            }

            using (ShellObject shell = ShellObject.FromParsingName(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)))
            {
                Desktop.Source = shell.Thumbnail.LargeBitmapSource;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ( (value as string).Length == 3 )
            {
                Uri uri = new Uri("pack://application:,,,/VirtualizingListView;component/Resources/Images/diskdrive.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
            else if (value.ToString() == Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString() || value.ToString() == Environment.GetFolderPath(Environment.SpecialFolder.MyVideos).ToString())
            {
                Image source = new Image();

                if ((value as string) == Environment.GetFolderPath(Environment.SpecialFolder.MyVideos).ToString())
                {
                    return Video.Source;
                }
                else
                {
                    return Desktop.Source;

                }
            }
            else
            {
                Uri uri = new Uri("pack://application:,,,/VirtualizingListView;component/Resources/Images/Movies - Copy.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
