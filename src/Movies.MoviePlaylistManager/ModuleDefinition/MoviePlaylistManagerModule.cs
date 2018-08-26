using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.MoviePlaylistManager.ModuleDefinition
{
    public class MoviePlaylistManagerModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;

        public MoviePlaylistManagerModule(IRegionManager regionManager, IUnityContainer unityContainer)
        {
            this.regionManager = regionManager;
            this.unityContainer = unityContainer;
        }
        public void Initialize()
        {
            RegisterServices();
            RegisterView();
        }

        private void RegisterView()
        {
            
        }

        private void RegisterServices()
        {
            
        }
    }
}
