using Meta.Vlc;
using Meta.Vlc.Interop.Core.Events;
using Meta.Vlc.Interop.Media;
using Meta.Vlc.Wpf;
using MovieHub.MediaPlayerElement.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieHub.MediaPlayerElement.Models
{
    public class MediaInformation : IMediaInfo
    {
        VlcPlayer vlcPlayer;
        internal MediaInformation(VlcPlayer vlcPlayer)
        {
            this.vlcPlayer = vlcPlayer;
            this.vlcPlayer.VlcMediaPlayer.Media.MetaChanged += Media_MetaChanged;
        }

        private void Media_MetaChanged(object sender, ObjectEventArgs<MediaMetaChangedArgs> e)
        {
            var enumItem = (e.Value).MetaType;
            this[enumItem.ToString()] = vlcPlayer.VlcMediaPlayer.Media.GetMeta(enumItem);

            NotifyChanges();
        }

        public event EventHandler MediaInfoChanged;

        public MediaTrackList MediaTrackList { get; private set; }
        public string Title { get; private set ; }
        public string Artist { get; private set ; }
        public string Genre { get ; private set; }
        public string Copyright { get; private set; }
        public string Album { get ; private set ; }
        public string TrackNumber { get; private set; }
        public string Description { get ; private set; }
        public string Rating { get ; private set; }
        public string Date { get ; private set; }
        public string Setting { get ; private set; }
        public string Url { get; private set; }
        public string Language { get ; private set; }
        public string NowPlaying { get ; private set; }
        public string Publisher { get; private set; }
        public string EncodedBy { get ; private set; }
        public string ArtworkUrl { get ; private set ; }
        public string TrackID { get ; private set; }
        public string TrackTotal { get; private set; }
        public string Director { get ; private set ; }
        public string Season { get; private set; }
        public string Episode { get; private set; }
        public string ShowName { get; private set; }
        public string Actors { get; private set; }
        public string AlbumArtist { get; private set; }
        public string DiscNumber { get; private set; }

        internal void LoadMediaInfo()
        {
            var Properties = this.GetType().GetProperties();
            var enums = Enum.GetValues(typeof(MetaDataType)).OfType<MetaDataType>().ToArray();
            foreach (MetaDataType enumItem in Enum.GetValues(typeof(MetaDataType)))
            {
                this[enumItem.ToString()] = vlcPlayer.VlcMediaPlayer.Media.GetMeta(enumItem);
            }

            NotifyChanges();
        }

        internal object this[string propertyName]
        {
            get
            {
                return this.GetType().GetProperty(propertyName).GetValue(this,null);
            }
            set
            {
                this.GetType().GetProperty(propertyName).SetValue(this, value, null);
            }
        }
        
        private void NotifyChanges()
        {
            MediaTrackList = vlcPlayer.VlcMediaPlayer.Media.GetTracks();

            if (MediaInfoChanged != null)
                MediaInfoChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
