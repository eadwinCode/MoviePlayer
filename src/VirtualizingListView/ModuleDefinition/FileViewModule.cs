using Common.ApplicationCommands;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualizingListView.View;

namespace VirtualizingListView.ModuleDefinition
{
    public class FileViewModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;

        public FileViewModule(IRegionManager regionManager, IUnityContainer unityContainer)
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
            this.unityContainer.RegisterType<IPageNavigatorHost, PageNavigatorHost>(new ContainerControlledLifetimeManager());
            PageNavigatorHost pagenavigatorhost = unityContainer.Resolve<IPageNavigatorHost>() as PageNavigatorHost;
            this.unityContainer.RegisterInstance(pagenavigatorhost.PageNavigator);
            this.regionManager.RegisterViewWithRegion(ApplicationRegion.MAINREGION,
                ()=>unityContainer.Resolve<IPageNavigatorHost>());
            
        }

        private void RegisterViews()
        {
            
        }
    }
}
