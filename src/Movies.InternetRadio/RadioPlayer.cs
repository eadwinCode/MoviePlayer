using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using MovieHub.MediaPlayerElement;
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
        private DelegateCommand starcommand;

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

        public DelegateCommand CloseMediaControl
        {
            get
            {
                if (closemediacontrol == null)
                {
                    closemediacontrol = new DelegateCommand(() =>
                    {
                        (IRadioService as RadioService)._idispatcherService.ExecuteTimerAction(()=> IRadioService.ShutdownRadio(),50);
                    });
                }

                return closemediacontrol;
            }
        }

        public DelegateCommand StarCommand
        {
            get
            {
                if (starcommand == null)
                {
                    starcommand = new DelegateCommand(() =>
                    {
                        RadioHomePageService.CheckFavoriteItemCommand.Execute(CurrentRadioStation, (IRadioService as RadioService).RadioHomepage as Page);
                    });
                }

                return starcommand;
            }
        }

        internal RadioPlayer()
        {
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
        }

        private void DestroyHomePageView()
        {
            this.Content = null;
            PageNavigatorHost.RemoveView(typeof(RadioPlayer).Name);
            MediaPlayerElement.Dispose();
            MediaPlayerElement = null;
        }
        
        public void PlayStation(RadioModel radiostation)
        {
            CurrentRadioStation = radiostation;

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
