using FlyoutControl;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
using Movies.Services;
using Movies.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RealMediaControl.ViewModel
{
    public class ShellWindowService:NotificationObject, IShellWindowService
    {
        IApplicationService ApplicationService;
        private IRegionManager RegionManager;
        IDictionary<string, Object> _views;
        private object currentview;

        public object CurrentView
        {
            get { return currentview; }
        }

        private IList<FlyoutSubMenuItem> flyoutsubmenuitem;

        public IList<FlyoutSubMenuItem> FlyoutSubMenuItem
        {
            get { return flyoutsubmenuitem; }
        }


        public ShellWindowService(IApplicationService applicationService, IRegionManager regionManager)
        {
            this.ApplicationService = applicationService;
            this.RegionManager = regionManager;
            _views = new Dictionary<string, object>();
            flyoutsubmenuitem = new ObservableCollection<FlyoutSubMenuItem>();
            this.AddView(new MainShellView(), typeof(MainShellView).Name);
        }
        
        
        private void VideoPlayerVM_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var imediaHost = ServiceLocator.Current.GetInstance<IMediaPlayerHostCollection>();
            foreach (var item in imediaHost)
            {
                item.ShutDown();
            }
            ApplicationService.SaveFiles();
        }

        public object GetView(string ViewName)
        {
            object view = null;
            _views.TryGetValue(ViewName, out view);
            return view;
        }

        public void OnWindowsLoaded()
        {
            CommandActions CommandAction = new CommandActions(ApplicationService);
            CommandAction.RegisterCommands();
            (ShellWindow as Window).Closing += VideoPlayerVM_Closing;
        }

        public void AddView(object view, string uniqueName)
        {
            if (!_views.ContainsKey(uniqueName))
            {
                _views.Add(uniqueName, view);
                UpdateCurrentView();
            }
        }

        public void RemoveView(object view)
        {
          //  _views.Remove(view);
        }

        public void RemoveView(string ViewName)
        {
            _views.Remove(ViewName);

            UpdateCurrentView();
        }

        private void UpdateCurrentView()
        {
            currentview = _views.LastOrDefault().Value;
            RaisePropertyChanged(() => this.CurrentView);
        }

        public void RegisterMenu(object flyoutSubMenuItem)
        {
            FlyoutSubMenuItem item = flyoutSubMenuItem as FlyoutSubMenuItem;
            if(item != null)
            {
                flyoutsubmenuitem.Add(item);
            }
        }

        public void RegisterMenuAt(object flyoutSubMenuItem, int Position)
        {
            FlyoutSubMenuItem item = flyoutSubMenuItem as FlyoutSubMenuItem;
            if (item != null)
            {
                int index = Position;
                if (Position > 0)
                    index = Position - 1;
                flyoutsubmenuitem.Insert(index, item);
            }
        }

        public MetroWindow ShellWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShellWindow>() as MetroWindow;
            }
        }
    }
}
