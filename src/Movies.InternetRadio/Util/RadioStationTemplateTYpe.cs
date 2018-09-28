using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Movies.InternetRadio.Util
{
    public class RadioStationTemplateType : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            IMoviesRadio itm = item as IMoviesRadio;
            if (itm == null) return item as DataTemplate;
            if (itm.FileType == Enums.GroupCatergory.Grouped)
            {
                return ((FrameworkElement)container).FindResource("RadioStationGroupTemplate") as DataTemplate;
            }
            else
            {
                return ((FrameworkElement)container).FindResource("RadioStationTemplate") as DataTemplate;
            }
        }
    }
}
