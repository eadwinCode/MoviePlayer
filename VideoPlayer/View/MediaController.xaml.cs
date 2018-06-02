using Common.Util;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VideoPlayer.ViewModel;

namespace VideoPlayer
{
    /// <summary>
    /// Interaction logic for MediaController.xaml
    /// </summary>
    public partial class MediaController : UserControl,IMediaController
    {
        private Slider CurrentSlider;

        public MovieTitle_Tab MovieTitle_Tab { get { return this.MovieBoard; } }

        public Panel GroupedControls { get { return this.GroupControl; } }

        public VolumeControl VolumeControl
        {
            get
            {
                return volCtrl;
            }
        }

        public MediaController()
        {
            InitializeComponent();
            this.DataContext = new MediaControllerVM();
            this.Loaded += MediaControllerVM.MediaControllerInstance.MediaController_Loaded;

            MediaControllerVM.MediaControllerInstance.positionSlideTimerTooltip = new DispatcherTimer(DispatcherPriority.Background);
            MediaControllerVM.MediaControllerInstance.positionSlideTimerTooltip.Tick += positionSlideTimerTooltip_Tick;
            MediaControllerVM.MediaControllerInstance.positionSlideTimerTooltip.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void PlNext_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = this.DataContext as MediaControllerVM;
            if (vm.IsPlaying && vm.DragPositionSlider.Value < 50)
            {
                MovieTitle_Tab.MovieText = "Previous:- " + vm.Playlist.WhatsPreviousItem();
                Button tt = (Button)sender;
                tt.ToolTip = MovieTitle_Tab.MovieText;
            }
            
        }
        
        private void Previous_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = this.DataContext as MediaControllerVM;
            if (vm.IsPlaying)
            {
                MediaControllerVM.MediaControllerInstance.MovieTitle_Tab.MovieText =
                    "Next:- " + vm.Playlist.WhatsNextItem();
                Button tt = (Button)sender;
                tt.ToolTip = MovieTitle_Tab.MovieText;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            e.Handled = true;
        }

        private void Previous_MouseLeave(object sender, MouseEventArgs e)
        {
            MovieTitle_Tab.MovieText = null;
        }

        private void PlNext_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            MovieTitle_Tab.MovieText = null;
        }

        #region PositionSlider

        private void PositionSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentSlider = sender as Slider;
            CurrentSlider.Cursor = Cursors.Hand;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var currentpos = MediaControllerVM.MediaControllerInstance.GetMousePointer(CurrentSlider);
                CurrentSlider.Value = Math.Round((currentpos * CurrentSlider.Maximum), 10);
                DragCom();
            }
        }

        void positionSlideTimerTooltip_Tick(object sender, EventArgs e)
        {
            if (CurrentSlider == null)
            {
                return;
            }
            var x = Mouse.GetPosition(CurrentSlider).X;
            var ratio = x / CurrentSlider.ActualWidth;
            UpdateText(x, ratio, TimeSpan.FromSeconds(Math.Round((ratio *
                CurrentSlider.Maximum), 10)).ToString(@"hh\:mm\:ss"));
            //positionslider.ToolTip = CommonHelper.SetDuration(Math.Round((ratio *
            //positionslider.Maximum), 10));
        }

        

        private void UpdateText(double x, double ratio, string p)
        {
            double nn = 0;

            MovieTitle_Tab.MovieText = "Seeking/ " + p;
            // positionslider.ToolTip = p;
            //if (ToolTipTxt.Visibility != System.Windows.Visibility.Visible) { ToolTipTxt.Visibility = System.Windows.Visibility.Visible; }

            //ToolTipTxt.ToolTip = p;

            if (x < 0)
            {
                nn = 0;
            }
            else if (x > CurrentSlider.ActualWidth)
            {
                nn = CurrentSlider.ActualWidth;
            }
            else { nn = x; }

            // Canvas.SetLeft(ToolTipTxt, nn);
        }

        private void DragCom()
        {
            MediaControllerVM.MediaControllerInstance.IVideoElement.MediaPlayer.Time
                = TimeSpan.FromSeconds(CurrentSlider.Value);

            //MediaControllerVM.Current.IVideoPlayer.MediaPlayer.MediaPosition = (long)(CurrentSlider.Value * 10000000);
            #region Not_Neccessary
            //mediaElement1.MediaPosition = (long)positionslider.Value * 10000000;
            // IsDragging = false;
            //if (IsPlaying)
            //{
            //    mediaPlayer.Play();
            //}
            //else
            //{
            //    if (mediaPlayer.CanPause) 
            //    {
            //        mediaPlayer.Pause(); 
            //        IsPlaying = false; 
            //    }
            //}   
            #endregion
        }

        private void PositionSlider_MouseEnter(object sender, MouseEventArgs e)
        {
            CurrentSlider = sender as Slider;
            if (MediaControllerVM.MediaControllerInstance.MediaState == MediaState.Playing 
                || MediaControllerVM.MediaControllerInstance.MediaState == MediaState.Paused)
            {
                CurrentSlider.Cursor = Cursors.Hand;
                // positionSlideTimer.Start();
                MediaControllerVM.MediaControllerInstance.positionSlideTimerTooltip.Start();
            }

        }



        private void PositionSlider_MouseMove(object sender, MouseEventArgs e)
        {
            CurrentSlider = sender as Slider;
            if (MediaControllerVM.MediaControllerInstance.MediaState == MediaState.Playing ||
                MediaControllerVM.MediaControllerInstance.MediaState == MediaState.Paused)
            {
                CurrentSlider.Cursor = Cursors.Hand;
            }

        }

        private void PositionSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            MediaControllerVM.MediaControllerInstance.positionSlideTimerTooltipStop();
        }

        

        private void PositionSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!MediaControllerVM.MediaControllerInstance.IVideoElement.MediaPlayer.VlcMediaPlayer.IsSeekable) return;
            if (MediaControllerVM.MediaControllerInstance.MediaState == MediaState.Playing)
                MediaControllerVM.MediaControllerInstance.IVideoElement.MediaPlayer.Pause();

            //MediaControllerVM.Current.IVideoElement.MediaPlayer.ScrubbingEnabled = true;
            if (MediaControllerVM.MediaControllerInstance.VolumeState == VolumeState.Active)
            {
                MediaControllerVM.MediaControllerInstance.IVideoElement.MediaPlayer.IsMute = true;
            }
        }

        private void PositionSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            MediaControllerVM.MediaControllerInstance.IsDragging = false;
            MediaControllerVM.MediaControllerInstance.IVideoElement.MediaPlayer.Time
                = TimeSpan.FromSeconds(CurrentSlider.Value);
            MediaControllerVM.MediaControllerInstance.positionSlideTimerTooltip.Start();

           // MediaControllerVM.Current.IVideoElement.MediaPlayer.ScrubbingEnabled = false;
            if (MediaControllerVM.MediaControllerInstance.MediaState == MediaState.Playing)
                MediaControllerVM.MediaControllerInstance.IVideoElement.MediaPlayer.Play();
            
            if (MediaControllerVM.MediaControllerInstance.VolumeState
                == VolumeState.Active)
            {
                MediaControllerVM.MediaControllerInstance.IVideoElement.MediaPlayer.IsMute = false;
            }
        }

        private void PositionSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MediaControllerVM.MediaControllerInstance.positionSlideTimerTooltip.Start();
            //Console.WriteLine("positionslider_MouseUp");
        }
        #endregion

        private void PositionSlider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            CurrentSlider.Cursor = Cursors.Hand;
            MediaControllerVM.MediaControllerInstance.IsDragging = true;
            MediaControllerVM.MediaControllerInstance.IVideoElement.MediaPlayer.Time
                = TimeSpan.FromSeconds(CurrentSlider.Value);
        }
    }

    public enum MediaState
    {
        Playing,
        Paused,
        Stopped,
        Finished,
        Failed
    };
}

