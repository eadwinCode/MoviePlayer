﻿using MediaControl;
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
using System.Windows.Threading;

namespace VideoPlayerView.FilePlayer
{
    public partial class PlayFile : IPlayFile
    {
        private static IVideoElement _videoelement;
        private WindowsMediaPlayControl WindowsMediaPlayer;
        private bool HasScubscribed;
        private bool isplayingMedia = false;
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

        IRadioService RadioServicecs
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioService>();
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
                if (imediaservice == null) { 
                    imediaservice = new MediaPlayerService();
                }
                return imediaservice;
            }
        }

        public IVideoElement VideoElement { get { return _videoelement; } }
        
        private IShell IShell
        {
            get { return ServiceLocator.Current.GetInstance<IShell>(); }
        }

        public bool IsPlayingMedia { get { return isplayingMedia; } }
    }

    public partial class PlayFile
    {
        private void PlayFile_Closing1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ShutDownMediaPlayer();
        }

        public void ShutDownMediaPlayer()
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
                if (RadioServicecs.IsRadioOn)
                    RadioServicecs.ShutdownRadio();
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
                {
                    LoadFiletoPlayer(obj);
                }
                isplayingMedia = true;
            }
            catch (Exception)
            {
                Window_Closing();
                throw;
            }

        }

        private void LoadFiletoPlayer(object obj)
        {
            mediaControllerViewModel.GetVideoItem((VideoFolderChild)obj);
        }

        private void InitWMPView(object obj)
        {
            #region Creating VideoPlayer Object
            try
            {
                if (RadioServicecs.IsRadioOn)
                    RadioServicecs.ShutdownRadio();
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
                isplayingMedia = true;

            }
            catch (Exception) { throw; }
            #endregion
        }

        private void WindowsMediaPlayer_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            WindowsMediaPlayer = null;
            CloseLibraries();
            isplayingMedia = false;

            (IShell as Window).WindowState = ShellState;
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
                mediaControllerViewModel.CloseMediaPlayer(true);
                _videoelement = null;
                isplayingMedia = false;
                CloseLibraries();
                (IShell as Window).WindowState = ShellState;
            }
            catch (Exception) { }
        }

        private void CloseLibraries()
        {
            iplaylistmanagerviewmodel = null;
            mediaControllerViewModel = null;
            imediaservice = null;
        }

        public void PlayFileFromPlayList(PlaylistModel plm)
        {
            InitPlayerView();
            MediaControlExtension.SetFileexpVisiblity(VideoElement.PlayListView as UIElement,
                Visibility.Visible);

            PlaylistManagerViewModel.PlayFromAList(plm);
            (IShell as Window).WindowState = WindowState.Minimized;
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
                        LoaderCompletion.FinishCollectionLoadProcess(videoFolder.OtherFiles,null);
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
                    InitPlayerView();
                    MediaControlExtension.
                        SetFileexpVisiblity(VideoElement.PlayListView as UIElement,Visibility.Visible);
                    VideoFolder vf = (VideoFolder)obj;
                    PlaylistManagerViewModel.Add(vf.OtherFiles.Where(x => x is VideoFolderChild));
                }

            }
        }
        /// <summary>
        /// Maps the VideoElement to memory to reduce lag when needed
        /// </summary>
        public void PrepareVideoElement()
        {
            _videoelement = new VideoElement();
            (_videoelement as Window).Width = 20;
            (_videoelement as Window).Height = 20;
            (_videoelement as Window).WindowState = WindowState.Normal;
            (_videoelement as Window).Show();
            (_videoelement as Window).CommandBindings.Clear();
            (_videoelement as Window).Close();

            CloseLibraries();
            _videoelement = null;
        }

        public void WMPPlayFileInit(IFolder vfc)
        {

        }
    }
}
