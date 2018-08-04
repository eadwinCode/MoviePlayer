using Common.Interfaces;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace VirtualizingListView
{
    public partial class ContextButton:Button
    {
        public ContextMenu ShowContextOnClick
        {
            get { return (ContextMenu)GetValue(ShowContextOnClickProperty); }
            set { SetValue(ShowContextOnClickProperty, value); }
        }

        public static readonly DependencyProperty ShowContextOnClickProperty =
            DependencyProperty.Register("ShowContextOnClick", typeof(ContextMenu), typeof(ContextButton), new PropertyMetadata(null));



        public PlacementMode ContextMentPlacement
        {
            get { return (PlacementMode)GetValue(ContextMentPlacementProperty); }
            set { SetValue(ContextMentPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextMentPlacementProperty =
            DependencyProperty.Register("ContextMentPlacement", typeof(PlacementMode), typeof(ContextButton), new FrameworkPropertyMetadata() {DefaultValue = PlacementMode.Top });



        private bool IsContextMenuOpen = false;
    }

    public partial class ContextButton
    {
        protected override void OnClick()
        {
            if (ShowContextOnClick == null)
                throw new NotImplementedException("ShowContextMenuOnClick Cannot be null");

            if (!IsContextMenuOpen)
            {
                ShowContextOnClick.Placement = ContextMentPlacement;
                ShowContextOnClick.PlacementTarget = this;
                ShowContextOnClick.Closed += Context_Closed;
                // btn.ContextMenu.p
                ShowContextOnClick.Focusable = false;
                ShowContextOnClick.StaysOpen = true;
                ShowContextOnClick.IsOpen = true;
                IsContextMenuOpen = true;
            }
            else
            {
                ShowContextOnClick.IsOpen = false;
                IsContextMenuOpen = false;
            }
        }

        private void Context_Closed(object sender, RoutedEventArgs e)
        {
            IsContextMenuOpen = false;
        }
    }


    public partial class WindowCommandButton : Button, IWindowsCommandButton
    {
        public static event RoutedEventHandler OnWindowsCommandActivated;
        private static List<WindowCommandButton> windowsCommands;
        private static bool isDeactivating = false;
        private HamburgerMenuIconItem hamburgermenuiconitem;
        public HamburgerMenuIconItem HamburgerMenuIconItem
        {
            get { return hamburgermenuiconitem; }
            set { hamburgermenuiconitem= value; }
        }
        
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value);
            }
        }


        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(WindowCommandButton), new PropertyMetadata(false));
        
        public string Label
        {
            get {
                if (HamburgerMenuIconItem == null)
                    GetData();
                return HamburgerMenuIconItem.Label;
            }
            set { }
        }

        public object Icon
        {
            get {
                if (HamburgerMenuIconItem == null)
                    GetData();
                return HamburgerMenuIconItem.Icon;
            }
            set { }
        }

        private void GetData()
        {
            HamburgerMenuIconItem = this.DataContext as HamburgerMenuIconItem;

            IMainPages mainPages = HamburgerMenuIconItem.Tag as IMainPages;
            if(!mainPages.HasController)
                mainPages.SetController(this);
        }

        public WindowCommandButton()
        {
            Add();
        }

        private void Add()
        {
            windowsCommands.Add(this);
        }

        static WindowCommandButton()
        {
            windowsCommands = new List<WindowCommandButton>();
        }

        private void DeActivateOtherButtons(bool loadpage)
        {
            foreach (var item in windowsCommands)
            {
                if (item == this) continue;
                item.IsActive = false;
            }

            if (loadpage && OnWindowsCommandActivated != null)
            {
                OnWindowsCommandActivated.Invoke(this, null);
            }
        }

        public void SetActive(bool setactive, bool loadPage)
        {
            IsActive = setactive;
            GetData();
            DeActivateOtherButtons(loadPage);
        }
    }
}
