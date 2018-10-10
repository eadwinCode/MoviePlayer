using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Common.ApplicationCommands;
using Movies;
using Movies.MoviesInterfaces;
using Movies.Services;
using Microsoft.Practices.Prism.Logging;
using PresentationExtension.EventManager;
using PresentationExtension.InterFaces;
using Movies.Themes.Service;
using RealMediaControl.ViewModel;
using FlyoutControl.PageNagivatorService;

namespace RealMediaControl
{
    public class Bootstrapper : UnityBootstrapper
    {

        //protected override IModuleCatalog CreateModuleCatalog()
        //{
        //    var moduleCatalog = new DirectoryModuleCatalog();
        //    //moduleCatalog.ModulePath = @".\Modules";
        //    //moduleCatalog.ModulePath = AppDomain.CurrentDomain.BaseDirectory + "Modules";
        //    return moduleCatalog;
        //}
        protected override ILoggerFacade CreateLogger()
        {
            return base.CreateLogger();
        }
        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ProjectConfiguration projectConfiguration = new ProjectConfiguration();
            foreach (ModuleInfo catalog in projectConfiguration.GetModuleInfo())
            {
                ModuleCatalog.AddModule(catalog);
            }
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings mappings = base.ConfigureRegionAdapterMappings();
            return mappings;
        }

        protected override void ConfigureContainer()
        {
            this.Container.RegisterType<IShellWindowService, ShellWindowService>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<IShellWindow, MainView>(new ContainerControlledLifetimeManager());
            base.ConfigureContainer();
            this.Container.RegisterType<IEventManager, MovieEventManager>(new ContainerControlledLifetimeManager());
        }

        protected override DependencyObject CreateShell()
        {
            SplashScreenWindow splashscreenwindow = new SplashScreenWindow();
            splashscreenwindow.Show();

            IModule thememodule = Container.Resolve<ThemeModule>();
            thememodule.Initialize();
            Container.RegisterType<IPageNavigatorHost, PageNavigatorHost>(new ContainerControlledLifetimeManager());

            base.InitializeModules();

            PageNavigatorHost pagenavigatorhost = Container.Resolve<IPageNavigatorHost>() as PageNavigatorHost;
            this.Container.RegisterInstance(pagenavigatorhost.PageNavigator);

            var shell = this.Container.Resolve<IShellWindow>() as MainView;
            shell.Dispatcher.BeginInvoke((Action)delegate
            {
                //shell.Show();
                splashscreenwindow.Close();
            });
            return shell;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }
    }
}
