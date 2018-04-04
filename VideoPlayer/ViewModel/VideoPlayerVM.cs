using Common.Interfaces;
using Common.Model;
using Common.Util;
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

        private bool allowautoresize = true;

        public bool AllowAutoResize
        {
            get { return allowautoresize; }
            set { allowautoresize = value; }
        }

        private string minimizemediactrltext;

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
            IVideoElement.MediaPlayer.MediaFailed += MediaElementPlayer_MediaFailed;

            // IVideoPlayer.MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
            IVideoElement.MediaPlayer.MediaEnded += MediaElementPlayer_MediaEnded;
            IVideoElement.MediaPlayer.MouseMove += ParentGrid_MouseMove;
           
           
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

        //private void MediaPlayer_MediaFailed(object sender, WPFMediaKit.DirectShow.MediaPlayers.MediaFailedEventArgs e)
        //{
        //    (IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
        //    {
        //        ResetVisibilityAnimation();
        //    }), null);
        //}

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
            get { return MediaControllerVM.Current; }
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
                    VideoPlayerView.OnScreenSettingsCanged(new object[] { value, SCREENSETTINGS.Fullscreen });
                    IsFullScreenMode = false;
                }
                else
                {
                    VideoPlayerView.OnScreenSettingsCanged(new object[] { value, SCREENSETTINGS.Normal });
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
           // VideoPlayerView.ControlHolder.Children.Remove(IVideoPlayer.MediaController);
            MediaControllerVM.CanAnimate = true;
            //IVideoPlayer.MediaController.Margin = new Thickness(5, 32, 10, 0);
            MediaControlExtension.SetCanAnimateControl(ISubtitleMediaController.MediaController, true);
            //IVideoPlayer.CanvasEnvironment.Children.Add(IVideoPlayer.MediaController);
           // VideoPlayerView.SubviewBox.Margin = new Thickness(3, 0, 3, 88);
            screensetting = SCREENSETTINGS.Fullscreen;
            MinimizeMediaCtrlText = "Restore MediaControl";
        }

        public void NormalScreenSettings()
        {
           // if (screensetting == SCREENSETTINGS.Normal) return;
            MediaControlExtension.SetCanAnimateControl(ISubtitleMediaController.MediaController, false);
            MediaControllerVM.CanAnimate = false;
           // IVideoPlayer.CanvasEnvironment.Children.Remove(IVideoPlayer.MediaController);
            //IVideoPlayer.MediaController.Margin = new Thickness(0,0,0,0);
           // VideoPlayerView.ControlHolder.Children.Add(IVideoPlayer.MediaController);
            ResetVisibilityAnimation();
           // VideoPlayerView.SubviewBox.Margin = new Thickness(3, 0, 3, 37);
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
            if (!MediaControllerVM.IsPlaying)
            {
                this.MousemoveTimer.Stop();
                return;
            }
            if (!MediaControllerVM.IsMouseControlOver )
            {
                (IVideoElement as Window).Cursor = Cursors.None;
                //if (!MediaControllerVM.Current.IsFullScreenMode)
                //{
                //IVideoElement.WindowsTab.Visibility = Visibility.Collapsed;
                MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, false);
                //}

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
                    //IVideoElement.WindowsTab.Visibility = Visibility.Collapsed;
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
            // || e.Source is FileView
            if (e.Handled)
            {
                return;
            }

            //if (MediaController.MediaState == MediaState.Playing)
            //{
            switch (MediaControllerVM.MediaState)
            {
                case MediaState.Playing:
                case MediaState.Paused:
                case MediaState.Stopped:
                    if (ScreenSetting != SCREENSETTINGS.Fullscreen)
                    {
                        // StateB4Max = this.WindowState;
                        //   this.WindowState = System.Windows.WindowState.Maximized;
                        // VM.WindowsControl = System.Windows.Visibility.Collapsed;
                        ScreenSetting = SCREENSETTINGS.Fullscreen;
                    }
                    else
                    {
                        //  this.WindowState = StateB4Max;
                        //  VM.WindowsControl = System.Windows.Visibility.Visible;
                        ScreenSetting = SCREENSETTINGS.Normal;
                        
                        //  VM.SetStyleOnWindowState(StateB4Max);
                    }
                    break;
                
                    //  this.WindowState = System.Windows.WindowState.Maximized;
                default:
                    break;
            }
            //  }

        }

        private bool isfullscreenmode;
        public bool IsFullScreenMode {
            get { return isfullscreenmode; }
            set { isfullscreenmode = value; RaisePropertyChanged(() => this.IsFullScreenMode); }
        }

        public void OnDrop(DragEventArgs e)
        {
            //base.OnDrop(e);
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
                    for (int i = 0; i < filePathInfo.Length; i++)
                    {
                        FileInfo file = new FileInfo(filePathInfo[i]);
                        if (file.Extension == ".srt")
                        {
                            SubtitleFilesModel.Add(MediaControllerVM.CurrentVideoItem.SubPath, 
                                file.FullName);                            
                        }
                    }
                    ISubtitleMediaController.Subtitle.LoadSub(MediaControllerVM.CurrentVideoItem.SubPath.First());
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

        private void Viewbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
           // (IVideoPlayer.CanvasEnvironment as DragCanvas).DragCanvas_OnPreviewMouseLeftButtonDown(sender, e);
        }

        private void ParentGrid_MouseMove(object sender, MouseEventArgs e)
        {
            //if (ScreenSetting == SCREENSETTINGS.Normal)
            //{
            //    return;
            //}

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

        private void MediaElementPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            ResetVisibilityAnimation();
        }

        private void MediaElementPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ResetVisibilityAnimation();
        }

        private void ResetVisibilityAnimation()
        {
            //(IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            //{
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


            //}), null);
        }

        public void VisibilityAnimation()
        {
            MediaControlExtension.SetIsMouseOverMediaElement(ISubtitleMediaController.MediaController as UIElement, null);
            (IVideoElement as Window).Cursor = Cursors.Arrow;
            if (Isloaded && ScreenSetting == SCREENSETTINGS.Normal && !IsFullScreenMode)
            {
                // IVideoElement.WindowsTab.Visibility = Visibility.Visible;
                MediaControlExtension.SetAnimateWindowsTab(IVideoElement.WindowsTab as UIElement, true);
            }

            this.MousemoveTimer.Start();
        }


    }
}
