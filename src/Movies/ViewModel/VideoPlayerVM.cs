using Common.ApplicationCommands;
using Common.Util;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Services;
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
    public class VideoPlayerVM:NotificationObject
    {
        private Visibility windowscontrol;
        IApplicationService ApplicationService;
        private double windowsheight;
        private Visibility _Sliderthumb;

        public Visibility WindowsControl
        {
            get { return windowscontrol; }
            set { windowscontrol = value; RaisePropertyChanged(() => this.WindowsControl); }
        }
     
        public double WindowsHeight
        {
            get { return windowsheight; }
            set { windowsheight = value; RaisePropertyChanged(() => this.WindowsHeight); }
        }
        
        public Visibility Sliderthumb
        {
            get { return _Sliderthumb; }
            set
            {
                _Sliderthumb = value;
                this.RaisePropertyChanged(() => this.Sliderthumb);
            }
        }

        public string Subtitletext { get; set; }

        public void SetST(string sub)
        {
            Subtitletext = sub;
            this.RaisePropertyChanged(() => this.Subtitletext);
            Sliderthumb = Visibility.Collapsed;
        }
        
        public VideoPlayerVM(IApplicationService applicationService)
        {
            WindowsControl = Visibility.Visible;
            this.ApplicationService = applicationService;
        }
        
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CommandActions CommandAction = new CommandActions(ApplicationService);
            CommandAction.RegisterCommands();
            (IShell as Window).Closing += VideoPlayerVM_Closing;
        }
        
        private void VideoPlayerVM_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ApplicationService.SaveFiles();
        }

        private IShell IShell
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShell>();
            }
        }
    }
}
