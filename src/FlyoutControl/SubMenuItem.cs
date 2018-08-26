using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FlyoutControl
{
    public class SubMenuItem : ContentControl
    {
        static SubMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SubMenuItem), new FrameworkPropertyMetadata(typeof(SubMenuItem)));
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SubMenuItem), new UIPropertyMetadata(" No Title "));


    }
}
