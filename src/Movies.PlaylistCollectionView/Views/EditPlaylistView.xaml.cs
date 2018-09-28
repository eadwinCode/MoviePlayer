using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
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
    /// Interaction logic for EditPlaylistView.xaml
    /// </summary>
    public partial class EditPlaylistView : UserControl
    {
        public EditPlaylistView(PlaylistModel playlistModel)
        {
            InitializeComponent();
           var vm  = new EditPlayViewModel(playlistModel);
            this.DataContext = vm;
            vm.CloseAction += () => {
                CloseView();
            };

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseView();
        }

        private void CloseView()
        {
            PageNavigatorHost.RemoveView(typeof(EditPlaylistView).Name);
        }

        public void ShowDialog()
        {
            PageNavigatorHost.AddView(this,typeof(EditPlaylistView).Name);
        }

        IPageNavigatorHost PageNavigatorHost
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPageNavigatorHost>();
            }
        }
    }
}
