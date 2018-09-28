using Meta.Vlc.Wpf;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieHub.MediaPlayerElement.Models
{
    public class AudioTracksManagement : NotificationObject
    {
        VlcPlayer mediaplayer;
        MediaTrackDescription audiotrack;
        MediaTrackDescriptionList audiotracks;
        private bool IsUpdating;

        public MediaTrackDescriptionList AudioTracks { get { return audiotracks; } }
        public MediaTrackDescription AudioTrack
        {
            get { return audiotrack; }
            set { audiotrack = value; }
        }

        public int AudioTrackCount { get { return mediaplayer.VlcMediaPlayer.AudioTrackCount; } }

        public AudioTracksManagement(VlcPlayer mediaplayer)
        {
            this.mediaplayer = mediaplayer;
        }

        public void LoadData()
        {
            audiotracks = new MediaTrackDescriptionList(mediaplayer.AudioTrackDescription.Pointer);
            audiotracks.OnPropertyChanged += Audiotracks_OnPropertyChanged;
            if(mediaplayer.AudioTrack > 0 && mediaplayer.AudioTrack < audiotracks.Count)
                SetAudioTrack(audiotracks[mediaplayer.AudioTrack]);
        }

        private void Audiotracks_OnPropertyChanged(object sender, EventArgs e)
        {
            if (!IsUpdating)
            {
                MediaTrackDescription mediaTrackDescription = sender as MediaTrackDescription;
                SetAudioTrack(mediaTrackDescription);
            }
        }

        private void SetAudioTrack(MediaTrackDescription mediaTrackDescription)
        {
            this.IsUpdating = true;
            foreach (var item in audiotracks)
            {
                if (item != mediaTrackDescription)
                    item.IsSelected = false;
                else
                {
                    item.IsSelected = true;
                }
            }

            audiotrack = mediaTrackDescription;
            mediaplayer.AudioTrack = mediaTrackDescription.Id;
            this.IsUpdating = false;
        }
    }
}
