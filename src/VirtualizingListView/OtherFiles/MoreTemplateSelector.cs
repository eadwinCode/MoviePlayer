﻿using Common.Util;
using Movies.Models.Model;
using Movies.Enums;
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
           if (itm.FileType == GroupCatergory.Grouped)
           {
               return ((FrameworkElement)container).FindResource("FolderLargeTemplate") as DataTemplate;
           }
           else
           {
                var template = ((FrameworkElement)container).FindResource("FileLargeTemplate") as DataTemplate;
                return template;
           }
       }
    }
}
