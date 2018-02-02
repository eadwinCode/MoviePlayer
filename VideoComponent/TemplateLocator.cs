using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace VideoComponent
{
    public class  FolderTemplate : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return ((FrameworkElement)container).FindResource("FolderView") as DataTemplate;
        }
    }

    public class FileTemplate : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return (((FrameworkElement)container).FindResource("TempLess") as DataTemplate);
        }
    }
}
