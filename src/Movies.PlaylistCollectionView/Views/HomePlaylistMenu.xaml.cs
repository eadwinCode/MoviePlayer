using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
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

namespace Movies.PlaylistCollectionView.Views
{
    /// <summary>
    /// Interaction logic for HomePlaylistMenu.xaml
    /// </summary>
    public partial class HomePlaylistMenu
    {
        public HomePlaylistMenu()
        {

            InitializeComponent();
            this.DataContext = new HomePlaylistMenuViewModel();

        }

        public class HomePlaylistMenuViewModel
        {
            private IHomePlaylist HomePlaylistService
            {
                get
                {
                    return ServiceLocator.Current.GetInstance<IHomePlaylist>();
                }
            }

            private INavigatorService INavigatorService
            {
                get
                {
                    return ServiceLocator.Current.GetInstance<INavigatorService>();
                }
            }

            private DelegateCommand loadpageCommand;
            public DelegateCommand LoadPageCommand
            {
                get
                {
                    if (loadpageCommand == null)
                    {
                        loadpageCommand = new DelegateCommand(LoadPageAction);
                    }
                    return loadpageCommand;
                }
            }

            public object Icon { get; private set; }

            public HomePlaylistMenuViewModel()
            {
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.PlaylistPlay };
            }

            private void LoadPageAction()
            {
                INavigatorService.NavigateMainPage(HomePlaylistService.GetHomePlaylistView() as IMainPage);
            }
        }
    }
}
