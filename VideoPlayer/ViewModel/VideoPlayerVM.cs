using Common.ApplicationCommands;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Meta.Vlc;
using Meta.Vlc.Interop.Media;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VideoComponent.BaseClass;
using VirtualizingListView.ViewModel;
using WPF.JoshSmith.Controls;

namespace VideoPlayer.ViewModel
{
    public partial class VideoPlayerVM:NotificationObject
    {
        private SubtitleMediaController VideoPlayerView;
        private ISubtitleMediaController ISubtitleMediaController;
        public SubtitleFilesModel CurrentSubtitle;
        private string minimizemediactrltext;

        private bool allowautoresize = true;
        public bool AllowAutoResize
        {
            get { return allowautoresize; }
            set { allowautoresize = value; }
        }
        public string MinimizeMediaCtrlText
        {
            get { return minimizemediactrltext; }
            set { minimizemediactrltext = value; RaisePropertyChanged(() => this.MinimizeMediaCtrlText); }
        }


        public VideoPlayerVM(ISubtitleMediaController ivideoplayer)
        {
            this.ISubtitleMediaController = ivideoplayer;
            VideoPlayerView = (SubtitleMediaController)ivideoplayer;
            MousemoveTimer = new DispatcherTimer(DispatcherPriority.Background);
            ScreenSetting = SCREENSETTINGS.Normal;
            VideoPlayerView.Loaded += VideoPlayerView_Loaded;
            
        }
        
        private void Init()
        {
            ISubtitleMediaController.MediaController.MouseLeave += mediacontrol_MouseLeave;
            ISubtitleMediaController.MediaController.MouseEnter += Mediacontrol_MouseEnter;
            IVideoElement.ParentGrid.MouseMove += ParentGrid_MouseMove;
            IVideoElement.MediaPlayer.VlcMediaPlayer.EndReached += VlcMediaPlayer_EndReached;
            IVideoElement.MediaPlayer.VlcMediaPlayer.EncounteredError += VlcMediaPlayer_EncounteredError;
            IVideoElement.MediaPlayer.MouseMove += ParentGrid_MouseMove;
            
        }

        private void VlcMediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            ResetVisibilityAnimation();
        }

        private void VlcMediaPlayer_EndReached(object sender, ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            ResetVisibilityAnimation();
        }

        private void WindowsTab_MouseLeave(object sender, MouseEventArgs e)
        {
            MediaControlExtension.SetIsMouseOverMediaElement(ISubtitleMediaController.MediaController as UIElement, true);
            if (!IsFullScreenMode)
            {
                MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
            }
            else
            {
                MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false);
            }
            this.MousemoveTimer.Start();
        }

        private void WindowsTab_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MousemoveTimer.Stop();
            MediaControlExtension.SetIsMouseOverMediaElement(ISubtitleMediaController.MediaController as UIElement, null);
            MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
        }

        private void VideoPlayerView_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            MousemoveTimer.Interval = TimeSpan.FromSeconds(5);
            MousemoveTimer.Tick += MousemoveTimer_Tick;
            this.MousemoveTimer.Stop();

            RegisterCommands();
        }

        private DispatcherTimer MousemoveTimer;

        private DelegateCommand _showFilexp;

        private MediaControllerVM MediaControllerVM
        {
            get { return MediaControllerVM.MediaControllerInstance; }
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

        public void FullScreenSettings()
        {
            if (screensetting == SCREENSETTINGS.Fullscreen) return;
            MediaControllerVM.CanAnimate = true;
            MediaControlExtension.SetCanAnimateControl(ISubtitleMediaController.MediaController, true);
            screensetting = SCREENSETTINGS.Fullscreen;
            MinimizeMediaCtrlText = "Restore MediaControl";
        }

        public void NormalScreenSettings()
        {
            MediaControlExtension.SetCanAnimateControl(ISubtitleMediaController.MediaController, false);
            MediaControllerVM.CanAnimate = false;
            screensetting = SCREENSETTINGS.Normal;
            MinimizeMediaCtrlText = "Minimize MediaControl";
        }

        public DelegateCommand ShowFileExp
        {
            get
            {
                if (_showFilexp == null)
                {
                    _showFilexp = new DelegateCommand(() =>
                    {
                        CollectionViewModel.Instance.CloseFileExplorerAction(this);
                    });

                }
                return _showFilexp;
            }
        }

        private SCREENSETTINGS screensetting;

        internal bool Isloaded;

        void MousemoveTimer_Tick(object sender, EventArgs e)
        {
            if(MediaControllerVM == null) { MousemoveTimer.Stop();return; }
            if (!MediaControllerVM.IsPlaying)
            {
                this.MousemoveTimer.Stop();
                return;
            }
            if (!MediaControllerVM.IsMouseControlOver )
            {
                (IVideoElement as Window).Cursor = Cursors.None;
                MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false);
                MediaControlExtension.SetIsMouseOverMediaElement(ISubtitleMediaController.MediaController as UIElement, false);
                this.MousemoveTimer.Stop();
            }
            else if (MediaControllerVM.IsMouseControlOver)
            {
                this.MousemoveTimer.Stop();
            }
            else
            {
                (IVideoElement as Window).Cursor = Cursors.None;

                if (!IsFullScreenMode)
                {
                    MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false);
                }
                else
                {
                    MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
                }
                this.MousemoveTimer.Stop();
            }
        }

        private void Mediacontrol_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MousemoveTimer.Stop();
            MediaControlExtension.SetIsMouseOverMediaElement(ISubtitleMediaController.MediaController as UIElement, null);
        }

        public void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            RestoreScreen();
        }

        private void RestoreScreen()
        {
            switch (MediaControllerVM.MediaState)
            {
                case MediaState.Playing:
                case MediaState.Paused:
                case MediaState.Stopped:
                    if (ScreenSetting != SCREENSETTINGS.Fullscreen)
                    {
                        ScreenSetting = SCREENSETTINGS.Fullscreen;
                    }
                    else
                    {
                        ScreenSetting = SCREENSETTINGS.Normal;
                    }
                    break;
                default:
                    break;
            }
        }

        private bool isfullscreenmode;
        public bool IsFullScreenMode {
            get { return isfullscreenmode; }
            set { isfullscreenmode = value; RaisePropertyChanged(() => this.IsFullScreenMode); }
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
               
                if (MediaControllerVM.CurrentVideoItem != null)
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
                MediaControllerVM.GetVideoItem(vfc as VideoFolderChild);
                return;
            }
            MediaControllerVM.GetVideoItem(vf as VideoFolderChild);
            CommandManager.InvalidateRequerySuggested();
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
                    SubtitleFilesModel.Add(MediaControllerVM.CurrentVideoItem.SubPath,
                        file.FullName, SubtitleType.SubFile);
                }
            }
            if (issubfile)
            {
                VideoPlayerCommands.SelectedSub.Execute(MediaControllerVM.CurrentVideoItem.SubPath.Last(), (Window)IVideoElement);
            }
        }
        
        private void ParentGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MediaControllerVM.IsMouseControlOver)
            {
                ResetVisibilityAnimation();
                this.MousemoveTimer.Start();
            }
        }

        private void mediacontrol_MouseLeave(object sender, MouseEventArgs e)
        {
            MediaControlExtension.SetIsMouseOverMediaElement(ISubtitleMediaController.MediaController as UIElement, true);
            if (!IsFullScreenMode)
            {
                MediaControlExtension.SetAnimateWindowsTab( IVideoElement.WindowsTab as UIElement,true);
            }
            else
            {
                MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false);
            }
            this.MousemoveTimer.Start();
        }
        
        private void ResetVisibilityAnimation()
        {
            (IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            {
            if (!MediaControllerVM.IsPlaying) return;
            this.MousemoveTimer.Stop();
            MediaControlExtension.SetIsMouseOverMediaElement(ISubtitleMediaController.MediaController as UIElement, null);
            (IVideoElement as Window).Cursor = Cursors.Arrow;
            if (Isloaded && ScreenSetting == SCREENSETTINGS.Normal && !IsFullScreenMode)
            {
                // IVideoElement.WindowsTab.Visibility = Visibility.Visible;
                MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
            }
            //else { MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false); }


            }), null);
        }

        public void VisibilityAnimation()
        {
            MediaControlExtension.SetIsMouseOverMediaElement(ISubtitleMediaController.MediaController as UIElement, null);
            (IVideoElement as Window).Cursor = Cursors.Arrow;
            if (Isloaded && ScreenSetting == SCREENSETTINGS.Normal && !IsFullScreenMode)
            {
                MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
            }

            this.MousemoveTimer.Start();
        }
        
    }
}
