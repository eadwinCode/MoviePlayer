using Common.ApplicationCommands;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Movies.MoviesInterfaces;
using Movies.StatusService.Services;
using Movies.StatusService.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.StatusService.ModuleDefinition
{
    public class StatusServiceModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;

        public StatusServiceModule(IRegionManager regionManager, IUnityContainer unityContainer)
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
            unityContainer.RegisterType<IStatusMessageManager,StatusManager>(new ContainerControlledLifetimeManager());
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion(ApplicationRegion.STATUSBARREGION, typeof(MessageView));
        }
    }
}
