﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Common.Util;
using Movies.Models.Model;
using Movies.Enums;

namespace VirtualizingListView
{
    public class ItemListSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            MediaFolder itm = item as MediaFolder;
            if (itm == null) return item as DataTemplate;
            if (itm.FileType == GroupCatergory.Grouped)
            {
                return ((FrameworkElement)container).FindResource("FolderSmallTemplate") as DataTemplate;
            }
            else
            {
                return ((FrameworkElement)container).FindResource("FileSmallTemplate") as DataTemplate;
            }
        }
    }
}
