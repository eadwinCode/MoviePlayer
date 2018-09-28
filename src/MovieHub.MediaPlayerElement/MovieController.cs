using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MovieHub.MediaPlayerElement.CustomControls;
using System.Windows.Threading;
using Common.Util;
using MovieHub.MediaPlayerElement.Interfaces;
using System.Windows.Controls.Primitives;
using Movies.Models.Interfaces;

namespace MovieHub.MediaPlayerElement
{
    public enum MediaControlViewType
    {
        MiniView,
        LargeView,
        SmallView
    };


    [TemplatePart(Name = "Part_VolumeControl", Type = typeof(VolumeControl))]
    [TemplatePart(Name = "Part_MediaSlider", Type = typeof(MediaSlider))]
    [TemplatePart(Name = "Part_MovieBoard", Type = typeof(MovieTitleBar))]
    [TemplatePart(Name = "Part_ParentRoot", Type = typeof(FrameworkElement))]
    public sealed class MovieControl : Control
    {
        private DispatcherTimer PositionSlideTimerTooltip;
        IMediaPlayerService MediaPlayerService;
        IMediaPlayabeLastSeen PlayableLastSeen;
        private string movieBoardInfo = "- MovieBoard Text -";
        private VolumeControl _volumeControl;

        private MovieTitleBar _movieBoard;
        private FrameworkElement _border;
        private MediaSlider _mediaslider;
        private MediaControlViewType controlviewtype = MediaControlViewType.LargeView;

        public MediaControlViewType ControlViewType
        {
            get { return controlviewtype; }
            set
            {
                controlviewtype = value;
                SetUseLargeControlView(this, value);
            }
        }

        internal VolumeControl VolumeControl
        {
            get { return _volumeControl; }
        }

        internal MovieTitleBar MovieBoard
        {
            get { return _movieBoard; }
        }

        internal MediaSlider MediaSlider
        {
            get { return _mediaslider; }
        }

        internal FrameworkElement RootElement
        {
            get { return _border; }
        }

        #region Commands
        private static RoutedCommand _playorpause = new RoutedCommand("PausePlay", typeof(MovieControl));
        private static RoutedCommand next = new RoutedCommand("Next", typeof(MovieControl));
        private static RoutedCommand previous = new RoutedCommand("Previous", typeof(MovieControl));
        private static RoutedCommand stop = new RoutedCommand("Stop", typeof(MovieControl));
        private static RoutedCommand volup = new RoutedCommand("VolUp", typeof(MovieControl));
        private static RoutedCommand voldown = new RoutedCommand("VolDown", typeof(MovieControl));
        private static RoutedCommand mute = new RoutedCommand("Mute", typeof(MovieControl));
        private static RoutedCommand fullscreen = new RoutedCommand("FullScreen", typeof(MovieControl));
        private static RoutedCommand togglemediaoptions = new RoutedCommand("ToggleMediaOption", typeof(MovieControl));
        private static RoutedCommand togglemediamenu = new RoutedCommand("ToggleMediaMenu", typeof(MovieControl));
        private static RoutedCommand topmost = new RoutedCommand("TopMost", typeof(MovieControl));
        private static RoutedCommand minimizecontrolcommand = new RoutedCommand("MinimizeControlCommand", typeof(MovieControl));

        private static RoutedCommand fforward = new RoutedCommand("FastForward", typeof(MovieControl));
        private static RoutedCommand rewind = new RoutedCommand("Rewind", typeof(MovieControl));
        private static RoutedCommand shiftforward = new RoutedCommand("ResizeMediaAlways", typeof(MovieControl));
        private static RoutedCommand shiftrewind = new RoutedCommand("ShiftRewind", typeof(MovieControl));
        private static RoutedCommand resizemediaalways = new RoutedCommand("ResizeMediaAlways",typeof(MovieControl));
        private static RoutedCommand repeatcommand = new RoutedCommand("RepeatCommand", typeof(MovieControl));
        private static RoutedCommand showmenucommand = new RoutedCommand("ShowMenuCommand", typeof(MovieControl));
        private static RoutedCommand closeLastSeen = new RoutedCommand("CloseLastSeenCommand", typeof(MovieControl));
        private static RoutedCommand closemediawindow = new RoutedCommand("CloseMediaWindow", typeof(MovieControl));
        private static RoutedCommand setlastseencommand = new RoutedCommand("SetLastSeenCommand", typeof(MovieControl));
        private static RoutedCommand controlviewchangecommand = new RoutedCommand("ControlViewChangeCommand", typeof(MovieControl));

        public static RoutedCommand ResizeMediaAlways 
        {
            get { return resizemediaalways; }
        }
        
        public static RoutedCommand ControlViewChangeCommand
        {
            get { return controlviewchangecommand; }
        }
        public static RoutedCommand CloseMediaWindow
        {
            get { return closemediawindow; }
        }
        public static RoutedCommand CloseLastSeenCommand
        {
            get { return closeLastSeen; }
        }

        public static RoutedCommand SetLastSeenCommand
        {
            get { return setlastseencommand; }
        }

        public static RoutedCommand ToggleMediaMenu
        {
            get { return togglemediamenu; }
        }

        public static RoutedCommand ToggleMediaOption
        {
            get { return togglemediaoptions; }
        }

        public static RoutedCommand TopMost
        {
            get { return topmost; }
        }

        public static RoutedCommand PausePlay
        {
            get { return _playorpause; }
        }
        public static RoutedCommand Rewind
        {
            get { return rewind; }
        }
        public static RoutedCommand ShiftRewind
        {
            get { return shiftrewind; }
        }

        public static RoutedCommand FastForward
        {
            get { return fforward; }
        }
        public static RoutedCommand ShiftFastForward
        {
            get { return shiftforward; }
        }

        public static RoutedCommand FullScreen
        {
            get { return fullscreen; }
        }

        public static RoutedCommand Stop
        {
            get { return stop; }
        }
        public static RoutedCommand Next
        {
            get { return next; }
        }
        public static RoutedCommand Previous
        {
            get { return previous; }
        }

        public static RoutedCommand VolUp
        {
            get { return volup; }
        }
        public static RoutedCommand VolDown
        {
            get { return voldown; }
        }
        public static RoutedCommand Mute 
        {
            get { return mute; }
        }

        public static RoutedCommand RepeatCommand
        {
            get { return repeatcommand; }
        }

        public static RoutedCommand ShowMenuCommand
        {
            get { return showmenucommand; }
        }

        public static RoutedCommand MinimizeControlCommand
        {
            get { return minimizecontrolcommand; }
        }
        #endregion

        static MovieControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MovieControl),
                new FrameworkPropertyMetadata(typeof(MovieControl)));

            RegisterInputGestures();

            RegisterDefaultCommands();
        }

        private static void RegisterInputGestures()
        {
            PausePlay.InputGestures.AddRange(new List<InputGesture>()
            { new KeyGesture(Key.Space), new KeyGesture(Key.MediaPlayPause) });
            Stop.InputGestures.Add(new KeyGesture(Key.MediaStop));
            Next.InputGestures.AddRange(new List<InputGesture>()
            { new KeyGesture(Key.N, ModifierKeys.Control), new KeyGesture(Key.MediaNextTrack) });
            Previous.InputGestures.AddRange(new List<InputGesture>()
            { new KeyGesture(Key.P, ModifierKeys.Control), new KeyGesture(Key.MediaPreviousTrack) });
            // FileExplorer.InputGestures.Add(new KeyGesture(Key.F,ModifierKeys.Control));
            VolUp.InputGestures.Add(new KeyGesture(Key.Up, ModifierKeys.Control));
            VolDown.InputGestures.Add(new KeyGesture(Key.Down, ModifierKeys.Control));
            Mute.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control));
            FullScreen.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));

            Rewind.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Control));
            FastForward.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Control));
            ShiftRewind.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Shift));
            ShiftFastForward.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Shift));
        }
        
        private static void RegisterDefaultCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(MovieControl), new CommandBinding(CloseLastSeenCommand, CloseLastSeenCommand_Executed));
            CommandManager.RegisterClassCommandBinding(typeof(MovieControl), new CommandBinding(ShowMenuCommand, ShowMenuCommand_Executed));
            CommandManager.RegisterClassCommandBinding(typeof(MovieControl), new CommandBinding(SetLastSeenCommand, SetLastSeenCommand_Executed));
        }

        private static void SetLastSeenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl movieControl = sender as MovieControl;
            if (movieControl != null)
                movieControl.SetLastSeenAction();
        }
        
        private static void ShowMenuCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl movieControl = sender as MovieControl;
            if (movieControl != null)
            {
                Popup popup = e.Parameter as Popup;
                popup.PlacementTarget = (e.OriginalSource as Button);
                popup.IsOpen = true;
            }
        }

        private static void CloseLastSeenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl movieControl = sender as MovieControl;
            if (movieControl != null)
                movieControl.CloseLastSeenBoardAction(true);
        }

        #region Property Dependencies



        public bool IsMinimizeControlButtonEnabled
        {
            get { return (bool)GetValue(IsMinimizeControlButtonEnabledProperty); }
            set { SetValue(IsMinimizeControlButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMinimizeControlButtonEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMinimizeControlButtonEnabledProperty =
            DependencyProperty.Register("IsMinimizeControlButtonEnabled", typeof(bool), typeof(MovieControl), new PropertyMetadata(false));



        public bool IsControlMediaCloseButtonEnabled
        {
            get { return (bool)GetValue(IsControlMediaCloseButtonEnabledProperty); }
            set { SetValue(IsControlMediaCloseButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsControlMediaCloseButtonEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsControlMediaCloseButtonEnabledProperty =
            DependencyProperty.Register("IsControlMediaCloseButtonEnabled", typeof(bool), typeof(MovieControl), new PropertyMetadata(false));
        

        public bool ShowTimerSeperator
        {
            get { return (bool)GetValue(HideTimerSeperatorProperty); }
            private set { SetValue(HideTimerSeperatorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HideTimerSeperator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HideTimerSeperatorProperty =
            DependencyProperty.Register("ShowTimerSeperator", typeof(bool), typeof(MovieControl), new PropertyMetadata(true));



        public bool MediaDurationDisplayVisible
        {
            get { return (bool)GetValue(MediaDurationDisplayVisibleProperty); }
            set { SetValue(MediaDurationDisplayVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaDurationDisplayVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaDurationDisplayVisibleProperty =
            DependencyProperty.Register("MediaDurationDisplayVisible", typeof(bool), typeof(MovieControl), new PropertyMetadata(true, OnMediaDurationDisplayVisibleChanged));

        private static void OnMediaDurationDisplayVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MovieControl movieControl = d as MovieControl;
            if(movieControl != null)
            {
                movieControl.ShowTimerSeperator = movieControl.CurrentTimerDisplayVisible && movieControl.MediaDurationDisplayVisible ? true : false;
            }
        }

        public bool CurrentTimerDisplayVisible
        {
            get { return (bool)GetValue(CurrentTimerDisplayVisibleProperty); }
            set { SetValue(CurrentTimerDisplayVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTimerDisplayVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimerDisplayVisibleProperty =
            DependencyProperty.Register("CurrentTimerDisplayVisible", typeof(bool), typeof(MovieControl), new PropertyMetadata(true, OnMediaDurationDisplayVisibleChanged));
        


        public bool DisableMovieBoardText
        {
            get { return (bool)GetValue(DisableMovieBoardTextProperty); }
            set { SetValue(DisableMovieBoardTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisableMovieBoardText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisableMovieBoardTextProperty =
            DependencyProperty.Register("DisableMovieBoardText", typeof(bool), typeof(MovieControl), new PropertyMetadata(true, OnDisableMovieBoardTextChanged));

        private static void OnDisableMovieBoardTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MovieControl movieControl = d as MovieControl;
            if (movieControl != null)
            {
                movieControl.IntiSliderPosition((bool)e.NewValue);
            }
        }

        public TimeSpan LastSeenTextInfo
        {
            get { return (TimeSpan)GetValue(LastSeenTextInfoProperty); }
            set { SetValue(LastSeenTextInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastSeenTextInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastSeenTextInfoProperty =
            DependencyProperty.Register("LastSeenTextInfo", typeof(TimeSpan), typeof(MovieControl), new PropertyMetadata(null));



        public bool IsPreviousButtonEnabled
        {
            get { return (bool)GetValue(IsPreviousButtonEnabledProperty); }
            set { SetValue(IsPreviousButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPreviousButtonEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPreviousButtonEnabledProperty =
            DependencyProperty.Register("IsPreviousButtonEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(false));
        

        public double MediaDuration
        {
            get { return (double)GetValue(MediaDurationProperty); }
            set { SetValue(MediaDurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaDurationProperty =
            DependencyProperty.Register("MediaDuration", typeof(double), typeof(MovieControl), 
                new FrameworkPropertyMetadata(0.00000000001));



        public double CurrentMediaTime
        {
            get { return (double)GetValue(CurrentMediaTimeProperty); }
            set { SetValue(CurrentMediaTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentMediaTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentMediaTimeProperty =
            DependencyProperty.Register("CurrentMediaTime", typeof(double), typeof(MovieControl), new PropertyMetadata(0.0));
        
        public bool IsLastSeenEnabled
        {
            get { return (bool)GetValue(IsLastSeenEnabledProperty); }
            set { SetValue(IsLastSeenEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLastSeenEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLastSeenEnabledProperty =
            DependencyProperty.Register("IsLastSeenEnabled", typeof(bool), typeof(MovieControl),
                new FrameworkPropertyMetadata(false));



        public bool IsMediaSliderEnabled
        {
            get { return (bool)GetValue(IsMediaSliderEnabledProperty); }
            set { SetValue(IsMediaSliderEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaSliderEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMediaSliderEnabledProperty =
            DependencyProperty.Register("IsMediaSliderEnabled", typeof(bool), typeof(MovieControl), new PropertyMetadata(true));



        public RepeatMode RepeatMode
        {
            get { return (RepeatMode)GetValue(RepeatModeProperty); }
            set { SetValue(RepeatModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RepeatMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RepeatModeProperty =
            DependencyProperty.Register("RepeatMode", typeof(RepeatMode), typeof(MovieControl), new PropertyMetadata(RepeatMode.NoRepeat));



        public bool IsVolumeControlEnabled
        {
            get { return (bool)GetValue(IsVolumeControlEnabledProperty); }
            set { SetValue(IsVolumeControlEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolumeControlEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVolumeControlEnabledProperty =
            DependencyProperty.Register("IsVolumeControlEnabled", typeof(bool), typeof(MovieControl), new PropertyMetadata(true));



        public bool IsNextButtonEnabled
        {
            get { return (bool)GetValue(IsNextButtonEnabledProperty); }
            set { SetValue(IsNextButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsNextButtonEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNextButtonEnabledProperty =
            DependencyProperty.Register("IsNextButtonEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(false));



        public bool IsMediaOptionToggleEnabled
        {
            get { return (bool)GetValue(IsMediaOptionToggleEnabledProperty); }
            set { SetValue(IsMediaOptionToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMediaOptionToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMediaOptionToggleEnabledProperty =
            DependencyProperty.Register("IsMediaOptionToggleEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(true));



        public bool IsPlaylistToggleEnabled
        {
            get { return (bool)GetValue(IsPlaylistToggleEnabledProperty); }
            set { SetValue(IsPlaylistToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlaylistToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlaylistToggleEnabledProperty =
            DependencyProperty.Register("IsPlaylistToggleEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(false));



        public bool IsRepeatToggleEnabled
        {
            get { return (bool)GetValue(IsRepeatToggleEnabledProperty); }
            set { SetValue(IsRepeatToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRepeatToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRepeatToggleEnabledProperty =
            DependencyProperty.Register("IsRepeatToggleEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(false));



        public bool IsFullScreenToggleEnabled
        {
            get { return (bool)GetValue(IsFullScreenToggleEnabledProperty); }
            set { SetValue(IsFullScreenToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFullScreenToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFullScreenToggleEnabledProperty =
            DependencyProperty.Register("IsFullScreenToggleEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(true));



        public bool IsShowMenuToggleEnabled
        {
            get { return (bool)GetValue(IsShowMenuToggleEnabledProperty); }
            set { SetValue(IsShowMenuToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowMenuToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowMenuToggleEnabledProperty =
            DependencyProperty.Register("IsShowMenuToggleEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(true));
        

        public static readonly DependencyProperty UseLargeControlViewProperty =
            DependencyProperty.RegisterAttached("UseLargeControlView",
       typeof(MediaControlViewType), typeof(MovieControl), new FrameworkPropertyMetadata {
           DefaultValue = MediaControlViewType.SmallView});
        

        public static void SetUseLargeControlView(UIElement element, MediaControlViewType value)
        {
            element.SetValue(UseLargeControlViewProperty, value);
        }
        public static MediaControlViewType GetUseLargeControlView(UIElement element)
        {
            return (MediaControlViewType)element.GetValue(UseLargeControlViewProperty);
        }


        public static readonly DependencyProperty CanAnimateControlProperty = 
            DependencyProperty.RegisterAttached("CanAnimateControl", typeof(bool), typeof(MovieControl),
          new FrameworkPropertyMetadata { DefaultValue = false });

        public static bool GetCanAnimateControl(UIElement obj)
        {
            return (bool)obj.GetValue(CanAnimateControlProperty);
        }

        public static void SetCanAnimateControl(UIElement obj, bool value)
        {
            obj.SetValue(CanAnimateControlProperty, value);
        }
        

        public static readonly DependencyProperty IsMouseOverMediaElementProperty = 
            DependencyProperty.RegisterAttached("IsMouseOverMediaElement", typeof(bool?), typeof(MovieControl), 
                new FrameworkPropertyMetadata(null));


        public static void SetIsMouseOverMediaElement(UIElement element, bool? value)
        {
            element.SetValue(IsMouseOverMediaElementProperty, value);
        }

        public static bool? GetIsMouseOverMediaElement(UIElement element)
        {
            return (bool?)element.GetValue(IsMouseOverMediaElementProperty);
        }
        #endregion
        
        internal MovieControl(IMediaPlayerService mediaPlayerService)
        {
            this.MediaPlayerService = mediaPlayerService;
            this.ControlViewType = MediaControlViewType.SmallView;
            DisableMovieBoardText = false;
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _volumeControl = (VolumeControl)this.GetTemplateChild("Part_VolumeControl");
            _mediaslider = (MediaSlider)this.GetTemplateChild("Part_MediaSlider");
            _movieBoard = (MovieTitleBar)this.GetTemplateChild("Part_MovieBoard");
            _border = (FrameworkElement)this.GetTemplateChild("Part_ParentRoot");

            _movieBoard.MovieTitleText = movieBoardInfo;
            _volumeControl.VolumeLevel = MediaPlayerService.Volume;
            HookUpEvents();
        }
        
        private void IntiSliderPosition(bool IsDisplayBoardEnabled)
        {
            PositionSlideTimerTooltip = new DispatcherTimer(DispatcherPriority.Background);

            if (!IsDisplayBoardEnabled)
            {
                PositionSlideTimerTooltip.Interval = TimeSpan.FromMilliseconds(100);
                PositionSlideTimerTooltip.Tick += (s, e) =>
                {
                    if (MediaSlider == null)
                    {
                        return;
                    }
                    var x = Mouse.GetPosition(MediaSlider).X;
                    var ratio = x / MediaSlider.ActualWidth;
                    UpdateText(x, ratio, TimeSpan.FromSeconds(Math.Round((ratio * MediaSlider.Maximum), 10)).
                        ToString(@"hh\:mm\:ss"));
                };
            }
        }

        private void HookUpEvents()
        {
            MediaSlider.MouseEnter += MediaSlider_MouseEnter;
            MediaSlider.MouseLeave += MediaSlider_MouseLeave;
            MediaSlider.MouseUp += MediaSlider_MouseUp;
            MediaSlider.MouseMove += MediaSlider_MouseMove;
            MediaSlider.PreviewMouseMove += MediaSlider_MouseMove;
            MediaSlider.MouseDown += MediaSlider_MouseDown;
            MediaSlider.PreviewMouseDown += MediaSlider_MouseDown;
            VolumeControl.VolumeControlValueChanged += VolumeControl_VolumeControlValueChanged;
        }

        private void VolumeControl_VolumeControlValueChanged(object sender, 
            RoutedPropertyChangedEventArgs<double> e)
        {
            SetMediaVolume(e.NewValue);
        }

        private void MediaSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Slider CurrentSlider = sender as Slider;
            CurrentSlider.Cursor = Cursors.Hand;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var currentpos = CommonHelper.GetMousePointer(CurrentSlider);
                CurrentSlider.Value = Math.Round((currentpos * CurrentSlider.Maximum), 10);
                DragCompleted();
            }
        }

        private void MediaSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsMediaSliderEnabled)
                return;

            PositionSlideTimerTooltip.Start();
        }

        private void MediaSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsMediaSliderEnabled)
                return;

            PositionSlideTimerTooltip.Stop();
            Slider slider = sender as Slider;
            slider.Cursor = Cursors.Arrow;
            MovieBoard.SecondMovieBarText = null;
        }

        private void MediaSlider_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsMediaSliderEnabled)
                return;


            Slider slider = sender as Slider;

            if (MediaPlayerService.State == MovieMediaState.Playing ||
                MediaPlayerService.State == MovieMediaState.Paused)
            {
                slider.Cursor = Cursors.Hand;
                PositionSlideTimerTooltip.Start();
            }
        }

        private void MediaSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsMediaSliderEnabled)
                return;

            Slider slider = sender as Slider;

            if (MediaPlayerService.State == MovieMediaState.Playing ||
               MediaPlayerService.State == MovieMediaState.Paused)
            {
                slider.Cursor = Cursors.Hand;
            }
        }

        private void DragCompleted()
        {
            MediaPlayerService.CurrentTimer = TimeSpan.FromSeconds(MediaSlider.Value);
        }

        private void UpdateText(double x, double ratio, string p)
        {
            if (DisableMovieBoardText)
                return;

            MovieBoard.SecondMovieBarText = "Seeking/ " + p;
        }

        private void CloseLastSeenBoardAction(bool clear = false)
        {
            LastSeenTextInfo = TimeSpan.Zero;
            IsLastSeenEnabled = false;
            if (clear)
                PlayableLastSeen.LastPlayedPoisition.ProgressLastSeen = 0;
        }

        private void SetLastSeenAction()
        {
            MediaPlayerService.CurrentTimer += LastSeenTextInfo;
            CloseLastSeenBoardAction();
        }

        internal void LoadPlaylistControls()
        {
            IsNextButtonEnabled = true;
            IsPreviousButtonEnabled = true;
            IsPlaylistToggleEnabled = true;
            IsRepeatToggleEnabled = true;
        }

        internal void UnloadedPlaylistControls()
        {
            IsNextButtonEnabled = false;
            IsPreviousButtonEnabled = false;
            IsPlaylistToggleEnabled = false;
            IsRepeatToggleEnabled = false;
        }

        internal void MediaPlayerElementNotifyLastSeen(IMediaPlayabeLastSeen playablelastseen)
        {
            if (playablelastseen.Progress > 0)
            {
                var timeSpan = TimeSpan.FromSeconds((double.Parse(MediaPlayerService.Duration.TotalSeconds.ToString()) * playablelastseen.LastPlayedPoisition.ProgressLastSeen) / 100);
                LastSeenTextInfo = timeSpan;
                IsLastSeenEnabled = true;
                this.PlayableLastSeen = playablelastseen;
            }
        }

        public void SetMediaVolume(double vol)
        {
            if (MediaPlayerService == null)
                return;
            this.MediaPlayerService.Volume = (int)vol;
        }

        public void SetMovieTitleBoard(string info)
        {
            MovieBoard.MovieTitleText = movieBoardInfo = info;
        }

        public void NotifyLastSeen(IMediaPlayabeLastSeen playablelastseen)
        {
            if (!IsLastSeenEnabled)
                throw new Exception("LastSeen Control is disabled");

            if (playablelastseen.Progress > 0)
            {
                var timeSpan = TimeSpan.FromSeconds((double.Parse(MediaPlayerService.Duration.TotalSeconds.ToString()) * playablelastseen.LastPlayedPoisition.ProgressLastSeen) / 100);
                LastSeenTextInfo = timeSpan;
                IsLastSeenEnabled = true;
                this.PlayableLastSeen = playablelastseen;
            }
        }

        public void CloseLastSeenBoard()
        {
            CloseLastSeenBoardAction();
        }

    }
}
