using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace VirtualizingListView.Util
{
    public class FolderListSort : IComparer<TreeViewItem>
    {
        public static string DesktopString = "Desktop";
        public static string VideoString = "Videos";

        public int Compare(TreeViewItem x, TreeViewItem y)
        {
            if (   x.Header.ToString() == DesktopString ||
                  x.Header.ToString() == VideoString )
            {
                return 0;
            }

            if ((x.Tag as string).Length == 3 && !((y.Tag as string).Length == 3 || ((y.Header.ToString() == DesktopString ||
                 y.Header.ToString() == VideoString))))
            {
                return -1;
            }

            if ((x.Tag as string).Length == 3 && ((y.Header.ToString() == DesktopString ||
                 y.Header.ToString() == VideoString)))
            {
                return 0;
            }

            if ((x.Tag as string).Length == 3 && ((y.Tag as string).Length == 3))
            {
                return x.Tag.ToString().CompareTo(y.Tag.ToString());
            }

            if (y.Header.ToString() == DesktopString ||
                  y.Header.ToString() == VideoString)
            {
                return 0;
            }
            if ((y.Tag as string).Length == 3 && (x.Header.ToString() == DesktopString ||
                  x.Header.ToString() == VideoString))
            {
                return -1;
            }
            if ((y.Tag as string).Length == 3 && !(x.Header.ToString() == DesktopString ||
                  x.Header.ToString() == VideoString))
            {
                return 1;
            }
            var result = x.Tag.ToString().CompareTo(y.Tag.ToString()) * -1;
            return result;
        }
    }
}
