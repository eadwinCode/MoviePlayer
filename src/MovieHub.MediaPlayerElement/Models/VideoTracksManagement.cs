using Meta.Vlc.Wpf;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieHub.MediaPlayerElement.Models
{
    public class VideoTracksManagement : NotificationObject
    {
        public MediaTrackDescriptionList VideoTracks { get { return videotracks; } }
        public MediaTrackDescription VideoTrack
        {
            get { return videotrack; }
            set { videotrack = value; }
        }

        VlcPlayer mediaplayer;
        MediaTrackDescription videotrack;
        MediaTrackDescriptionList videotracks;
        private bool IsUpdating;

        public int VideoTracksCount { get { return mediaplayer.VlcMediaPlayer.VideoTrackCount; } }

        public VideoTracksManagement(VlcPlayer mediaplayer)
        {
            this.mediaplayer = mediaplayer;
        }

        public void LoadData()
        {
            videotracks = new MediaTrackDescriptionList(mediaplayer.AudioTrackDescription.Pointer);
            videotracks.OnPropertyChanged += Videotracks_OnPropertyChanged;
            if (mediaplayer.VlcMediaPlayer.VideoTrack > 0 &&
                mediaplayer.VlcMediaPlayer.VideoTrack < videotracks.Count)
            {
                SetVideoTrack(videotracks[mediaplayer.VlcMediaPlayer.VideoTrack]);
            }
        }

        private void Videotracks_OnPropertyChanged(object sender, EventArgs e)
        {
            if (!IsUpdating)
            {
                MediaTrackDescription mediaTrackDescription = sender as MediaTrackDescription;
                SetVideoTrack(mediaTrackDescription);
            }
        }

        private void SetVideoTrack(MediaTrackDescription mediaTrackDescription)
        {
            this.IsUpdating = true;

            foreach (var item in videotracks)
            {
                if (item != mediaTrackDescription)
                    item.IsSelected = false;
                else
                    item.IsSelected = true;
            }

            videotrack = mediaTrackDescription;
            mediaplayer.VlcMediaPlayer.VideoTrack = mediaTrackDescription.Id;
            this.IsUpdating = false;
        }
    }
}
