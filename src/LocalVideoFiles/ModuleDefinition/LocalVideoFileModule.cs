using Common.ApplicationCommands;
using LocalVideoFiles.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace VirtualizingListView.ModuleDefinition
{
    public class LocalVideoFileModule : IModule
    {
        IRegionManager regionManager;
        IUnityContainer unityContainer;

        public LocalVideoFileModule(IRegionManager regionManager, IUnityContainer unityContainer)
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
            this.regionManager.RegisterViewWithRegion(ApplicationRegion.SUBMENUITEMSREGION, 
                typeof(VideoFolderView));
        }
    }
}
