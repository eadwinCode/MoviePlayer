using Movies.PlaylistCollectionView.ViewModels;
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

namespace Movies.PlaylistCollectionView.Views
{
    /// <summary>
    /// Interaction logic for HomePlaylistButton.xaml
    /// </summary>
    public partial class HomePlaylistButton : UserControl
    {
        public HomePlaylistButton(HomePlaylistButtonViewModel homePlaylistButtonViewModel)
        {
            InitializeComponent();
            this.DataContext = homePlaylistButtonViewModel;
        }
    }
}
