using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace VirtualizingListView
{
    public class TreeViewItemSort : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TreeViewItem obj = value as TreeViewItem;
            if (obj.Header.ToString()== "Videos" || obj.Header.ToString() == "Desktop")
            {
                return "Special Folders";
            }
            else if (obj.Tag.ToString().Length == 3)
            {
                return "Directory";
            }
            else
            {
                return "Others";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //public class TreeViewItemSort : IComparer<TreeViewItem>
    //{
    //    public int Compare(TreeViewItem x, TreeViewItem y)
    //    {
    //        if (x.Tag.ToString() == y.Tag.ToString()) return 0;

    //        if (x.Tag.ToString().Length == 3 && y.Tag.ToString().Length == 3)
    //        {
    //            return x.Tag.ToString().CompareTo(y.Tag.ToString());
    //        }

    //        if (x.Tag.ToString().Length == 3 || y.Tag.ToString().Length == 3)
    //        {
    //            return 1;
    //        }

    //        var result = x.Header.ToString().CompareTo(y.Header.ToString());
    //        return result;
    //    }

    //}
}
