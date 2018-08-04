using Common.Interfaces;
using SearchComponent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VideoComponent.BaseClass;
using VirtualizingListView.Model;
using VirtualizingListView.ViewModel;

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for FileView.xaml
    /// </summary>
    public partial class FileView : UserControl, IPageNavigatorHost
    {
        private SearchControl<VideoFolder> SearchControl;
        public INavigatorService PageNavigator { get { return this.pagenavigator; } }

        public ISearchControl GetSearchControl
        {
            get;set;
        }
        public ContentControl DockControl
        {
            get
            {
                return this.DialogDock;
            }
        }
        public FileView()
        {
            InitializeComponent();
            this.DataContext = new FileViewViewModel(this);
        }

        private void WindowCommandButton_Click(object sender, RoutedEventArgs e)
        {
            WindowCommandButton windowCommandButton = (WindowCommandButton)sender;
            windowCommandButton.SetActive(true, true);
        }

        private void WindowCommandButton_Loaded(object sender, RoutedEventArgs e)
        {
            WindowCommandButton windowCommandButton = sender as WindowCommandButton;
            windowCommandButton.SetActive(true, true);
        }
    }
}
