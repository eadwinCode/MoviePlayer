using Common.Interfaces;
using Meta.Vlc.Wpf;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using VideoPlayerControl.ViewModel;
using VideoPlayerView.Util;
using VideoPlayerView.ViewModel;
using MahApps.Metro.Controls;

namespace VideoPlayerView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class VideoElement : MetroWindow, IVideoElement
    {
        //IPlayListClose playlistview;
        public VideoElement()
        {
            InitializeComponent();
            this.DataContext = new VideoElementViewModel();
            this.Loaded += VideoElement_Loaded;

            //var previousExecutionState = NativeMethods.SetThreadExecutionState(
            //    NativeMethods.ES_CONTINUOUS
            //    | NativeMethods.ES_SYSTEM_REQUIRED);
        }
        

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            var vm = videoplayer.DataContext as VideoPlayerVM;
            vm.OnDrop(e);
        }
        

        private void VideoElement_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as VideoElementViewModel;
            vm.Loaded();
            MediaElementPlayer.MediaOpened += MediaControllerInstance_VlcMediaOpened;
           // IVideoPlayer.MediaPlayer.MediaUriPlayer.MediaPositionChanged += MediaUriPlayer_MediaPositionChanged;
        }

        private void MediaControllerInstance_VlcMediaOpened(object sender, EventArgs e)
        { //+ IVideoPlayer.MediaController.ActualHeight
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var vm = videoplayer.DataContext as VideoPlayerVM;
                if (!vm.AllowAutoResize) return;
                this.Height = Math.Min(720, MediaPlayer.VlcMediaPlayer.PixelHeight* 0.6666666666667);
                this.Width = Math.Min(1280, MediaPlayer.VlcMediaPlayer.PixelWidth * 0.6666666666667);
               //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //playlistview = (IPlayListClose)this.Template.FindName("playlistview", this);
        }

        public IPlayListClose PlayListView { get { return this.playlistview; } }

        public IMediaController IVideoPlayer
        {
            get
            {
                return this.videoplayer;
            }
        }

        //UIElement IVideoElement.WindowsTab
        //{
        //    get
        //    {
        //        return this.WindowsTab;
        //    }
        //}

        //UIElement IVideoElement.WindowsTabDock
        //{
        //    get
        //    {
        //        return this.WindowsTabDock;
        //    }
        //}

        public VlcPlayer MediaPlayer { get { return this.MediaElementPlayer; } }

        public UIElement ParentGrid
        {
            get { return this._videoContent; }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            (this.IVideoPlayer as UserControl).Focus();
            //this.Focus();
            
        }
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            
            if (MediaControllerVM.MediaControllerInstance.IsRewindOrFastForward)
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
            if (!(e.Source is VlcPlayer)) return;
            var vm = (VideoPlayerVM)videoplayer.DataContext;
            vm.OnMouseDoubleClick(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MediaElementPlayer.Dispose();
            ApiManager.ReleaseAll();
            base.OnClosing(e);
        }

        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            MenuItem mnItem = sender as MenuItem;
            
            if (!mnItem.IsCheckable)
            {
                mnItem.GetBindingExpression(MenuItem.ItemsSourceProperty).UpdateSource();
            }
             else
                mnItem.GetBindingExpression(MenuItem.IsCheckedProperty).UpdateTarget();
        }
    }
}
