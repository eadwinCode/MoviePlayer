using Common.Interfaces;
using Common.Model;
using Common.Util;
using Meta.Vlc;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
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
        public DelegateCommand<object> SelectedAudioTrackCommand { get; private set; }
        public DelegateCommand<object> SelectedVideoTrackCommand { get; private set; }
        private WindowState IntialWindowsState;

        public VideoElementViewModel()
        {
            // SetStyleOnWindowState((Application.Current.MainWindow.WindowState));
            SelectedAudioTrackCommand = new DelegateCommand<object>(SetSelectedAudioTrack);
            SelectedVideoTrackCommand = new DelegateCommand<object>(SetSelectedVideoTrack);
        }

        private void SetSelectedAudioTrack(object parameter)
        {
            if(CurrentAudioTrack != parameter)
            {
                CurrentAudioTrack.IsActive = false;
                IVideoElement.MediaPlayer.AudioTrack = (parameter as Stream).Track.Id;
                currentAudiotrack = (Stream)parameter;
            }
        }

        private void SetSelectedVideoTrack(object parameter)
        {
            if (CurrentVideoTrack != parameter)
            {
                CurrentVideoTrack.IsActive = false;
                IVideoElement.MediaPlayer.VlcMediaPlayer.VideoTrack = (parameter as Stream).Track.Id;
                currentAudiotrack = (Stream)parameter;
            }
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
            IVideoElement.PlayListView.OnPlaylistClose += Plv_OnPlaylistClose;
            IVideoElement.IVideoPlayer.ScreenSettingsChanged += IVideoPlayer_ScreenSettingsChanged;
            MediaControlExtension.SetFileexpVisiblity(IVideoElement.PlayListView as UIElement, 
                System.Windows.Visibility.Collapsed);
            //FocusManager.SetFocusedElement(IVideoElement as DependencyObject,Mouse.Captured);

            SystemEvents.PowerModeChanged += this.SystemEvents_PowerModeChanged;
            MediaControllerVM.MediaControllerInstance.SubtitleChanged += MediaControllerInstance_SubtitleChanged;
            IVideoElement.MediaPlayer.MediaOpened += new EventHandler(MediaPlayer_MediaOpened);
            (IVideoElement as Window).Closing += new System.ComponentModel.CancelEventHandler(VideoElementViewModel_Closing);
        }

        private void VideoElementViewModel_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SystemEvents.PowerModeChanged -= this.SystemEvents_PowerModeChanged;
        }

        void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            CurrentAudioTrack = null;
            videotracks = null;
            audiotracks = null;
            CurrentVideoTrack = null;
        }

        private void MediaControllerInstance_SubtitleChanged(object sender, EventArgs e)
        {
            SubtitleTitleCollection = null;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    if (IsSuspended)
                    {
                        MediaControllerVM.MediaControllerInstance.PlayAction(); 
                        IsSuspended = false;
                    }
                    break;
                case PowerModes.StatusChange:
                    break;
                case PowerModes.Suspend:
                    if (MediaControllerVM.MediaControllerInstance.IsPlaying)
                    {
                        MediaControllerVM.MediaControllerInstance.PlayAction();
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
        private Stream currentAudiotrack;
        public Stream CurrentAudioTrack
        {
            get
            {
                return currentAudiotrack;
            }
            set { currentAudiotrack = value; RaisePropertyChanged(() => this.CurrentAudioTrack); }
        }

        private Stream currentVideotrack;
        public Stream CurrentVideoTrack
        {
            get
            {
                return currentVideotrack;
            }
            set { currentVideotrack = value; RaisePropertyChanged(() => this.CurrentVideoTrack); }
        }

        private List<Stream> audiotracks;
        public List<Stream> AudioTracks
        {
            get
            {
                if (audiotracks == null)
                {
                    audiotracks = new List<Stream>();
                    foreach (var item in IVideoElement.MediaPlayer.AudioTrackDescription)
                    {
                        Stream astrack = new Stream(item, TrackType.Audio);
                        audiotracks.Add(astrack);
                        if (item.Id == IVideoElement.MediaPlayer.AudioTrackDescription[IVideoElement.MediaPlayer.AudioTrack].Id)
                            CurrentAudioTrack = astrack;
                    }
                }
                return audiotracks;

            }
        }

        private List<Stream> videotracks;
        public List<Stream> VideoTracks
        {
            get
            {
                if (videotracks == null)
                {
                    videotracks = new List<Stream>();
                    foreach (var item in IVideoElement.MediaPlayer.VlcMediaPlayer.VideoTrackDescription)
                    {
                        Stream astrack = new Stream(item, TrackType.Video);
                        videotracks.Add(astrack);
                        if (item.Id == IVideoElement.MediaPlayer.VlcMediaPlayer.VideoTrackDescription[IVideoElement.MediaPlayer.VlcMediaPlayer.VideoTrack].Id)
                            CurrentVideoTrack = astrack;
                    }
                }
                return videotracks;

            }
        }

        private ObservableCollection<SubtitleFilesModel> subtitletitlecollection;
        public ObservableCollection<SubtitleFilesModel> SubtitleTitleCollection
        {
            get
            {
                if (subtitletitlecollection == null)
                    subtitletitlecollection = new ObservableCollection<SubtitleFilesModel>(MediaControllerVM.
                        MediaControllerInstance.CurrentVideoItem.SubPath);
                return subtitletitlecollection;
            }
            private set
            {
                subtitletitlecollection = value; RaisePropertyChanged(() => this.SubtitleTitleCollection);
            }
        }
    }
}
