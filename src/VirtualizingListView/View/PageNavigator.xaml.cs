using Movies.MoviesInterfaces;
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
using VirtualizingListView.ViewModel;

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for PageNavigator.xaml
    /// </summary>
    public partial class PageNavigator : UserControl,INavigatorService
    {
        public PageNavigator()
        {
            InitializeComponent();
            this.DataContext = new PageNavigatorViewModel(this);
        }

        public NavigationService NavigationService
        {
            get { return frameNavigator.NavigationService; }
        }

        public ContentControl DockControl
        {
            get
            {
                return this.DialogDock;
            }
        }

        public Frame Host
        {
            get { return frameNavigator; }
        }
    }
}
