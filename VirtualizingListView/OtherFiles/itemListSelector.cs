using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using VideoComponent.BaseClass;
using Common.Util;

namespace VirtualizingListView
{
    public class itemListSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            VideoFolder itm = item as VideoFolder;
            if (itm == null) return item as DataTemplate;
            if (itm.FileType == FileType.Folder)
            {
                return ((FrameworkElement)container).FindResource("FolderTempLess") as DataTemplate;
            }
            else
            {
                return ((FrameworkElement)container).FindResource("TempLess") as DataTemplate;
            }
        }
    }
}
