using Common.ApplicationCommands;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using MovieHub.MediaPlayerElement;
using MovieHub.MediaPlayerElement.Models;
using MovieHub.MediaPlayerElement.Service;
using Movies.InternetRadio.StreamManager;
using Movies.InternetRadio.Util;
using Movies.InternetRadio.ViewModels;
using Movies.InternetRadio.Views;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Movies.InternetRadio
{
    public class RadioPlayer : MovieBase
    {
        private NetworkConnectionStatus networkconnectionstatus;
        private RadioStationDetails radiostationdetails;
        private RadioModel currentradiostation;
        private DelegateCommand closemediacontrol;
        private MediaPlayerElement mediaplayerelement;
        private bool Isclosing;
        private RadioStreamHomePageControl radioPlayerView;
        private static DelegateCommand starcommand;
        
        private void StarCommandAction()
        {
            RadioHomePageService.CheckFavoriteItemCommand.Execute(CurrentRadioStation, (IRadioService as RadioService).RadioHomepage as Page);
        }

        IRadioService IRadioService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioService>();
            }
        }

        IPageNavigatorHost PageNavigatorHost
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPageNavigatorHost>();
            }
        }

        private IShellWindowService ShellWindowService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShellWindowService>();
            }
        }

        public NetworkConnectionStatus NetworkConnectionStatus 
        {
            get { return networkconnectionstatus; }
            internal set { networkconnectionstatus = value; }
        }

        public RadioModel CurrentRadioStation
        {
            get { return currentradiostation; }
            internal set { currentradiostation = value; RaisePropertyChanged(() => this.CurrentRadioStation); }
        }
        
        public RadioStationDetails RadioStationDetails
        {
            get { return radiostationdetails; }
            internal set { radiostationdetails = value; RaisePropertyChanged(() => this.RadioStationDetails); }
        }

        public MediaPlayerElement MediaPlayerElement
        {
            get { return mediaplayerelement; }
            internal set { mediaplayerelement = value; RaisePropertyChanged(() => this.MediaPlayerElement); }
        }

        private MediaInformation _mediainformation;

        public MediaInformation MediaInformation
        {
            get { return _mediainformation; }
            set { _mediainformation = value; RaisePropertyChanged(() => this.MediaInformation); }
        }
        
        public DelegateCommand CloseMediaControl
        {
            get
            {
                if (closemediacontrol == null)
                {
                    closemediacontrol = new DelegateCommand(() =>
                    {
                        (IRadioService as RadioService)._idispatcherService.ExecuteTimerAction(()=> IRadioService.ShutDown(),50);
                    });
                }

                return closemediacontrol;
            }
        }

        public static DelegateCommand StarCommand
        {
            get
            {
                return starcommand;
            }
        }

        internal RadioPlayer()
        {
            RegisterCommand();
            radioPlayerView = new RadioStreamHomePageControl()
            {
                DataContext = this,
            };
            RadioStationDetails = new RadioStationDetails() { DataContext = this };
            NetworkConnectionStatus = new NetworkConnectionStatus();
            InitMediaPlayerService();

            this.Content = radioPlayerView;
            this.Unloaded += (s, e) => { UnloadedRadioPlayer(); };
        }

        private void RegisterCommand()
        {
            starcommand = new DelegateCommand(StarCommandAction);
            //this.CommandBindings.Add(new CommandBinding(starcommand, ));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.MediaPlayerElement.Focus();
        }

        private void UnloadedRadioPlayer()
        {
            
        }

        private void InitMediaPlayerService()
        {
            MediaPlayerElement = new MediaPlayerElement
            {
                CanMediaFastForwardOrRewind = false,
                AllowMovieControlAnimation = false,
                AllowMediaPlayerAutoDispose = false,
                IsMediaContextMenuEnabled = false,
                MediaPlayerViewType = MediaPlayerViewType.MiniMediaPanel
            };
            var control = MediaPlayerElement.MovieControl;
            control.ControlViewType = MediaControlViewType.MiniView;
            control.IsMediaOptionToggleEnabled = false;
            control.IsShowMenuToggleEnabled = false;
            control.IsFullScreenToggleEnabled = false;
            control.DisableMovieBoardText = true;
            control.MediaDurationDisplayVisible = false;
            control.IsMediaSliderEnabled = false;

            MediaPlayerElement.OnMediaTitleChanged += MediaPlayerElement_OnMediaTitleChanged;
            MediaPlayerElement.OnMediaInfoChanged += RadioPlayer_OnMediaInfoChanged;
        }

        private void RadioPlayer_OnMediaInfoChanged(object sender, MediaInfoChangedEventArgs e)
        {
            MediaInformation = e.MediaInformation;
        }

        private void MediaPlayerElement_OnMediaTitleChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ShellWindowService.ShellWindow.Title = string.Format("{0} - Radio", MediaPlayerElement.MediaTitle);
        }

        private void DestroyHomePageView()
        {
            this.Content = null;
            PageNavigatorHost.RemoveView(typeof(RadioPlayer).Name);
            MediaPlayerElement.Dispose();
            MediaPlayerElement = null;
            this.ShellWindowService.ShellWindow.Title = ApplicationConstants.SHELLWINDOWTITLE;
            this.CurrentRadioStation.IsActive = false;
        }
        
        public void PlayStation(RadioModel radiostation)
        {
            CurrentRadioStation = radiostation;
            MediaInformation = null;
            if (!string.IsNullOrEmpty(radiostation.StationURL))
            {
                var url = radiostation.StationURL;
                MediaPlayerElement.Source(radiostation);
            }
        }

        public void HostViewInHomePage()
        {
            PageNavigatorHost.AddView(this, typeof(RadioPlayer).Name);
        }

        public void Shutdown()
        {
            if (!Isclosing)
            {
                Isclosing = true;
                DestroyHomePageView();
                Isclosing = false;
            }
        }
        
    }
}
