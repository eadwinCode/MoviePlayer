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
            this.unityContainer.RegisterType<IPageNavigatorHost, FileView>(new ContainerControlledLifetimeManager());
            FileView fileView = unityContainer.Resolve<IPageNavigatorHost>() as FileView;
            this.unityContainer.RegisterInstance(fileView.PageNavigator);
            this.regionManager.RegisterViewWithRegion(ApplicationRegion.MAINREGION,
                ()=>unityContainer.Resolve<IPageNavigatorHost>());
            
        }

        private void RegisterViews()
        {
            
        }
    }
}
