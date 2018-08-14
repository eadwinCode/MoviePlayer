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

namespace VirtualizingListView.Pages.Views
{
    /// <summary>
    /// Interaction logic for UsbNoFOlder.xaml
    /// </summary>
    public partial class UsbNoFolder : UserControl
    {
        ContentControl ParentController;
        public event RoutedEventHandler OnFinished;

        public UsbNoFolder(ContentControl contentControl)
        {
            InitializeComponent();
            ParentController = contentControl;
        }
        
        public void Show()
        {
            ParentController.Content = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OnFinished != null)
                OnFinished(this, new RoutedEventArgs());

            ParentController.Content = null;
        }
    }
}
