using Common.ApplicationCommands;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Movies.Models.Interfaces;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using VideoPlayerView.FilePlayer;
using VideoPlayerView.interfaces;
using VideoPlayerView.View;

namespace VideoPlayerView.ModuleDefinition
{
    public class VideoPlayerModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;

        public VideoPlayerModule(IRegionManager regionManager, IUnityContainer unityContainer)
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
            PlayFile playFile = new PlayFile();
            this.unityContainer.RegisterInstance<IPlayFile>(playFile);
            this.unityContainer.Resolve<IMediaPlayerHostCollection>().Add(playFile);
            this.unityContainer.RegisterType<IHomeControl, DefaultPlayerControl>(new ContainerControlledLifetimeManager());

            regionManager.RegisterViewWithRegion(ApplicationRegion.STATUSCONTROLREGION,
                () => unityContainer.Resolve<IHomeControl>());
        }

        private void RegisterViews()
        {
            var filepilemanager = unityContainer.Resolve<IPlayFile>();
            filepilemanager.PrepareVideoElement();
        }
    }
}
