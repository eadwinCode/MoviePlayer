using Common.Interfaces;
using Common.Util;
using MediaControl;
using System;
using System.Windows;
using VideoComponent.BaseClass;
using VideoPlayerControl;
using VideoPlayerControl.ViewModel;
using Microsoft.Practices.ServiceLocation;
using VirtualizingListView.Model;
using System.IO;
using Common.Model;
using Meta.Vlc;

namespace VideoPlayerView.FilePlayer
{
    public class PlayFile : IPlayFile
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

        public IVideoElement VideoElement { get { return _videoelement; } }
        private static IVideoElement _videoelement;
        private WindowsMediaPlayControl WindowsMediaPlayer;
        private bool HasScubscribed;
        private WindowState ShellState = WindowState.Normal;

        public void PlayFileInit(object obj)
        {
            InitPlayerView(obj);

            if (!HasScubscribed)
            {
                Subscribe();
            }
            (IShell as Window).WindowState = WindowState.Minimized;
        }

        private void Subscribe()
        {
            (IShell as Window).Closing += PlayFile_Closing1;
            HasScubscribed = true;
            ShellState = (IShell as Window).WindowState;
        }

        public void WMPPlayFileInit(object vfc)
        {
            InitWMPView(vfc);

            if (!HasScubscribed)
            {
                Subscribe();
            }

            (IShell as Window).WindowState = WindowState.Minimized;
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
                    MediaControllerVM.MediaControllerInstance.GetVideoItem((VideoFolderChild)obj);
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

        public void AddFiletoPlayList(object obj)
        {
            InitPlayerView();
            MediaControlExtension.SetFileexpVisiblity(VideoElement.PlayListView as UIElement,
                Visibility.Visible);

            VideoFolderChild vfc = (VideoFolderChild)obj;

            MediaControllerVM.MediaControllerInstance.Playlist.Add(vfc);
        }

        public void PlayFileFromPlayList(PlaylistModel plm)
        {
            InitPlayerView();
            MediaControlExtension.SetFileexpVisiblity(VideoElement.PlayListView as UIElement,
                Visibility.Visible);
            
            MediaControllerVM.MediaControllerInstance.Playlist.PlayFromAList(plm);
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
                MediaControllerVM.MediaControllerInstance.CloseMediaPlayer();
                _videoelement = null;
                (IShell as Window).WindowState = ShellState;
            }
            catch (Exception) { }
        }

        private static void GetVideoItem()
        {
            //
        }
        
        private IShell IShell
        {
            get { return ServiceLocator.Current.GetInstance<IShell>(); }
        }
    }
}
