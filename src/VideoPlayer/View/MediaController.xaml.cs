using Common.Util;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VideoPlayerControl.ViewModel;
using Microsoft.Practices.ServiceLocation;

namespace VideoPlayerControl
{
    /// <summary>
    /// Interaction logic for MediaController.xaml
    /// </summary>
    public partial class MediaController : UserControl,IControllerView
    {
        private Slider CurrentSlider;

        public IMovieTitleBar MovieTitle_Tab { get { return this.MovieBoard; } }

        //public Panel GroupedControls { get { return this.GroupControl; } }

        public VolumeControl VolumeControl
        {
            get
            {
                return volCtrl;
            }
        }

        internal MediaController(MediaControllerViewModel mediacontrollerviewmodel)
        {
            InitializeComponent();

             this.DataContext = mediacontrollerviewmodel;
            mediacontrollerviewmodel. PositionSlideTimerTooltip = new DispatcherTimer(DispatcherPriority.Background);
            mediacontrollerviewmodel.PositionSlideTimerTooltip.Tick += PositionSlideTimerTooltip_Tick;
            mediacontrollerviewmodel.PositionSlideTimerTooltip.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void PlNext_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = this.DataContext as MediaControllerViewModel;
            if (vm.IsPlaying && vm.DragPositionSlider.Value < 50)
            {
                MovieTitle_Tab.MovieText = "Previous:- " + FilePlayerManager.PlaylistManagerViewModel.WhatsPreviousItem();
                Button tt = (Button)sender;
                tt.ToolTip = MovieTitle_Tab.MovieText;
            }
            
        }
        
        private void Previous_MouseEnter(object sender, MouseEventArgs e)
        {
            var vm = this.DataContext as MediaControllerViewModel;
            if (vm.IsPlaying)
            {
               FilePlayerManager.MediaControllerViewModel.IMovieTitle_Tab.MovieText =
                    "Next:- " + FilePlayerManager.PlaylistManagerViewModel.WhatsNextItem();
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
                var currentpos =FilePlayerManager.MediaControllerViewModel.GetMousePointer(CurrentSlider);
                CurrentSlider.Value = Math.Round((currentpos * CurrentSlider.Maximum), 10);
                DragCom();
            }
        }

        void PositionSlideTimerTooltip_Tick(object sender, EventArgs e)
        {
            if (CurrentSlider == null)
            {
                return;
            }
            var x = Mouse.GetPosition(CurrentSlider).X;
            var ratio = x / CurrentSlider.ActualWidth;
            UpdateText(x, ratio, TimeSpan.FromSeconds(Math.Round((ratio *
                CurrentSlider.Maximum), 10)).ToString(@"hh\:mm\:ss"));
        }
        
        private void UpdateText(double x, double ratio, string p)
        {
            double nn = 0;

            MovieTitle_Tab.MovieText = "Seeking/ " + p;

            //if (x < 0)
            //{
            //    nn = 0;
            //}
            //else if (x > CurrentSlider.ActualWidth)
            //{
            //    nn = CurrentSlider.ActualWidth;
            //}
            //else { nn = x; }
        }

        private void DragCom()
        {
            FilePlayerManager.MediaPlayerService.CurrentTimer
                = TimeSpan.FromSeconds(CurrentSlider.Value);
            
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
            if (FilePlayerManager.MediaControllerViewModel.MediaState == MovieMediaState.Playing 
                ||FilePlayerManager.MediaControllerViewModel.MediaState == MovieMediaState.Paused)
            {
                CurrentSlider.Cursor = Cursors.Hand;
               FilePlayerManager.MediaControllerViewModel.PositionSlideTimerTooltip.Start();
            }

        }
        
        private void PositionSlider_MouseMove(object sender, MouseEventArgs e)
        {
            CurrentSlider = sender as Slider;
            if (FilePlayerManager.MediaControllerViewModel.MediaState == MovieMediaState.Playing ||
               FilePlayerManager.MediaControllerViewModel.MediaState == MovieMediaState.Paused)
            {
                CurrentSlider.Cursor = Cursors.Hand;
            }

        }

        private void PositionSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
           FilePlayerManager.MediaControllerViewModel.PositionSlideTimerTooltipStop();
        }

        

        private void PositionSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            if (!FilePlayerManager.MediaPlayerService.IsSeekable) return;
            if (FilePlayerManager.MediaControllerViewModel.MediaState == MovieMediaState.Playing)
                FilePlayerManager.MediaPlayerService.Pause();
            
            if (FilePlayerManager.MediaControllerViewModel.VolumeState == VolumeState.Active)
            {
                FilePlayerManager.MediaPlayerService.IsMute = true;
            }
        }

        private void PositionSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            FilePlayerManager.MediaControllerViewModel.IsDragging = false;
            FilePlayerManager.MediaPlayerService.CurrentTimer
                = TimeSpan.FromSeconds(CurrentSlider.Value);
            FilePlayerManager.MediaControllerViewModel.PositionSlideTimerTooltip.Start();
            
            FilePlayerManager.MediaPlayerService.Play();

            if (FilePlayerManager.MediaControllerViewModel.VolumeState
                == VolumeState.Active)
            {
                FilePlayerManager.MediaPlayerService.IsMute = false;
            }
        }

        private void PositionSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
           FilePlayerManager.MediaControllerViewModel.PositionSlideTimerTooltip.Start();
        }
        #endregion

        private void PositionSlider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            CurrentSlider.Cursor = Cursors.Hand;
           FilePlayerManager.MediaControllerViewModel.IsDragging = true;
            FilePlayerManager.MediaPlayerService.CurrentTimer
                = TimeSpan.FromSeconds(CurrentSlider.Value);
        }

        IPlayFile FilePlayerManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }
    }
}

