using Common.ApplicationCommands;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Movies.MoviesInterfaces;
using Movies.PlaylistCollectionView.Views;
using PresentationExtension;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.PlaylistCollectionView.ViewModels
{
    public class HomePlaylistButtonViewModel : NotificationObject
    {
        private IHomePlaylist HomePlaylistService;
        IEventManager EventManager;
        private DelegateCommand<WindowCommandButton> injectviewcommand;
        public DelegateCommand<WindowCommandButton> InjectViewCommand
        {
            get
            {
                if (injectviewcommand == null)
                    injectviewcommand = new DelegateCommand<WindowCommandButton>((o) =>
                    {
                        var data = o;
                        data.SetActive(true, true);
                    });
                return injectviewcommand;
            }
        }
        public HomePlaylistButtonViewModel(IEventManager eventmanager, IHomePlaylist  homePlaylistmanager)
        {
            this.HomePlaylistService = homePlaylistmanager;
            EventManager = eventmanager;
            InitHamBurgerMenu();
        }

        private void InjectViewHandler()
        {
            EventManager.GetEvent<NavigateNewPage>().Publish(HomePlaylistService.GetHomePlaylistView());
        }

        private HamburgerMenuIconItem usbDrive;

        public HamburgerMenuIconItem HomePlaylist
        {
            get { return usbDrive; }
            set { usbDrive = value; this.RaisePropertyChanged(() => this.HomePlaylist); }
        }

        private void InitHamBurgerMenu()
        {

            HomePlaylist = new HamburgerMenuIconItem()
            {
                Label = "Manage Playlist",
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.PlaylistPlay },
                Tag = HomePlaylistService.GetHomePlaylistView()
            };
        }

    }
    
}
