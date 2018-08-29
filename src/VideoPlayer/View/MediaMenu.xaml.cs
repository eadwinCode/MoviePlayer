using Microsoft.Practices.ServiceLocation;
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
using VideoPlayerControl.ViewModel;

namespace VideoPlayerControl.View
{
    /// <summary>
    /// Interaction logic for MediaMenu.xaml
    /// </summary>
    public partial class MediaMenu : UserControl
    {
        IPlayFile FilePlayerManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }

        public MediaMenu()
        {
            InitializeComponent();
        }

        public void ShowDialog()
        {
            FilePlayerManager.VideoElement.ContentDockRegion.Content = this;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            FilePlayerManager.VideoElement.ContentDockRegion.Content = null;
        }
    }
}
