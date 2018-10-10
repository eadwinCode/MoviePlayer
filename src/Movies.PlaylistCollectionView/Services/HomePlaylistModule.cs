using Common.ApplicationCommands;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Movies.MoviesInterfaces;
using Movies.PlaylistCollectionView.ViewModels;
using Movies.PlaylistCollectionView.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.PlaylistCollectionView.Services
{
    public class HomePlaylistModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;

        public HomePlaylistModule(IRegionManager regionManager, IUnityContainer unityContainer)
        {
            this.regionManager = regionManager;
            this.unityContainer = unityContainer;
        }
        public void Initialize()
        {
            RegisterServices();
            RegisterViews();
        }

        private void RegisterServices()
        {
            unityContainer.RegisterType<IHomePlaylist, HomePlaylistViewModel>(new ContainerControlledLifetimeManager());
        }

        private void RegisterViews()
        {
            unityContainer.Resolve<IShellWindowService>().RegisterMenu(new HomePlaylistMenu());
           // regionManager.RegisterViewWithRegion(ApplicationRegion.SHELLPLAYLISTREGION, typeof(HomePlaylistButton));
        }
    }
}
