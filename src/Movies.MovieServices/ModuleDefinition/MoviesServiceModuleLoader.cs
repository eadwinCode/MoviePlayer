using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MovieServices.Services;
using Movies.MovieServices.Threading;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.MovieServices.ModuleDefinition
{
    public class MoviesServiceModuleLoader : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;

        public MoviesServiceModuleLoader(IRegionManager regionManager, IUnityContainer unityContainer)
        {
            this.regionManager = regionManager;
            this.unityContainer = unityContainer;
        }
        public void Initialize()
        {
            RegisterServices();
        }

        private void RegisterServices()
        {
            ApplicationService applicationService = new ApplicationService();

            this.unityContainer.RegisterType<IBackgroundService, ExecutorImpl>(new ContainerControlledLifetimeManager());
            this.unityContainer.RegisterInstance<IApplicationService>(applicationService);
            this.unityContainer.RegisterInstance<IApplicationModelService>(applicationService);
            bool singleton = ReferenceEquals(unityContainer.Resolve<IApplicationService>(), unityContainer.Resolve<IApplicationModelService>());
            this.unityContainer.RegisterType<IFileLoader, FileLoader>(new ContainerControlledLifetimeManager());
            this.unityContainer.RegisterType<IFileExplorerCommonHelper, FileExplorerCommonHelper>();
            this.unityContainer.RegisterType<IFileLoaderCompletion, FileLoaderCompletion>();
            this.unityContainer.RegisterType<IDataSource<VideoFolder>,MovieDataStore>
                (new ContainerControlledLifetimeManager());
            this.unityContainer.RegisterType<IDispatcherService, DispatcherService>(new ContainerControlledLifetimeManager());
            this.unityContainer.RegisterType<IOpenFileCaller, OpenFileCaller>();

            applicationService.CreateFolder();
            applicationService.LoadFiles();
        }
        
    }
}
