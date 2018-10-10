using MahApps.Metro.Controls;
using Movies.MoviesInterfaces;
using PresentationExtension.InterFaces;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PresentationExtension
{
    public class WindowCommandButton : Button, IWindowsCommandButton
    {
        public static event RoutedEventHandler OnWindowsCommandActivated;
        private static List<WindowCommandButton> windowsCommands;
        private HamburgerMenuIconItem hamburgermenuiconitem;
        public HamburgerMenuIconItem HamburgerMenuIconItem
        {
            get { return hamburgermenuiconitem; }
            set { hamburgermenuiconitem = value; }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set
            {
                SetValue(IsActiveProperty, value);
            }
        }


        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(WindowCommandButton), new PropertyMetadata(false));

        public string Label
        {
            get
            {
                if (HamburgerMenuIconItem == null)
                    GetData();
                return HamburgerMenuIconItem.Label;
            }
            set { }
        }

        public object Icon
        {
            get
            {
                if (HamburgerMenuIconItem == null)
                    GetData();
                return HamburgerMenuIconItem.Icon;
            }
            set { }
        }

        private void GetData()
        {
            HamburgerMenuIconItem = this.DataContext as HamburgerMenuIconItem;
            if (HamburgerMenuIconItem != null)
            {
                IMainPage mainPages = HamburgerMenuIconItem.Tag as IMainPage;
                //if (!mainPages.HasController)
                //    mainPages.SetController(this);
            }
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
