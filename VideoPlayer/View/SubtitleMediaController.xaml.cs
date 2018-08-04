using Common.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using VideoPlayerControl.ViewModel;

namespace VideoPlayerControl
{
    /// <summary>
    /// Interaction logic for VideoPlayerView.xaml
    /// </summary>
    public partial class SubtitleMediaController : UserControl, Common.Interfaces.IMediaController
    {
       // public Canvas CanvasEnvironment { get { return this.mycanvas; } }

        //public ISubtitle Subtitle { get { return this.SubtitleText; } }

        public UserControl MediaController { get { return this.mediacontrol; } }

       // public MediaUriElement MediaPlayer => this.MediaElementPlayer;

        public SubtitleMediaController()
        {
            InitializeComponent();
            this.DataContext = new VideoPlayerVM(this);
            this.Loaded += VideoPlayerView_Loaded;
            //this.MediaElementPlayer.VideoRenderer =WPFMediaKit.DirectShow.MediaPlayers.VideoRendererType.EnhancedVideoRenderer;
        }

        public event EventHandler ScreenSettingsChanged;

        private void VideoPlayerView_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (VideoPlayerVM)this.DataContext;
            
            vm.Loaded();
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
      

       

    }

    public enum SCREENSETTINGS
    {
        Normal,
        Fullscreen
    };
}
