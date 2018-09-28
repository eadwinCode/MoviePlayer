using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VideoPlayerControl.CustomControls;

namespace VideoPlayerControl
{
    [TemplatePart(Name = "Part_VolumeControl", Type = typeof(VolumeControl))]
    [TemplatePart(Name = "Part_MediaSlider", Type = typeof(MediaSlider))]
    [TemplatePart(Name = "Part_MovieBoard", Type = typeof(MovieTitleBar))]
    [TemplatePart(Name = "Part_ParentRoot", Type = typeof(FrameworkElement))]
    public class MovieControl : Control
    {
        #region Commands
        private static RoutedCommand _playorpause = new RoutedCommand("PausePlay", typeof(MovieControl));
        private static RoutedCommand next = new RoutedCommand("Next", typeof(MovieControl));
        private static RoutedCommand previous = new RoutedCommand("Previous", typeof(MovieControl));
        private static RoutedCommand stop = new RoutedCommand("Stop", typeof(MovieControl));
        private static RoutedCommand volup = new RoutedCommand("VolUp", typeof(MovieControl));
        private static RoutedCommand voldown = new RoutedCommand("VolDown", typeof(MovieControl));
        private static RoutedCommand mute = new RoutedCommand("Mute", typeof(MovieControl));
        private static RoutedCommand fullscreen = new RoutedCommand("FullScreen", typeof(MovieControl));
        private static RoutedCommand togglemediaoptions = new RoutedCommand("ToggleMediaMenu", typeof(MovieControl));
        private static RoutedCommand topmost = new RoutedCommand("TopMost", typeof(MovieControl));

        private static RoutedCommand fforward = new RoutedCommand("FastForward", typeof(MovieControl));
        private static RoutedCommand rewind = new RoutedCommand("Rewind", typeof(MovieControl));
        private static RoutedCommand shiftforward = new RoutedCommand("ResizeMediaAlways", typeof(MovieControl));
        private static RoutedCommand shiftrewind = new RoutedCommand("ShiftRewind", typeof(MovieControl));
        private static RoutedCommand resizemediaalways = new RoutedCommand("ResizeMediaAlways",typeof(MovieControl));
        public static RoutedCommand repeatcommand = new RoutedCommand("RepeatCommand", typeof(MovieControl));
        public static RoutedCommand showmenucommand = new RoutedCommand("ShowMenuCommand", typeof(MovieControl));
        public static RoutedCommand closeLastSeen = new RoutedCommand("CloseLastSeenCommand", typeof(MovieControl));
        public static RoutedCommand setlastseencommand = new RoutedCommand("SetLastSeenCommand", typeof(MovieControl));

        public static RoutedCommand ResizeMediaAlways
        {
            get { return resizemediaalways; }
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
        #endregion

        static MovieControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MovieControl),
                new FrameworkPropertyMetadata(typeof(MovieControl)));

            RegisterInputGestures();
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

        #region Property Dependencies

        public bool IsPreviousButtonEnabled
        {
            get { return (bool)GetValue(IsPreviousButtonEnabledProperty); }
            set { SetValue(IsPreviousButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPreviousButtonEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPreviousButtonEnabledProperty =
            DependencyProperty.Register("IsPreviousButtonEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(true));
        

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
            DependencyProperty.Register("IsNextButtonEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(true));



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
            DependencyProperty.Register("IsPlaylistToggleEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(true));



        public bool IsRepeatToggleEnabled
        {
            get { return (bool)GetValue(IsRepeatToggleEnabledProperty); }
            set { SetValue(IsRepeatToggleEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRepeatToggleEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRepeatToggleEnabledProperty =
            DependencyProperty.Register("IsRepeatToggleEnabled", typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata(true));



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
       typeof(bool), typeof(MovieControl), new FrameworkPropertyMetadata { DefaultValue = false});



        public static void SetUseLargeControlView(UIElement element, bool value)
        {
            element.SetValue(UseLargeControlViewProperty, value);
        }
        public static bool GetUseLargeControlView(UIElement element)
        {
            return (bool)element.GetValue(UseLargeControlViewProperty);
        }

        #endregion

        private MediaControlViewType controlviewtype = MediaControlViewType.LargeView;

        public MediaControlViewType ControlViewType
        {
            get { return controlviewtype; }
            set { controlviewtype = value;
                SetUseLargeControlView(this,value == MediaControlViewType.LargeView ? true:false);
            }
        }
        
        private VolumeControl _volumeControl;

        internal VolumeControl VolumeControl
        {
            get { return _volumeControl; }
        }

        private MovieTitleBar _movieBoard;

        internal MovieTitleBar MovieBoard
        {
            get { return _movieBoard; }
        }

        private MediaSlider _mediaslider;

        internal MediaSlider MediaSlider
        {
            get { return _mediaslider; }
        }

        private FrameworkElement _border;

        internal FrameworkElement RootElement
        {
            get { return _border; }
        }

        internal MovieControl()
        {
            this.ControlViewType = MediaControlViewType.LargeView;
            CommandManager.RegisterClassCommandBinding(typeof(MediaController),
                new CommandBinding(CloseLastSeenCommand, CloseLastSeenCommand_Executed));
        }

        private void CloseLastSeenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.IsLastSeenEnabled = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _volumeControl = (VolumeControl)this.GetTemplateChild("Part_VolumeControl");
            _mediaslider = (MediaSlider)this.GetTemplateChild("Part_MediaSlider");
            _movieBoard = (MovieTitleBar)this.GetTemplateChild("Part_MovieBoard");
            _border = (FrameworkElement)this.GetTemplateChild("Part_ParentRoot");
        }


        
    }
}
