using Common.ApplicationCommands;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualizingListView.Pages.ViewModel;
using VirtualizingListView.View;

namespace VirtualizingListView.ModuleDefinition
{
    public class FileViewModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;
        static PageEventHost pageEventHost;
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
            pageEventHost = new PageEventHost();
            unityContainer.Resolve<IShellWindowService>().RegisterMenu(new MyVideoPageMenu());
        }

        private void RegisterViews()
        {
            
        }
    }
}
