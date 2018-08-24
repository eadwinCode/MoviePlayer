using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using VideoPlayerControl.ViewModel;

namespace VideoPlayerControl
{
    /// <summary>
    /// Interaction logic for VideoPlayerView.xaml
    /// </summary>
    public partial class SubtitleMediaController : UserControl, IMediaController
    {
        // public Canvas CanvasEnvironment { get { return this.mycanvas; } }

        //public ISubtitle Subtitle { get { return this.SubtitleText; } }

        IControllerView mediacontroller;
        public IControllerView MediaController
        {
            get
            {
                if (mediacontroller == null)
                    mediacontroller = new MediaController(FilePlayerManager.MediaControllerViewModel);
                return this.mediacontroller;
            }
        }

       // public MediaUriElement MediaPlayer => this.MediaElementPlayer;

        public SubtitleMediaController()
        {
            InitializeComponent();
            this.DataContext = new VideoPlayerVM(this);
            this.Loaded += VideoPlayerView_Loaded;

            this.controlRegion.Content = this.MediaController;
            //this.MediaElementPlayer.VideoRenderer =WPFMediaKit.DirectShow.MediaPlayers.VideoRendererType.EnhancedVideoRenderer;
        }

        public event EventHandler ScreenSettingsChanged;

        private void VideoPlayerView_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (VideoPlayerVM)this.DataContext;
            
            vm.Loaded();
            this.FocusableChanged += SubtitleMediaController_FocusableChanged;
           
        }

        private void SubtitleMediaController_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        #region Not Necessary
        private void CloseSecondView(object s)
        {
            //if (s == null)
            //{
            //    if (ScreenSetting == SCREENSETTINGS.Normal)
            //    {
            //        SubviewBox.Margin = new Thickness(3, 0, 3, 14);
            //    }
            //    else
            //    {
            //        SubviewBox.Margin = new Thickness(3, 0, 3, 72);
            //    }
            //}
            //else
            //{
            //    SubviewBox.Margin = new Thickness(3, 0, 3, 5);
            //}
        }


        //private void MediaElementPlayer_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    if (MediaController.CanAnimate && MediaController.IsPlaying)
        //    {
        //        this.MousemoveTimer.Start();
        //    }
        //}

        //private void MediaElementPlayer_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (!MediaController.MovieTitle_Tab.IsCanvasDrag)
        //    {
        //        if (MediaController.IsMouseControlOver)
        //        {
        //            return;
        //        }
        //        this.MousemoveTimer.Start();
        //    }

        //} 
        #endregion

        internal void OnScreenSettingsChanged(object sender)
        {
            if (ScreenSettingsChanged != null)
            {
                ScreenSettingsChanged(sender, null);
            }
        }

        public IPlayFile FilePlayerManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }

    }

    public enum SCREENSETTINGS
    {
        Normal,
        Fullscreen
    };
}
