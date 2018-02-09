using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VideoComponent.BaseClass;

namespace VirtualizingListView
{
   public class MoreTemplateSelector:DataTemplateSelector
    {
       public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
       {
           VideoFolder itm = item as VideoFolder;
           if (itm == null) return item as DataTemplate;
           if (itm.FileType == FileType.Folder)
           {
               return ((FrameworkElement)container).FindResource("FolderLargeTemplate") as DataTemplate;
           }
           else
           {
               return ((FrameworkElement)container).FindResource("FileLargeTemplate") as DataTemplate;
           }
       }
    }
}
