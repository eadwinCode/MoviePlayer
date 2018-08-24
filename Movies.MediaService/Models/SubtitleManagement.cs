using Meta.Vlc.Wpf;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.MediaService.Models
{
    public class SubtitleManagement : NotificationObject
    {
        VlcPlayer mediaplayer;

        private MediaTrackDescriptionList subtitlelist;
        private MediaTrackDescription subtitle;
        private bool IsUpdating;

        private int Currentsubtitle
        {
            get { return mediaplayer.VlcMediaPlayer.Subtitle; }
        }
        public MediaTrackDescriptionList SubtitleList
        {
            get
            {
                return subtitlelist; 
            }
        }
        public MediaTrackDescription Subtitle { get { return subtitle; } }

        public int SubstituteCount { get { return SubtitleList.Count; } }// _vlcPlayer.VlcMediaPlayer.SubtitleCount; } }

        public long SubtitleDelay
        {
            get { return mediaplayer.VlcMediaPlayer.SubtitleDelay; }
            set { mediaplayer.VlcMediaPlayer.SubtitleDelay = value; }
        }

        public SubtitleManagement(VlcPlayer mediaplayer)
        {
            this.mediaplayer = mediaplayer;
        }

        public void ReloadData()
        {
            subtitlelist = new MediaTrackDescriptionList(mediaplayer.VlcMediaPlayer.SubtitleDescription.Pointer);
            if(Currentsubtitle > 0 && Currentsubtitle < subtitlelist.Count)
                SetSubtitle(subtitlelist[0]);
            subtitlelist.OnPropertyChanged += Subtitlelist_OnPropertyChanged;
        }

        private void Subtitlelist_OnPropertyChanged(object sender, EventArgs e)
        {
            if (!IsUpdating)
            {
                MediaTrackDescription mediaTrackDescription = sender as MediaTrackDescription;
                SetSubtitle(mediaTrackDescription);
            }
        }

        public void SetSubtitle(string filepath)
        {
            mediaplayer.VlcMediaPlayer.SetSubtitleFile(filepath);
            ReloadData();
        }

        private void SetSubtitle(MediaTrackDescription mediaTrackDescription)
        {
            this.IsUpdating = true;

            foreach (var item in subtitlelist)
            {
                if (item != mediaTrackDescription)
                    item.IsSelected = false;
                else
                    item.IsSelected = true;
            }

            subtitle = mediaTrackDescription;
            mediaplayer.VlcMediaPlayer.Subtitle = mediaTrackDescription.Id;
            this.IsUpdating = false;
        }
    }
}
