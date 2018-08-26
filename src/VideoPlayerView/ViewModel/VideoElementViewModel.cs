using Common.Util;
using Meta.Vlc;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Movies.MediaService.Interfaces;
using Movies.MediaService.Models;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using VideoComponent.BaseClass;
using VideoPlayerControl;
using VideoPlayerControl.ViewModel;
using VideoPlayerView.Model;

namespace VideoPlayerView.ViewModel
{
    public partial class VideoElementViewModel:NotificationObject
    {
        private WindowState IntialWindowsState;

        public VideoElementViewModel()
        {
            // SetStyleOnWindowState((Application.Current.MainWindow.WindowState));
        }

        private string maxbtntooltip;
        public string MaxbtnTooltip
        {
            get { return maxbtntooltip; }
            private set { maxbtntooltip = value; this.RaisePropertyChanged(() => this.MaxbtnTooltip); }
        }

        void Plv_OnPlaylistClose(object sender, EventArgs e)
        {
            if (MediaControlExtension.GetFileexpVisiblity(IVideoElement.PlayListView as UIElement) == System.Windows.Visibility.Visible)
            {
                MediaControlExtension.SetFileexpVisiblity(IVideoElement.PlayListView as UIElement, System.Windows.Visibility.Collapsed);
            }
            else
            {
                MediaControlExtension.SetFileexpVisiblity(IVideoElement.PlayListView as UIElement, System.Windows.Visibility.Visible);
            }
        }

        private Visibility fullscreenbtn = Visibility.Collapsed;
        private bool IsSuspended;

        public Visibility FullScreenBtn
        {
            get { return fullscreenbtn ; }
            set { fullscreenbtn = value;RaisePropertyChanged(() => this.FullScreenBtn); }
        }
        
        internal void Loaded()
        {
            //remember to fix dis
            IVideoElement.PlayListView.OnPlaylistClose += Plv_OnPlaylistClose;
            IVideoElement.IVideoPlayerController.ScreenSettingsChanged += IVideoPlayer_ScreenSettingsChanged;
            MediaControlExtension.SetFileexpVisiblity(IVideoElement.PlayListView as UIElement,
                System.Windows.Visibility.Collapsed);

            //FocusManager.SetFocusedElement(IVideoElement as DependencyObject,Mouse.Captured);

            SystemEvents.PowerModeChanged += this.SystemEvents_PowerModeChanged;
           FilePlayerManager.MediaControllerViewModel.SubtitleChanged += MediaControllerInstance_SubtitleChanged;
            MediaPlayerService.OnMediaOpened += new EventHandler(MediaPlayer_MediaOpened);
            (IVideoElement as Window).Closing += new System.ComponentModel.CancelEventHandler(VideoElementViewModel_Closing);
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
        }

        private void VideoElementViewModel_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SystemEvents.PowerModeChanged -= this.SystemEvents_PowerModeChanged;
        }
        

        private void MediaControllerInstance_SubtitleChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(() => this.SubtitleTitleCollection);
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    if (IsSuspended)
                    {
                       FilePlayerManager.MediaControllerViewModel.PlayAction(); 
                        IsSuspended = false;
                    }
                    break;
                case PowerModes.StatusChange:
                    break;
                case PowerModes.Suspend:
                    if (FilePlayerManager.MediaControllerViewModel.IsPlaying)
                    {
                        FilePlayerManager.MediaControllerViewModel.PlayAction();
                        IsSuspended = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void IVideoPlayer_ScreenSettingsChanged(object sender, EventArgs e)
        {
            var args = sender as object[];
            if(args[0] == null)
            {
                if (FullScreenBtn == Visibility.Visible)
                {
                    FullScreenBtn = Visibility.Collapsed;
                }
                else
                {
                    FullScreenBtn = Visibility.Visible;
                }

                return;
            }

            if (((SCREENSETTINGS)args[0]) == SCREENSETTINGS.Fullscreen)
            {
                FullScreenBtn = Visibility.Visible;
               // IVideoElement.WindowsTab.Visibility = Visibility.Collapsed;
                IntialWindowsState = (IVideoElement as Window).WindowState;
                (IVideoElement as Window).WindowState = WindowState.Maximized;
            }
            else if(((SCREENSETTINGS)args[0]) == SCREENSETTINGS.Normal)
            {
                FullScreenBtn = Visibility.Collapsed;
                //IVideoElement.WindowsTab.Visibility = Visibility.Visible;
                (IVideoElement as Window).WindowState = IntialWindowsState;
            }
          //  }
        }

        private IVideoElement IVideoElement
        {
            get { return ServiceLocator.Current.GetInstance<IPlayFile>().VideoElement; }
        }
        
    }

    public partial class VideoElementViewModel
    {
       
       // private List<Stream> audiotracks;
        public IEnumerable<MediaTrackDescription> AudioTracks
        {
            get
            {
                return (MediaPlayerService.AudioTracksManagement.AudioTracks);
            }
        }

        //private List<Stream> videotracks;
        public IEnumerable<MediaTrackDescription> VideoTracks
        {
            get
            {
                return (MediaPlayerService.VideoTracksManagement.VideoTracks);
            }
        }
        
        public IEnumerable<MediaTrackDescription> SubtitleTitleCollection
        {
            get
            {
                return (MediaPlayerService.SubtitleManagement.SubtitleList);
            }
        }


        IMediaPlayerService MediaPlayerService
        {
            get
            {
                return FilePlayerManager.MediaPlayerService;
            }
        }

        IPlayFile FilePlayerManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }
    }
}
