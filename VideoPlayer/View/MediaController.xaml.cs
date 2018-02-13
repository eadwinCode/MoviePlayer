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
            this.DataContext = MediaControllerVM.Current;
            this.Loaded += MediaControllerVM.Current.MediaController_Loaded;

            MediaControllerVM.Current.positionSlideTimerTooltip = new DispatcherTimer(DispatcherPriority.Background);
            MediaControllerVM.Current.positionSlideTimerTooltip.Tick += positionSlideTimerTooltip_Tick;
            MediaControllerVM.Current.positionSlideTimerTooltip.Interval = TimeSpan.FromMilliseconds(100);
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
                MediaControllerVM.Current.MovieTitle_Tab.MovieText =
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
                var currentpos = MediaControllerVM.Current.GetMousePointer(CurrentSlider);
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
            MediaControllerVM.Current.IVideoElement.MediaPlayer.Position
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
            if (MediaControllerVM.Current.MediaState == MediaState.Playing 
                || MediaControllerVM.Current.MediaState == MediaState.Paused)
            {
                CurrentSlider.Cursor = Cursors.Hand;
                // positionSlideTimer.Start();
                MediaControllerVM.Current.positionSlideTimerTooltip.Start();
            }

        }



        private void PositionSlider_MouseMove(object sender, MouseEventArgs e)
        {
            CurrentSlider = sender as Slider;
            if (MediaControllerVM.Current.MediaState == MediaState.Playing ||
                MediaControllerVM.Current.MediaState == MediaState.Paused)
            {
                CurrentSlider.Cursor = Cursors.Hand;
            }

        }

        private void PositionSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            MediaControllerVM.Current.positionSlideTimerTooltipStop();
        }

        

        private void PositionSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            // var slider = (Slider)sender;
            if (MediaControllerVM.Current.MediaState == MediaState.Playing)
                MediaControllerVM.Current.IVideoElement.MediaPlayer.Pause();

            MediaControllerVM.Current.IVideoElement.MediaPlayer.ScrubbingEnabled = true;
            if (MediaControllerVM.Current.VolumeState == VolumeState.Active)
            {
                MediaControllerVM.Current.IVideoElement.MediaPlayer.IsMuted = true;
            }
            //CurrentSlider.Cursor = Cursors.Hand;
            //MediaControllerVM.Current.IsDragging = true;
            //MediaControllerVM.Current.IVideoPlayer.MediaPlayer.MediaPosition = (long)(CurrentSlider.Value * 10000000);
            //positionSlideTimerTooltipStop();
        }

        private void PositionSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            MediaControllerVM.Current.IsDragging = false;
            MediaControllerVM.Current.IVideoElement.MediaPlayer.Position
                = TimeSpan.FromSeconds(CurrentSlider.Value);
            MediaControllerVM.Current.positionSlideTimerTooltip.Start();

            MediaControllerVM.Current.IVideoElement.MediaPlayer.ScrubbingEnabled = false;
            if (MediaControllerVM.Current.MediaState == MediaState.Playing)
                MediaControllerVM.Current.IVideoElement.MediaPlayer.Play();
            
            if (MediaControllerVM.Current.VolumeState
                == VolumeState.Active)
            {
                MediaControllerVM.Current.IVideoElement.MediaPlayer.IsMuted = false;
            }
            //MediaControllerVM.Current.IVideoPlayer.MediaPlayer.MediaPosition = (long)CurrentSlider.Value * 10000000;
            //MediaControllerVM.Current.IsDragging = false;
            //MediaControllerVM.Current.positionSlideTimerTooltip.Start();
        }

        private void PositionSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MediaControllerVM.Current.positionSlideTimerTooltip.Start();
            //Console.WriteLine("positionslider_MouseUp");
        }
        #endregion

        private void PositionSlider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            CurrentSlider.Cursor = Cursors.Hand;
            MediaControllerVM.Current.IsDragging = true;
            MediaControllerVM.Current.IVideoElement.MediaPlayer.Position
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

