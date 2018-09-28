using Common.Util;
using Microsoft.Practices.Prism.ViewModel;
using Movies.Enums;
using MovieHub.MediaPlayerElement.Interfaces;
using MovieHub.MediaPlayerElement.Service;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VideoPlayerControl.ViewModel;

namespace VideoPlayerControl
{
    public partial class MediaPlayerControlService : NotificationObject
    {
        private IMediaPlayerService MediaPlayerService;
        private MovieControl MediaControl;
        private bool hasSubscribed;
        private bool IsDragging;

        public string CurrentStreamFilePath { get; private set; }

        public MediaPlayerControlService(IMediaPlayerService mediaPlayerService)
        {
            this.MediaPlayerService = mediaPlayerService;
            this.MediaControl = new MovieControl();
            mediaMenuViewModel = new MediaMenuViewModel();

            MediaControl.Loaded += (s, e) => OnLoaded();
        }

        private void OnLoaded()
        {
            if(!hasSubscribed)
            {
                RegisterCommands();
                Subscribe_to_MediaPlayerServiceEvents();
                hasSubscribed = true;
            }
        }

        private void Subscribe_to_MediaPlayerServiceEvents()
        {
            MediaPlayerService.OnMediaOpened += MediaPlayerService_OnMediaOpened;
            MediaPlayerService.OnTimeChanged += MediaPlayerService_OnTimeChanged;
            MediaPlayerService.EncounteredError += MediaPlayerService_EncounteredError;
            MediaPlayerService.EndReached += MediaPlayerService_EndReached;
            MediaPlayerService.Buffering += MediaPlayerService_Buffering;
        }

        private void MediaPlayerService_Buffering(object sender, MovieHub.MediaPlayerElement.Models.MediaBufferingEventArgs e)
        {
           
        }

        private void MediaPlayerService_EndReached(object sender, EventArgs e)
        {

        }

        private void MediaPlayerService_EncounteredError(object sender, EventArgs e)
        {
            
        }

        private void MediaPlayerService_OnTimeChanged(object sender, EventArgs e)
        {
            TimeChangeAction();
        }

        private void MediaPlayerService_OnStateChanged(object sender, EventArgs e)
        {
            
        }

        private void MediaPlayerService_OnMediaStopped(object sender, EventArgs e)
        {

        }

        private void MediaPlayerService_OnMediaOpened(object sender, EventArgs e)
        {
            SetMediaControlDetails();
            MediaControl.MediaDuration = MediaPlayerService.Duration.TotalSeconds;

            if (mediaMenuViewModel != null)
                mediaMenuViewModel.Dispose();
        }

        private void SetMediaControlDetails()
        {
            MediaControl.MediaDuration = MediaPlayerService.Duration.TotalSeconds;
            SetMediaVolume(MediaControl.VolumeControl.VolumeLevel);
        }
        

        private void SetMediaVolume(double vol)
        {
            if (MediaPlayerService == null) return;
            MediaPlayerService.Volume = (int)vol;
        }

        private void TimeChangeAction()
        {
            if (!IsDragging)
            {
                MediaControl.CurrentMediaTime = MediaPlayerService.CurrentTimer.TotalSeconds;
            }
        }

        public MovieControl GetMediaControlView()
        {
            return this.MediaControl;
        }
    }
}
