using Common.Util;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using Movies.MediaService.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace VideoPlayerControl.ViewModel
{
    public partial class VideoPlayerVM : NotificationObject
    {
        private DispatcherTimer MousemoveTimer;
        private SubtitleMediaController VideoPlayerView;
        private IMediaController IMediaController;
        private string minimizemediactrltext;
        private bool allowautoresize = true;
        private SCREENSETTINGS screensetting;
        internal bool Isloaded;
        private bool isfullscreenmode;
        private IVideoElement icommandbindings;
        MediaMenuViewModel mediaMenuViewModel;

        IMediaControllerViewModel MediaControllerViewModel
        {
            get { return FilePlayerManager.MediaControllerViewModel; }
        }

        IDispatcherService IDispatcherService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDispatcherService>();
            }
        }

        private IVideoElement IVideoElement
        {
            get
            {
                if (icommandbindings == null)
                {
                    icommandbindings = FilePlayerManager.VideoElement;
                }
                return icommandbindings;
            }
        }

        IMediaPlayerService MediaPlayerService
        {
            get
            {
                return FilePlayerManager.MediaPlayerService;
            }
        }

        IPlayFile FilePlayerManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }

        public bool AllowAutoResize
        {
            get { return allowautoresize; }
            set { allowautoresize = value; }
        }

        public string MinimizeMediaCtrlText
        {
            get { return minimizemediactrltext; }
            set { minimizemediactrltext = value;
                RaisePropertyChanged(() => this.MinimizeMediaCtrlText); }
        }

        public SCREENSETTINGS ScreenSetting
        {
            get
            {
                return screensetting;
            }
            set
            {

                if (IsFullScreenMode)
                {
                    VideoPlayerView.OnScreenSettingsChanged(new object[] { value, SCREENSETTINGS.Fullscreen });
                    IsFullScreenMode = false;
                }
                else
                {
                    VideoPlayerView.OnScreenSettingsChanged(new object[] { value, SCREENSETTINGS.Normal });
                }
                if (value == SCREENSETTINGS.Normal)
                {
                    //Grid gd = VideoPlayerView.VideoContent as Grid;
                    //Grid.SetRowSpan(gd, 1);
                    //VideoPlayerView.ControlHolder.Visibility = Visibility.Visible; 
                    NormalScreenSettings();
                }
                else
                {
                    //Grid gd = VideoPlayerView.VideoContent as Grid;
                    //Grid.SetRowSpan(gd, 2);
                    //VideoPlayerView.ControlHolder.Visibility = Visibility.Collapsed;
                    FullScreenSettings();
                }
                screensetting = value;
                this.RaisePropertyChanged(() => this.ScreenSetting);
            }
        }
        
        public bool IsFullScreenMode
        {
            get { return isfullscreenmode; }
            set { isfullscreenmode = value; RaisePropertyChanged(() => this.IsFullScreenMode); }
        }
        
        public VideoPlayerVM(IMediaController ivideoplayer)
        {
            this.IMediaController = ivideoplayer;
            VideoPlayerView = (SubtitleMediaController)ivideoplayer;
            MousemoveTimer = new DispatcherTimer(DispatcherPriority.Background);
            ScreenSetting = SCREENSETTINGS.Normal;
            VideoPlayerView.Loaded += VideoPlayerView_Loaded;
            mediaMenuViewModel = new MediaMenuViewModel();
        }
        
        private void Init()
        {
            (IMediaController.MediaController as UserControl).MouseLeave += Mediacontrol_MouseLeave;
            (IMediaController.MediaController as UserControl).MouseEnter += Mediacontrol_MouseEnter;
            (IVideoElement as Window).MouseMove += ParentGrid_MouseMove;
            MediaPlayerService.EndReached += VlcMediaPlayer_EndReached;
            MediaPlayerService.EncounteredError += VlcMediaPlayer_EncounteredError;
            MediaPlayerService.MouseMove += ParentGrid_MouseMove;
            MediaPlayerService.OnMediaOpened += MediaPlayerService_OnMediaOpened;
        }

        private void MediaPlayerService_OnMediaOpened(object sender, EventArgs e)
        {
            if (mediaMenuViewModel != null)
                mediaMenuViewModel.Dispose();
        }

        private void VlcMediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            ResetVisibilityAnimationAsyn();
        }

        private void VlcMediaPlayer_EndReached(object sender, EventArgs e)
        {
            ResetVisibilityAnimationAsyn();
        }

        private void WindowsTab_MouseLeave(object sender, MouseEventArgs e)
        {
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaController.MediaController as UIElement, true);
            if (!IsFullScreenMode)
            {
               // MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
            }
            else
            {
                //MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false);
            }
            this.MousemoveTimer.Start();
        }

        private void WindowsTab_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MousemoveTimer.Stop();
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaController.MediaController as UIElement, null);
            //MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
        }

        private void VideoPlayerView_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            MousemoveTimer.Interval = TimeSpan.FromSeconds(5);
            MousemoveTimer.Tick += MousemoveTimer_Tick;
            this.MousemoveTimer.Stop();

            RegisterCommands();
        }

        void MousemoveTimer_Tick(object sender, EventArgs e)
        {
            if (MediaControllerViewModel == null) { MousemoveTimer.Stop(); return; }
            if (!MediaControllerViewModel.IsPlaying)
            {
                this.MousemoveTimer.Stop();
                return;
            }
            if (!MediaControllerViewModel.IsMouseControlOver)
            {
                (IVideoElement as Window).Cursor = Cursors.None;
                // MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false);
                MediaControlExtension.SetIsMouseOverMediaElement(IMediaController.MediaController as UIElement, false);
                this.MousemoveTimer.Stop();
            }
            else if (MediaControllerViewModel.IsMouseControlOver)
            {
                this.MousemoveTimer.Stop();
            }
            else
            {
                (IVideoElement as Window).Cursor = Cursors.None;

                if (!IsFullScreenMode)
                {
                    // MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false);
                }
                else
                {
                    //MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
                }
                this.MousemoveTimer.Stop();
            }
        }

        private void Mediacontrol_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MousemoveTimer.Stop();
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaController.MediaController as UIElement, null);
        }
        
        private void RestoreScreen()
        {
            switch (MediaControllerViewModel.MediaState)
            {
                case MovieMediaState.Playing:
                case MovieMediaState.Paused:
                case MovieMediaState.Stopped:

                    if (ScreenSetting != SCREENSETTINGS.Fullscreen)
                    {
                        ScreenSetting = SCREENSETTINGS.Fullscreen;
                    }
                    else
                    {
                        ScreenSetting = SCREENSETTINGS.Normal;
                    }
                    break;
                case MovieMediaState.Ended:
                    if (ScreenSetting != SCREENSETTINGS.Normal)
                    {
                        ScreenSetting = SCREENSETTINGS.Normal;
                    }
                    break;
                default:
                    break;
            }
        }

        private void AddSubtitleFileAction(string[] filePathInfo)
        {
            bool issubfile = false;
            for (int i = 0; i < filePathInfo.Length; i++)
            {
                FileInfo file = new FileInfo(filePathInfo[i]);
                if (file.Extension == ".srt")
                {
                    issubfile = true;
                    MediaControllerViewModel.SetSubtitle(file.FullName);
                }
            }
            //    if (issubfile)
            //    {
            //        MediaControllerVM.UpdateHardCodedSubs();
            //    }
        }

        private void ParentGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MediaControllerViewModel.IsMouseControlOver && MediaControllerViewModel.IsPlaying)
            {
                ResetVisibilityAnimation();
                this.MousemoveTimer.Start();
            }
        }

        private void Mediacontrol_MouseLeave(object sender, MouseEventArgs e)
        {
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaController.MediaController as UIElement, true);
            if (!IsFullScreenMode)
            {
                //MediaControlExtension.SetAnimateWindowsTab( IVideoElement.WindowsTab as UIElement,true);
            }
            else
            {
                //MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false);
            }
            this.MousemoveTimer.Start();
        }

        private void ResetVisibilityAnimation()
        {
            //if((IVideoElement as Window).Dispatcher.)//if () return;
            this.MousemoveTimer.Stop();
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaController.MediaController as UIElement, null);
            (IVideoElement as Window).Cursor = Cursors.Arrow;
            if (Isloaded && ScreenSetting == SCREENSETTINGS.Normal && !IsFullScreenMode)
            {
                // IVideoElement.WindowsTab.Visibility = Visibility.Visible;
                //MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
            }
            //else { MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false); }

        }

        private void ResetVisibilityAnimationAsyn()
        {
            //if((IVideoElement as Window).Dispatcher.)
            (IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            {
                //if () return;
                this.MousemoveTimer.Stop();
                MediaControlExtension.SetIsMouseOverMediaElement(IMediaController.MediaController as UIElement, null);
                (IVideoElement as Window).Cursor = Cursors.Arrow;
                if (Isloaded && ScreenSetting == SCREENSETTINGS.Normal && !IsFullScreenMode)
                {
                    // IVideoElement.WindowsTab.Visibility = Visibility.Visible;
                    //MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
                }
                //else { MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false); }
            }));
        }

        public void FullScreenSettings()
        {
            if (screensetting == SCREENSETTINGS.Fullscreen) return;
            MediaControllerViewModel.CanAnimate = true;
            MediaControlExtension.SetCanAnimateControl((IMediaController.MediaController as UserControl), true);
            screensetting = SCREENSETTINGS.Fullscreen;
            MinimizeMediaCtrlText = "Restore MediaControl";
            if (IVideoElement != null)
            {
                (IVideoElement as MetroWindow).UseNoneWindowStyle = true;
                (IVideoElement as MetroWindow).IgnoreTaskbarOnMaximize = true;
            }
        }

        public void NormalScreenSettings()
        {
            MediaControlExtension.SetCanAnimateControl((IMediaController.MediaController as UserControl), false);
            MediaControllerViewModel.CanAnimate = false;
            screensetting = SCREENSETTINGS.Normal;
            MinimizeMediaCtrlText = "Minimize MediaControl";
            if (IVideoElement != null)
            {
                (IVideoElement as MetroWindow).UseNoneWindowStyle = false;
                (IVideoElement as MetroWindow).ShowTitleBar = true;
                (IVideoElement as MetroWindow).IgnoreTaskbarOnMaximize = false;
            }
        }
        
        public void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            RestoreScreen();
        }
                
        public void OnDrop(DragEventArgs e)
        {
            VideoFolder vf = (VideoFolder)e.Data.GetData(typeof(VideoFolder));
            if (vf == null)
            {
                vf = (VideoFolder)e.Data.GetData(typeof(VideoFolderChild));
            }
            if (vf == null)
            {
               
                if (MediaControllerViewModel.CurrentVideoItem != null)
                {
                    String[] filePathInfo = (String[])e.Data.GetData("FileName", false);
                    AddSubtitleFileAction(filePathInfo);
                    return;
                }
            }
            if (vf.FileType == FileType.Folder)
            {
                VideoFolder vfc = null;
                foreach (VideoFolder item in vf.OtherFiles)
                {
                    if (item.FileType == FileType.File)
                    {
                        vfc = item;
                        break;
                    }
                }
                if (vfc == null)
                {
                    return;
                }
                MediaControllerViewModel.GetVideoItem(vfc as VideoFolderChild);
                return;
            }
            MediaControllerViewModel.GetVideoItem(vf as VideoFolderChild);
            CommandManager.InvalidateRequerySuggested();
        }

        public void VisibilityAnimation()
        {
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaController.MediaController as UIElement, null);
            (IVideoElement as Window).Cursor = Cursors.Arrow;
            if (Isloaded && ScreenSetting == SCREENSETTINGS.Normal && !IsFullScreenMode)
            {
                //MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
            }

            this.MousemoveTimer.Start();
        }
        
    }
}
