using MediaControl;
using System;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Movies.MoviesInterfaces;
using Movies.Models.Model;
using Movies.Models.Interfaces;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Threading;
using MovieHub.MediaPlayerElement.Interfaces;
using System.Collections.ObjectModel;
using VideoPlayerView;

namespace VideoPlayerView.FilePlayer
{
    public partial class PlayFile : IPlayFile
    {
        private static MediaPlayerWindow _videoelement;
        private WindowsMediaPlayControl WindowsMediaPlayer;
        private bool HasScubscribed;
        private bool isplayingMedia = false;
        private object padlock = new object();
        
        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }

        IRadioService RadioServicecs
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioService>();
            }
        }
        
        public MediaPlayerWindow VideoElement { get { return _videoelement; } }
        
        private IShellWindow IShell
        {
            get { return ServiceLocator.Current.GetInstance<IShellWindow>(); }
        }

        public bool IsPlayingMedia { get { return isplayingMedia; } }

        public void ShutDown()
        {
            if (_videoelement != null)
            {
                (_videoelement).Close();
                // _videoelement = null;
            }

            if (WindowsMediaPlayer != null)
            {
                WindowsMediaPlayer.Close();
            }
        }
    }

    public partial class PlayFile
    {
        private void Subscribe()
        {
            HasScubscribed = true;
        }

        private void InitPlayerView()
        {
            try
            {
                if (RadioServicecs.IsRadioOn)
                    RadioServicecs.ShutDown();
                if (_videoelement == null)
                {
                    _videoelement = new MediaPlayerWindow();

                    VideoElement.Unloaded += (s, e) => {
                        if(_videoelement.CanUnload)
                           _videoelement = null;
                    };
                }

                if (WindowsMediaPlayer != null)
                {
                    WindowsMediaPlayer.Close();
                }
            (_videoelement).Show();
                isplayingMedia = true;
                if (!HasScubscribed)
                {
                    Subscribe();
                }
            }
            catch (Exception)
            {
                Window_Closing();
                throw;
            }

        }

        private void InitPlayerView(IPlayable item, IEnumerable<IPlayable> enumerable)
        {
            InitPlayerView();
            if (item != null && enumerable != null)
            {
                _videoelement.LoadMediaFile(item, enumerable);
                return;
            }
            if(item != null)
                _videoelement.LoadMediaFile(item);
        }

        private void InitPlayerView(PlaylistModel plm)
        {
            InitPlayerView();
            if (plm != null)
            {
                _videoelement.LoadMediaFile(plm);
            }
        }

        private void LoadFiletoPlayer(object obj)
        {
            VideoElement.LoadMediaFile((IPlayable)obj);
        }

        private void InitWMPView(object obj)
        {
            #region Creating VideoPlayer Object
            try
            {
                if (RadioServicecs.IsRadioOn)
                    RadioServicecs.ShutDown();

                if (WindowsMediaPlayer == null)
                {
                    WindowsMediaPlayer = new WindowsMediaPlayControl();
                    WindowsMediaPlayer.FormClosed += WindowsMediaPlayer_FormClosed;
                }

                if (_videoelement != null)
                {
                    (_videoelement).Close();
                }
                WindowsMediaPlayer.OpenFile((VideoFolderChild)obj);
                WindowsMediaPlayer.Show();
                isplayingMedia = true;
                if (!HasScubscribed)
                {
                    Subscribe();
                }
            }
            catch (Exception) { throw; }
            #endregion
        }

        private void WindowsMediaPlayer_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            WindowsMediaPlayer = null;
            CloseLibraries();
            isplayingMedia = false;
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
            isplayingMedia = false;

        }

        private void Window_Closing()
        {
            try
            {
               // mediaControllerViewModel.CloseMediaPlayer(true);
                _videoelement = null;
                isplayingMedia = false;
                CloseLibraries();
               // (IShell as Window).WindowState = ShellState;
            }
            catch (Exception) { }
        }

        private void CloseLibraries()
        {

        }

        public void PlayFileFromPlayList(PlaylistModel plm)
        {
            InitPlayerView(plm);
        }

        public void PlayFileInit(IVideoData obj)
        {
            InitPlayerView();
            LoadFiletoPlayer(obj);
            Subscribe();
        }

        public void PlayFileInit(IPlayable obj,IEnumerable<IPlayable> enumerable)
        {
            InitPlayerView(obj,enumerable);
        }

        public void AddFiletoPlayList(IVideoData obj)
        {
            InitPlayerView();
            VideoFolderChild vfc = (VideoFolderChild)obj;
            _videoelement.AddToPlaylist(vfc);
        }

        public void WMPPlayFileInit(IVideoData vfc)
        {
            InitWMPView(vfc);
            
            (IShell as Window).WindowState = WindowState.Minimized;
        }

        public void PlayFileInit(IFolder obj)
        {
            lock (padlock)
            {
                VideoFolder videoFolder = obj as VideoFolder;
                VideoFolderChild item = (VideoFolderChild)videoFolder.OtherFiles.FirstOrDefault(x => x is VideoFolderChild);
                if (item != null)
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        LoaderCompletion.FinishCollectionLoadProcess(videoFolder.OtherFiles,null);
                    }).ContinueWith(t => { }, TaskScheduler.FromCurrentSynchronizationContext());

                    InitPlayerView(item, videoFolder.OtherFiles.OfType<IPlayable>());
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
                    InitPlayerView();
                    VideoFolder vf = (VideoFolder)obj;
                    IEnumerable<IPlayable> playables = vf.OtherFiles.OfType<IPlayable>();
                    _videoelement.AddRangeToPlaylist(playables);
                }

            }
        }
        /// <summary>
        /// Maps the VideoElement to memory to reduce lag when needed
        /// </summary>
        public void PrepareVideoElement()
        {
            var Ipagenavigator = ServiceLocator.Current.GetInstance<IPageNavigatorHost>();
            MediaPlayerWindow mediaPlayerWindow = new MediaPlayerWindow();
            Ipagenavigator.AddView(mediaPlayerWindow, typeof(MediaPlayerWindow).Name);
            mediaPlayerWindow.Visibility = Visibility.Hidden;
            mediaPlayerWindow.Loaded += (s, e) => {
                Ipagenavigator.RemoveView(typeof(MediaPlayerWindow).Name);
                mediaPlayerWindow.MediaPlayerElement.Dispose();
                mediaPlayerWindow = null;
            };
           
        }

        public void WMPPlayFileInit(IFolder vfc)
        {

        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     