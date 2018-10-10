using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using MovieHub.MediaPlayerElement.Interfaces;
using MovieHub.MediaPlayerElement.Models;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MovieHub.MediaPlayerElement.View;
using Microsoft.Practices.Prism.Commands;
using System.IO;

namespace MovieHub.MediaPlayerElement.ViewModel
{
    public class MediaMenu:NotificationObject
    {
        MediaMenuView MediaMenuView;
        MediaPlayerElement MediaPlayerElement;
        IMediaPlayerService MediaPlayerService;
        private DelegateCommand closeview;

        public DelegateCommand CloseView
        {
            get
            {
                if (closeview == null)
                    closeview = new DelegateCommand(CloseViewHandler);
                return closeview;
            }
        }

        private DelegateCommand addsubfilecommand;

        public DelegateCommand AddSubFileCommand
        {
            get {
                if(addsubfilecommand == null)
                {
                    addsubfilecommand = new DelegateCommand(AddSubFileAction);
                }
                return addsubfilecommand; }
        }

        // private List<Stream> audiotracks;
        public IEnumerable<MediaTrackDescription> AudioTracks
        {
            get
            {
                if (MediaPlayerService.AudioTracksManagement == null)
                    return null;
                return (MediaPlayerService.AudioTracksManagement.AudioTracks);
            }
        }

        //private List<Stream> videotracks;
        public IEnumerable<MediaTrackDescription> VideoTracks
        {
            get
            {
                if (MediaPlayerService.VideoTracksManagement == null)
                    return null;
                return (MediaPlayerService.VideoTracksManagement.VideoTracks);
            }
        }

        public IEnumerable<MediaTrackDescription> SubtitleTitleCollection
        {
            get
            {
                if (MediaPlayerService.SubtitleManagement == null)
                    return null;
                return (MediaPlayerService.SubtitleManagement.SubtitleList);
            }
        }
        
        internal MediaMenu(MediaPlayerElement mediaPlayerElement)
        {
            this.MediaPlayerElement = mediaPlayerElement;
            MediaPlayerService = mediaPlayerElement.MediaPlayerServices;
            MediaPlayerService.OnSubItemAdded += MediaPlayerService_SubItemChanged;
        }

        public void Dispose()
        {
            MediaMenuView = null;
        }

        private void MediaPlayerService_SubItemChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(() => this.SubtitleTitleCollection);
        }
        
        private void CloseViewHandler()
        {
            MediaPlayerElement._contentdockregion.Content = null;
            MediaPlayerElement.Focus();
        }

        public void ShowDialog()
        {
            if (MediaMenuView == null)
            {
                MediaMenuView = new View.MediaMenuView
                {
                    DataContext = this
                };
            }
            MediaPlayerElement._contentdockregion.Content = MediaMenuView;
        }

        private void AddSubFileAction()
        {
            var openfiles = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "All files(*.*)|*.srt;"
            };
            if (openfiles.ShowDialog() == true)
            {
                AddSubtitleFileAction(new string[] { openfiles.FileName });
            }
        }

        private void AddSubtitleFileAction(string[] subfiles)
        {
            for (int i = 0; i < subfiles.Length; i++)
            {
                FileInfo file = new FileInfo(subfiles[i]);
                if (file.Extension == ".srt")
                {
                    MediaPlayerElement.MediaPlayerServices.SubtitleManagement.SetSubtitle(file.FullName);
                }
            }
        }
    }
}
