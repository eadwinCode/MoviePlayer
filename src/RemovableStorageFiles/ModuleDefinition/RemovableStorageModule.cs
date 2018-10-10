﻿using Common.ApplicationCommands;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Movies.MoviesInterfaces;
using RemovableStorageFiles.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualizingListView.ModuleDefinition
{
    public class RemovableStorageModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;

        public RemovableStorageModule(IRegionManager regionManager, IUnityContainer unityContainer)
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
            //this.unityContainer.RegisterType<IPageNavigatorHost, FileView>(new ContainerControlledLifetimeManager());

            //FileView fileView = unityContainer.Resolve<IPageNavigatorHost>() as FileView;

            //this.unityContainer.RegisterInstance(fileView.PageNavigator);
            //this.regionManager.RegisterViewWithRegion(ApplicationRegion.MAINREGION,
            //    ()=>unityContainer.Resolve<IPageNavigatorHost>());
        }

        private void RegisterViews()
        {
            unityContainer.Resolve<IShellWindowService>().RegisterMenu(new RemovableStorageFilesMenu());
            //this.regionManager.RegisterViewWithRegion(ApplicationRegion.SUBMENUITEMSREGION, 
            //    typeof(RemovableStorageView));
        }
    }
}
