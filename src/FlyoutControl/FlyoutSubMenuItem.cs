using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace FlyoutControl
{
    public class FlyoutSubMenuItem : Control,ICommandSource, IMenuFlyout
    {
        static FlyoutSubMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutSubMenuItem), new FrameworkPropertyMetadata(typeof(FlyoutSubMenuItem)));
        }
        
        private Button ListBoxItemButton { get { return GetTemplateChild("ListBoxItemButton") as Button; } }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(FlyoutSubMenuItem), new PropertyMetadata(String.Empty));


        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(FlyoutSubMenuItem), new PropertyMetadata(null));

        public Brush SelectionIndicatorColor
        {
            get { return (Brush)GetValue(SelectionIndicatorColorProperty); }
            set { SetValue(SelectionIndicatorColorProperty, value); }
        }

        public static readonly DependencyProperty SelectionIndicatorColorProperty =
            DependencyProperty.Register("SelectionIndicatorColor", typeof(Brush), typeof(FlyoutSubMenuItem), new FrameworkPropertyMetadata(Brushes.Blue, FrameworkPropertyMetadataOptions.Inherits));

        public Brush MenuIconColor
        {
            get { return (Brush)GetValue(MenuIconColorProperty); }
            set { SetValue(MenuIconColorProperty, value); }
        }

        public static readonly DependencyProperty MenuIconColorProperty =
            DependencyProperty.Register("MenuIconColor", typeof(Brush), typeof(FlyoutSubMenuItem), new FrameworkPropertyMetadata(System.Windows.Media.Brushes.White, FrameworkPropertyMetadataOptions.Inherits));


        public Brush MenuItemForeground
        {
            get { return (Brush)GetValue(MenuItemForegroundProperty); }
            set { SetValue(MenuItemForegroundProperty, value); }
        }

        public static readonly DependencyProperty MenuItemForegroundProperty =
            DependencyProperty.Register("MenuItemForeground", typeof(Brush), typeof(FlyoutSubMenuItem), new FrameworkPropertyMetadata(Brushes.Transparent,FrameworkPropertyMetadataOptions.Inherits));


        
        public static readonly DependencyProperty CommandProperty =
                ButtonBase.CommandProperty.AddOwner(
                        typeof(FlyoutSubMenuItem),
                        new FrameworkPropertyMetadata(
                                (ICommand)null,
                                new PropertyChangedCallback(OnCommandChanged)));
        
        [Bindable(true)]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }


        public static readonly DependencyProperty CommandParameterProperty =
                ButtonBase.CommandParameterProperty.AddOwner(typeof(FlyoutSubMenuItem),new FrameworkPropertyMetadata((object)null));
        
        [Bindable(true)]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandTargetProperty =
                ButtonBase.CommandTargetProperty.AddOwner(
                        typeof(FlyoutSubMenuItem),
                        new FrameworkPropertyMetadata((IInputElement)null));
        
        [Bindable(true)]
        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        public bool IsSelected
        {
            get { return GetIsSelected(this); }
            set { SetIsSelected(this, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
                DependencyProperty.RegisterAttached(
                        "IsSelected",typeof(bool),typeof(FlyoutSubMenuItem), new FrameworkPropertyMetadata(false,
                                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender, new PropertyChangedCallback(OnIsSelectedChanged)));
        
        
        public static bool GetIsSelected(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(IsSelectedProperty);
        }

        
        public static void SetIsSelected(DependencyObject element, bool isSelected)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(IsSelectedProperty, isSelected);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ListBoxItemButton.Click += ListBoxItemButton_Click;
        }

        private void ListBoxItemButton_Click(object sender, RoutedEventArgs e)
        {
            IsSelected = true;
        }
        
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlyoutSubMenuItem flyoutsubmenuitem = (FlyoutSubMenuItem)d;
            if(flyoutsubmenuitem != null)
            {
                if ((bool)e.OldValue)
                {
                    return;
                }
                flyoutsubmenuitem.RaiseEvent(new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue, FlyoutMenu.IsSelectedChangedEvent));
            }
        }
    }
}
