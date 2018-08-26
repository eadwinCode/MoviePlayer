using Common.ApplicationCommands;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using SearchComponent.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchComponent.ModuleDefinition
{
    public class SearchControlModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;

        public SearchControlModule(IRegionManager regionManager, IUnityContainer unityContainer)
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
            this.unityContainer.RegisterType<ISearchControl<VideoFolder>,SearchControl<VideoFolder>>
                (new ContainerControlledLifetimeManager());
        }

        private void RegisterViews()
        {
            this.regionManager.RegisterViewWithRegion(ApplicationRegion.SHELLSEARCHREGION,
               typeof(ShellSearchButton));
        }
    }
}
