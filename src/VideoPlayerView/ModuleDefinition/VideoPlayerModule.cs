using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using VideoPlayerView.FilePlayer;

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
            this.unityContainer.RegisterType<IPlayFile, PlayFile>(new ContainerControlledLifetimeManager());
        }

        private void RegisterViews()
        {
            IPlayFile playFile = new PlayFile();
            var initPlayer = playFile.MediaPlayerService.VideoPlayer;
            playFile = null;
            initPlayer = null;
        }
    }
}
