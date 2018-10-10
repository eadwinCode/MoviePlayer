using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlyoutControl
{
    public sealed class FlyoutMenu : ItemsControl
    {
        private static FlyoutMenu _flyoutmenuinstance;
        private static bool IsFirstLaunched = false;
        static FlyoutMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutMenu), new FrameworkPropertyMetadata(typeof(FlyoutMenu)));
            EventManager.RegisterClassHandler(typeof(FlyoutSubMenuItem), IsSelectedChangedEvent, new RoutedPropertyChangedEventHandler<bool>(OnIsSelectedChanged));
        }

        internal static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent(
         "IsSelectedChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(FlyoutMenu));

       
        internal FlyoutSubMenuItem CurrentSelection
        {
            get { return (FlyoutSubMenuItem)GetValue(CurrentSelectionProperty); }
            set { SetValue(CurrentSelectionProperty, value); }
        }

        internal static FlyoutMenu FlyoutMenuInstance
        {
            get { return _flyoutmenuinstance; }
        }

        // Using a DependencyProperty as the backing store for CurrentSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentSelectionProperty =
            DependencyProperty.Register("CurrentSelection", typeof(FlyoutSubMenuItem), typeof(FlyoutMenu), new PropertyMetadata(null,OnCurrentSelectionChanged));

        private static void OnCurrentSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlyoutMenu flyoutMenu = d as FlyoutMenu;

            if (e.OldValue != null)
            {
                flyoutMenu.UnSelectedItem(e.OldValue as FlyoutSubMenuItem);
            }
            flyoutMenu.CurrentSelection = e.NewValue as FlyoutSubMenuItem;
        }

        public object MenuTopSection
        {
            get { return (object)GetValue(MenuTopSectionProperty); }
            set { SetValue(MenuTopSectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MenuTopSection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuTopSectionProperty =
            DependencyProperty.Register("MenuTopSection", typeof(object), typeof(FlyoutMenu), new PropertyMetadata(null));
        
        public object SubMenuHiddenContent
        {
            get { return (object)GetValue(SubMenuHiddenContentProperty); }
            set { SetValue(SubMenuHiddenContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubMenuHiddenContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubMenuHiddenContentProperty =
            DependencyProperty.Register("SubMenuHiddenContent", typeof(object), typeof(FlyoutMenu), new PropertyMetadata(null));


        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(FlyoutMenu), new PropertyMetadata(true));


        public Brush MenuIconColor
        {
            get { return (Brush)GetValue(MenuIconColorProperty); }
            set { SetValue(MenuIconColorProperty, value); }
        }

        public static readonly DependencyProperty MenuIconColorProperty =
            DependencyProperty.Register("MenuIconColor", typeof(Brush), typeof(FlyoutMenu), new FrameworkPropertyMetadata(System.Windows.Media.Brushes.White, FrameworkPropertyMetadataOptions.Inherits));


        public Brush SelectionIndicatorColor
        {
            get { return (Brush)GetValue(SelectionIndicatorColorProperty); }
            set { SetValue(SelectionIndicatorColorProperty, value); }
        }
        
        public static readonly DependencyProperty SelectionIndicatorColorProperty =
            DependencyProperty.Register("SelectionIndicatorColor", typeof(Brush), typeof(FlyoutMenu), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.Inherits));
        
        public Brush MenuItemForeground
        {
            get { return (Brush)GetValue(MenuItemForegroundProperty); }
            set { SetValue(MenuItemForegroundProperty, value); }
        }

        public static readonly DependencyProperty MenuItemForegroundProperty =
            DependencyProperty.Register("MenuItemForeground", typeof(Brush), typeof(FlyoutMenu), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.Inherits));


       
        private static void OnIsSelectedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            FlyoutSubMenuItem flyoutSubMenuItem = e.OriginalSource as FlyoutSubMenuItem;
            if (flyoutSubMenuItem != null)
            {
                if (_flyoutmenuinstance.CurrentSelection == flyoutSubMenuItem)
                    return;

                _flyoutmenuinstance.CurrentSelection = flyoutSubMenuItem;
            }
        }

        public override void BeginInit()
        {
            OverridesDefaultStyle = false;
            Focusable = false;
            
            base.BeginInit();
        }

        public FlyoutMenu()
        {
            _flyoutmenuinstance = this;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if(oldValue == null && newValue != null && !IsFirstLaunched)
            {
                var firstMenu = Items[0] as FlyoutSubMenuItem;
                this.IsOpen = !IsOpen;
                if (firstMenu.Command != null)
                {
                    firstMenu.IsSelected = true;
                    firstMenu.Command.Execute(firstMenu.CommandParameter);
                    IsFirstLaunched = true;
                }
            }
        }

        private void UnSelectedItem(FlyoutSubMenuItem currentSelection)
        {
            if (currentSelection.IsSelected)
                currentSelection.IsSelected = false;
        }


    }
}
