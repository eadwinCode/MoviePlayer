using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VirtualizingListView.Pages.ViewModel;

namespace VirtualizingListView.Pages.Views
{
    /// <summary>
    /// Interaction logic for UsbDrivePage.xaml
    /// </summary>
    public partial class UsbDrivePage : Page, IMainPages
    {
        public bool HasController { get { return WindowCommandButton != null; } }
        private IWindowsCommandButton WindowCommandButton;
        public UsbDrivePage()
        {
            InitializeComponent();
            this.DataContext = new UsbDriveViewModel();
            this.Loaded += UsbDrivePage_Loaded;
        }
        
        public void SetController(IWindowsCommandButton controller)
        {
            this.WindowCommandButton = controller;
        }

        private void UsbDrivePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (WindowCommandButton != null)
                WindowCommandButton.SetActive(true, false);
        }
    }
}
