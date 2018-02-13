using Common.Interfaces;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using VideoPlayer.ViewModel;
using VideoPlayerView.Util;
using VideoPlayerView.ViewModel;

namespace VideoPlayerView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class VideoElement : Window, IVideoElement
    {
        IPlayListClose playlistview;
        public VideoElement()
        {
            InitializeComponent();
            this.DataContext = new VideoElementViewModel();
            this.Loaded += VideoElement_Loaded;
            // videoplayer.MediaPlayer.LoadedBehavior = WPFMediaKit.DirectShow.MediaPlayers.MediaState.Manual;
            this.Closing += VideoElement_Closing;

            var previousExecutionState = NativeMethods.SetThreadExecutionState(
                NativeMethods.ES_CONTINUOUS 
                | NativeMethods.ES_SYSTEM_REQUIRED);

          
        }
        

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            var vm = videoplayer.DataContext as VideoPlayerVM;
            vm.OnDrop(e);
        }

        private void VideoElement_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MediaPlayer.Close();
        }

        private void VideoElement_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VideoElementViewModel;
            vm.Loaded();
            MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
           // IVideoPlayer.MediaPlayer.MediaUriPlayer.MediaPositionChanged += MediaUriPlayer_MediaPositionChanged;
        }

        private void MediaUriPlayer_MediaPositionChanged(object sender, EventArgs e)
        {
            //Dispatcher.BeginInvoke(new Action(() => {
            //    if (IVideoPlayer.MediaPlayer.HasVideo)
            //    {
            //        this.Height = Math.Min(720, IVideoPlayer.MediaPlayer.NaturalVideoHeight + IVideoPlayer.MediaController.ActualHeight);
            //        this.Width = Math.Min(1280, IVideoPlayer.MediaPlayer.NaturalVideoWidth);
            //        IVideoPlayer.MediaPlayer.MediaUriPlayer.MediaPositionChanged -= MediaUriPlayer_MediaPositionChanged;
            //    }
            //}), null);
           
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.HasVideo)
            {
                //+ IVideoPlayer.MediaController.ActualHeight
                this.Height = Math.Min(720, MediaPlayer.NaturalVideoHeight);
                this.Width = Math.Min(1280, MediaPlayer.NaturalVideoWidth);
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            playlistview = (IPlayListClose)this.Template.FindName("playlistview", this);
        }

        public IPlayListClose PlayListView { get { return this.playlistview; } }

        public ISubtitleMediaController IVideoPlayer
        {
            get
            {
                return this.videoplayer;
            }
        }

        UIElement IVideoElement.WindowsTab
        {
            get
            {
                return this.WindowsTab;
            }
        }

        UIElement IVideoElement.WindowsTabDock
        {
            get
            {
                return this.WindowsTabDock;
            }
        }

        public MediaElement MediaPlayer { get { return this.MediaElementPlayer; } }

        public UIElement ParentGrid
        {
            get { return this._videoContent; }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            this.Focus();
            
        }
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            
            if (MediaControllerVM.Current.IsRewindOrFastForward)
            {
                ((IVideoPlayer as UserControl).DataContext as VideoPlayerVM).RestoreMediaState();
            }
            ((IVideoPlayer as UserControl).DataContext as VideoPlayerVM).VisibilityAnimation();
        }
        public void SetTopMost()
        {
            if (!this.Topmost)
            {
                this.Topmost = true;
            }
            else
            {
                this.Topmost = false;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is Rectangle || e.OriginalSource is MediaElement)) return;
            var vm = (VideoPlayerVM)videoplayer.DataContext;
            vm.OnMouseDoubleClick(e);
        }
    }
}
