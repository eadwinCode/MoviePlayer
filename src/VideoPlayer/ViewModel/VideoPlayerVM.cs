using Common.Util;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using MovieHub.MediaPlayerElement.Interfaces;
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
    public partial class MediaControllerViewModel 
    {
        private DispatcherTimer MousemoveTimer;
        private IControllerView IMediaControllerView;
        private string minimizemediactrltext;
        private bool allowautoresize = true;
        private SCREENSETTINGS screensetting;
        internal bool Isloaded;
        private bool isfullscreenmode;
        private IVideoElement icommandbindings;
        MediaMenuViewModel mediaMenuViewModel;

        IDispatcherService IDispatcherService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDispatcherService>();
            }
        }

        public bool AllowAutoResize
        {
            get { return allowautoresize; }
            set { allowautoresize = value; }
        }

        public SCREENSETTINGS ScreenSetting
        {
            get
            {
                return screensetting;
            }
            set
            {
                if (value == SCREENSETTINGS.Normal)
                {
                    NormalScreenSettings();
                    IsFullScreenMode = false;

                }
                else
                {
                    IsFullScreenMode = true;
                    FullScreenSettings();
                }
                screensetting = value;
                this.RaisePropertyChanged(() => this.ScreenSetting);
            }
        }

        MediaPlayerControlService movieController;

        public bool IsFullScreenMode
        {
            get { return isfullscreenmode; }
            set { isfullscreenmode = value; RaisePropertyChanged(() => this.IsFullScreenMode); }
        }

        public MediaControllerViewModel()
        {
            this.IMediaControllerView = new MediaController(this);
            movieController = new MediaPlayerControlService((IMediaControllerView as UserControl).CommandBindings,
                FilePlayerManager.MediaPlayerService);
            (this.IMediaControllerView as UserControl).Loaded += this.MediaController_Loaded;
            
            MousemoveTimer = new DispatcherTimer(DispatcherPriority.Background);
            ScreenSetting = SCREENSETTINGS.Normal;
            mediaMenuViewModel = new MediaMenuViewModel();

        }
        
        public IControllerView GetControllerView()
        {
            return IMediaControllerView;
        }

        public object GetControllerNewView()
        {
            //return IMediaControllerView;
            return movieController.GetMediaControlView();
        }

        private void MediaPlayerService_OnMediaOpened(object sender, EventArgs e)
        {
            if (mediaMenuViewModel != null)
                mediaMenuViewModel.Dispose();
        }
        

        private void WindowsTab_MouseLeave(object sender, MouseEventArgs e)
        {
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaControllerView as UIElement, true);
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
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaControllerView as UIElement, null);
            //MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
        }
        
        void MousemoveTimer_Tick(object sender, EventArgs e)
        {
            if (!IsPlaying)
            {
                this.MousemoveTimer.Stop();
                return;
            }
            if (!IsMouseControlOver)
            {
                (IVideoElement as Window).Cursor = Cursors.None;
                // MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false);
                MediaControlExtension.SetIsMouseOverMediaElement(IMediaControllerView as UIElement, false);
                this.MousemoveTimer.Stop();
            }
            else if (IsMouseControlOver)
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
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaControllerView as UIElement, null);
        }
        
        private void RestoreScreen()
        {
            switch (MediaState)
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
            for (int i = 0; i < filePathInfo.Length; i++)
            {
                FileInfo file = new FileInfo(filePathInfo[i]);
                if (file.Extension == ".srt")
                {
                    SetSubtitle(file.FullName);
                }
            }
        }

        private void ParentGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsMouseControlOver && IsPlaying)
            {
                ResetVisibilityAnimation();
                this.MousemoveTimer.Start();
            }
        }

        private void Mediacontrol_MouseLeave(object sender, MouseEventArgs e)
        {
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaControllerView as UIElement, true);
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
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaControllerView as UIElement, null);
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
            (IVideoElement as Window).Dispatcher.Invoke(new Action(() => ResetVisibilityAnimation()));
        }

        public void FullScreenSettings()
        {
            if (screensetting == SCREENSETTINGS.Fullscreen) return;
            CanAnimate = true;
            MediaControlExtension.SetCanAnimateControl((IMediaControllerView as UserControl), true);
            screensetting = SCREENSETTINGS.Fullscreen;
            if (IVideoElement != null)
            {
                (IVideoElement as MetroWindow).UseNoneWindowStyle = true;
                (IVideoElement as MetroWindow).IgnoreTaskbarOnMaximize = true;
            }
        }

        public void NormalScreenSettings()
        {
            MediaControlExtension.SetCanAnimateControl((IMediaControllerView as UserControl), false);
            CanAnimate = false;
            screensetting = SCREENSETTINGS.Normal;
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
            FullScreenAction();
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
               
                if (CurrentVideoItem != null)
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
                GetVideoItem(vfc as VideoFolderChild);
                return;
            }
            GetVideoItem(vf as VideoFolderChild);
            CommandManager.InvalidateRequerySuggested();
        }

        public void VisibilityAnimation()
        {
            MediaControlExtension.SetIsMouseOverMediaElement(IMediaControllerView as UIElement, null);
            (IVideoElement as Window).Cursor = Cursors.Arrow;
            if (Isloaded && ScreenSetting == SCREENSETTINGS.Normal && !IsFullScreenMode)
            {
                //MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
            }

            this.MousemoveTimer.Start();
        }
        
    }
}
