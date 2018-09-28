using Common.ApplicationCommands;
using Common.Util;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Services;
using Movies.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using VideoComponent.BaseClass;

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

        public ShellWindowService(IApplicationService applicationService, IRegionManager regionManager)
        {
            this.ApplicationService = applicationService;
            this.RegionManager = regionManager;
            _views = new Dictionary<string, object>();
            this.AddView(new MainShellView(), typeof(MainShellView).Name);
        }
        
        
        private void VideoPlayerVM_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
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

        public MetroWindow ShellWindow
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShellWindow>() as MetroWindow;
            }
        }
    }
}
