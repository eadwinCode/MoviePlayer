using MediaControl;
using System;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Movies.MoviesInterfaces;
using Meta.Vlc;
using Movies.Models.Model;
using Movies.Models.Interfaces;
using Common.Util;
using Movies.MoviePlaylistManager.ViewModel;
using VideoPlayerControl.ViewModel;
using System.Windows.Controls;
using Movies.MediaService.Interfaces;
using Movies.MediaService.Service;
using System.Collections.Generic;

namespace VideoPlayerView.FilePlayer
{
    public partial class PlayFile : IPlayFile
    {
        private static IVideoElement _videoelement;
        private WindowsMediaPlayControl WindowsMediaPlayer;
        private bool HasScubscribed;
        private WindowState ShellState = WindowState.Normal;
        private object padlock = new object();

        IMediaControllerViewModel mediaControllerViewModel;
        public IMediaControllerViewModel MediaControllerViewModel
        {
            get
            {
                if (mediaControllerViewModel == null)
                    mediaControllerViewModel = new MediaControllerViewModel();
                return mediaControllerViewModel;
            }
        }
        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }

        IPlaylistManagerViewModel iplaylistmanagerviewmodel;
        public IPlaylistManagerViewModel PlaylistManagerViewModel
        {
            get
            {
                if (iplaylistmanagerviewmodel == null)
                    iplaylistmanagerviewmodel = new PlaylistManagerViewModel();
                return iplaylistmanagerviewmodel;
            }
        }

        IMediaPlayerService imediaservice;
        public IMediaPlayerService MediaPlayerService
        {
            get
            {
                if (imediaservice == null)
                    imediaservice = new MediaPlayerService();
                return imediaservice;
            }
        }

        public IVideoElement VideoElement { get { return _videoelement; } }
        
        private IShell IShell
        {
            get { return ServiceLocator.Current.GetInstance<IShell>(); }
        }
    }

    public partial class PlayFile
    {
        private void PlayFile_Closing1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_videoelement != null)
            {
                (_videoelement as Window).Close();
            }

            if (WindowsMediaPlayer != null)
            {
                WindowsMediaPlayer.Close();

            }
            if (Win32Api.SetThreadExecutionState(Win32Api.ES_CONTINUOUS) == null)
            {
                // try XP variant as well just to make sure 
                Win32Api.SetThreadExecutionState(Win32Api.ES_CONTINUOUS);
            }
        }

        private void Subscribe()
        {
            (IShell as Window).Closing += PlayFile_Closing1;
            HasScubscribed = true;
            ShellState = (IShell as Window).WindowState;
        }

        private void InitPlayerView(object obj = null)
        {
            try
            {
                if (_videoelement == null)
                {
                    _videoelement = new VideoElement();
                    (_videoelement as Window).Closing += PlayFile_Closing;
                    (_videoelement as Window).Closed += PlayFile_Closed;
                }

                if (WindowsMediaPlayer != null)
                {
                    WindowsMediaPlayer.Close();
                }
            (_videoelement as Window).Show();
                if (obj != null)
                    mediaControllerViewModel.GetVideoItem((VideoFolderChild)obj);
            }
            catch (Exception)
            {
                Window_Closing();
                throw;
            }

        }

        private void InitWMPView(object obj)
        {
            #region Creating VideoPlayer Object
            try
            {
                if (WindowsMediaPlayer == null)
                {
                    WindowsMediaPlayer = new WindowsMediaPlayControl();
                    WindowsMediaPlayer.FormClosed += WindowsMediaPlayer_FormClosed;
                }

                if (_videoelement != null)
                {
                    (_videoelement as Window).Close();
                }
                WindowsMediaPlayer.OpenFile((VideoFolderChild)obj);
                WindowsMediaPlayer.Show();
            }
            catch (Exception) { throw; }
            #endregion
        }

        private void WindowsMediaPlayer_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            (IShell as Window).WindowState = ShellState;
        }

        public void PlayFileFromPlayList(PlaylistModel plm)
        {
            InitPlayerView();
            MediaControlExtension.SetFileexpVisiblity(VideoElement.PlayListView as UIElement,
                Visibility.Visible);

            PlaylistManagerViewModel.PlayFromAList(plm);
            (IShell as Window).WindowState = WindowState.Minimized;
        }

        private void PlayFile_Closed(object sender, EventArgs e)
        {
            if (_videoelement != null)
            {
                Window_Closing();
            }
        }

        private void PlayFile_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Window_Closing();
        }

        private void Window_Closing()
        {
            try
            {
                mediaControllerViewModel.CloseMediaPlayer(true);
                _videoelement = null;
                iplaylistmanagerviewmodel = null;
                mediaControllerViewModel = null;
                imediaservice = null;
                (IShell as Window).WindowState = ShellState;
            }
            catch (Exception) { }
        }

        public void PlayFileInit(IVideoData obj)
        {
            InitPlayerView(obj);

            if (!HasScubscribed)
            {
                Subscribe();
            }
            (IShell as Window).WindowState = WindowState.Minimized;
        }

        public void PlayFileInit(IVideoData obj,IEnumerable<VideoFolderChild> enumerable)
        {
            InitPlayerView();
            MediaControlExtension.SetFileexpVisiblity(VideoElement.PlayListView as UIElement,
                Visibility.Visible);

            PlaylistManagerViewModel.PlayFromTemperalList(obj,enumerable);
            (IShell as Window).WindowState = WindowState.Minimized;
        }

        public void AddFiletoPlayList(IVideoData obj)
        {
            InitPlayerView();
            MediaControlExtension.SetFileexpVisiblity(VideoElement.PlayListView as UIElement,
                Visibility.Visible);

            VideoFolderChild vfc = (VideoFolderChild)obj;

            PlaylistManagerViewModel.Add(vfc);
        }

        public void WMPPlayFileInit(IVideoData vfc)
        {
            InitWMPView(vfc);

            if (!HasScubscribed)
            {
                Subscribe();
            }

            (IShell as Window).WindowState = WindowState.Minimized;
        }

        public void PlayFileInit(IFolder obj)
        {
            lock (padlock)
            {

                VideoFolder videoFolder = obj as VideoFolder;
                var item = videoFolder.OtherFiles.FirstOrDefault(x => x is VideoFolderChild);
                if (item != null)
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        LoaderCompletion.FinishCollectionLoadProcess(videoFolder.OtherFiles);
                    }).ContinueWith(t => { }, TaskScheduler.FromCurrentSynchronizationContext());

                    InitPlayerView(item);
                    PlaylistManagerViewModel.PlayListCollection = new System.Collections.ObjectModel.ObservableCollection<VideoFolder>(videoFolder.OtherFiles.Where(x => x is VideoFolderChild));
                    if (!HasScubscribed)
                    {
                        Subscribe();
                    }
                (IShell as Window).WindowState = WindowState.Minimized;
                }
            }

        }

        public void AddFiletoPlayList(IFolder obj)
        {
            lock (padlock)
            {
                VideoFolder videoFolder = obj as VideoFolder;
                var item = videoFolder.OtherFiles.FirstOrDefault(x => x is VideoFolderChild);
                if (item != null)
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        LoaderCompletion.FinishCollectionLoadProcess(videoFolder.OtherFiles);
                    }).ContinueWith(t => { }, TaskScheduler.FromCurrentSynchronizationContext());

                    InitPlayerView();
                    MediaControlExtension.SetFileexpVisiblity(VideoElement.PlayListView as UIElement,
                        Visibility.Visible);

                    VideoFolder vf = (VideoFolder)obj;
                    var vfc = vf.OtherFiles.Where(x => x is VideoFolderChild);
                    foreach (var folderchild in vfc)
                    {
                        PlaylistManagerViewModel.Add(folderchild);
                    }
                }

            }
        }

        public void WMPPlayFileInit(IFolder vfc)
        {

        }
    }
}
