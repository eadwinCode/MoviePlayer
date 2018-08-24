using Meta.Vlc;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoPlayerControl.ViewModel;

namespace VideoPlayerView.Model
{
    public class Stream : NotificationObject
    {
        private bool isactive;
        private TrackType TrackType;
        public bool IsActive
        {
            get {
               
                return isactive;
            }
            set { isactive = value; RaisePropertyChanged(() => this.IsActive); }
        }
        private int CurrentTrack
        {
            get
            {
                //switch (TrackType)
                //{
                //    case TrackType.Audio:
                //        returnFilePlayerManager.MediaControllerViewModel.IVideoElement.MediaPlayer.AudioTrackDescription
                //    [FilePlayerManager.MediaControllerViewModel.IVideoElement.MediaPlayer.AudioTrack].Id;
                //    case TrackType.Video:
                //        returnFilePlayerManager.MediaControllerViewModel.IVideoElement.MediaPlayer.VlcMediaPlayer.VideoTrackDescription
                //    [FilePlayerManager.MediaControllerViewModel.IVideoElement.MediaPlayer.VlcMediaPlayer.VideoTrack].Id;
                //    default:
                //        break;
                //}
                return 0;
            }
        }
        public TrackDescription Track { get; set; }
        public Stream(TrackDescription AudioTrack,TrackType trackType)
        {
            this.Track = AudioTrack;
            this.TrackType = trackType;
            if (CurrentTrack == AudioTrack.Id)
                IsActive = true;
        }

        public override string ToString()
        {
            return Track.ToString();
        }
    }

    public enum TrackType
    {
        Audio,
        Video
    }
}
