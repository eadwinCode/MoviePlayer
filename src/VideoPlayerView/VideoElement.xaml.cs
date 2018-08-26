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
using Movies.MoviesInterfaces;
using VideoPlayerControl;
using Microsoft.Practices.ServiceLocation;
using Movies.MediaService.Interfaces;
using Movies.MediaService.Service;

namespace VideoPlayerView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class VideoElement : MetroWindow, IVideoElement
    {
        //IPlayListClose playlistview;
        public IMediaPlayerService imediaplayerservice;

        private IPlaylistViewMediaPlayerView playlistview;
        public IPlaylistViewMediaPlayerView PlayListView
        {
            get
            {
                if (playlistview == null)
                {
                    playlistview = FilePlayerMananger.PlaylistManagerViewModel.
                    GetPlaylistView(FilePlayerMananger.MediaControllerViewModel) as IPlaylistViewMediaPlayerView;
                }
                return playlistview;
            }
        }

        IMediaController ivideoplayercontroller;
        public IMediaController IVideoPlayerController
        {
            get
            {
                if (ivideoplayercontroller == null)
                    ivideoplayercontroller = new SubtitleMediaController();
                return this.ivideoplayercontroller;
            }
        }

        IPlayFile FilePlayerMananger
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }

        IMediaPlayerService MediaPlayerService
        {
            get
            {
                return FilePlayerMananger.MediaPlayerService;
            }
        }

        public UIElement ParentGrid
        {
            get { return this._videoContent; }
        }

        public ContentControl ContentDockRegion
        {
            get { return this.contentdockregion; }
        }

        public VideoElement()
        {
            InitializeComponent();
            var VideoelementViemModel = new VideoElementViewModel();
            this.DataContext = VideoelementViemModel;
            this.Loaded += (s,e)=> {
                VideoelementViemModel.Loaded();
                MediaPlayerService.OnMediaOpened += MediaControllerInstance_VlcMediaOpened;
                this.FocusElement();
            };

            this.MediaControlRegion.Content = IVideoPlayerController;
            this.PlaylistViewRegion.Content = PlayListView;
            this.MediaElementViewRegion.Content = MediaPlayerService.VideoPlayer;
        }


        private void MediaElementViewRegion_Drop(object sender, DragEventArgs e)
        {
            var vm = (IVideoPlayerController as UserControl).DataContext as VideoPlayerVM;
            vm.OnDrop(e);
        }
        
        private void FocusElement()
        {
            this.Focus();
            (this.IVideoPlayerController as UserControl).Focus();
        }

        private void MediaControllerInstance_VlcMediaOpened(object sender, EventArgs e)
        { 
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var vm = (IVideoPlayerController as UserControl).DataContext as VideoPlayerVM;
                if (!vm.AllowAutoResize) return;
                this.Height = Math.Min(720, MediaPlayerService.PixelHeight* 0.6666666666667);
                this.Width = Math.Min(1280, MediaPlayerService.PixelWidth * 0.6666666666667);
               //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }));
        }
        
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            (this.IVideoPlayerController as UserControl).Focus();
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            
            if (FilePlayerMananger.MediaControllerViewModel.IsRewindOrFastForward)
            {
                ((IVideoPlayerController as UserControl).DataContext as VideoPlayerVM).RestoreMediaState();
            }
            ((IVideoPlayerController as UserControl).DataContext as VideoPlayerVM).VisibilityAnimation();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (!(e.Source is VlcPlayer)) return;
            var vm = (VideoPlayerVM)(IVideoPlayerController as UserControl).DataContext;
            vm.OnMouseDoubleClick(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MediaPlayerService.Dispose();
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

    }
}
