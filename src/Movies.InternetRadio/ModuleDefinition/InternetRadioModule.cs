using Common.ApplicationCommands;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Movies.InternetRadio.StreamManager;
using Movies.InternetRadio.Views;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.InternetRadio.ModuleDefinition
{
    public class InternetRadioModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;

        public InternetRadioModule(IRegionManager regionManager, IUnityContainer unityContainer)
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
            this.unityContainer.RegisterType<IRadioDataService, RadioDataService>(new ContainerControlledLifetimeManager());
            this.unityContainer.RegisterType<IRadioService, RadioService>(new ContainerControlledLifetimeManager());
            this.unityContainer.Resolve<IMediaPlayerHostCollection>().Add(unityContainer.Resolve<IRadioService>());
        }

        private void RegisterViews()
        {
            unityContainer.Resolve<IShellWindowService>().RegisterMenuAt(new RadioPageMenu(),4);

            //this.regionManager.RegisterViewWithRegion(ApplicationRegion.SHELLRADIOREGION,
            //    typeof(RadioStreamToggle));
        }
    }
}
