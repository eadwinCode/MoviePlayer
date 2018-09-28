using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using MovieHub.MediaPlayerElement.Interfaces;
using MovieHub.MediaPlayerElement.Models;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoPlayerControl.View;

namespace VideoPlayerControl.ViewModel
{
    public class MediaMenuViewModel:NotificationObject
    {
        MediaMenu MediaMenu;
        IMediaPlayerService MediaPlayerService;

        public MediaMenuViewModel(IMediaPlayerService mediaplayerservice)
        {
            this.MediaPlayerService = mediaplayerservice;
            MediaPlayerService.OnSubItemAdded += MediaPlayerService_SubItemChanged;
        }

        public void Dispose()
        {
            MediaMenu = null;
        }

        private void MediaPlayerService_SubItemChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(() => this.SubtitleTitleCollection);
        }

        public MediaMenu GetMediaMenuView()
        {
            if (MediaMenu == null)
            {
                MediaMenu = new MediaMenu
                {
                    DataContext = this
                };
            }
            return MediaMenu;
        }

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

        IPlayFile FilePlayerManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }
    }
}
