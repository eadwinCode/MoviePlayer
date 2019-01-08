using Common.ApplicationCommands;
using Common.Util;
using MovieHub.MediaPlayerElement.CustomControls;
using MovieHub.MediaPlayerElement.Models;
using Movies.Enums;
using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace MovieHub.MediaPlayerElement
{
    public enum MediaControlViewType
    {
        MiniView,
        LargeView,
        SmallView, HomeView
    };


    [TemplatePart(Name = "Part_VolumeControl", Type = typeof(VolumeControl))]
    [TemplatePart(Name = "Part_MediaSlider", Type = typeof(MediaSlider))]
    [TemplatePart(Name = "Part_MovieBoard", Type = typeof(MovieTitleBar))]
    [TemplatePart(Name = "Part_ParentRoot", Type = typeof(FrameworkElement))]
    public sealed class MovieControl : Control, IMovieControl
    {
        private DispatcherTimer PositionSlideTimerTooltip;
        Popup _popup = null;
        IMediaPlayabeLastSeen PlayableLastSeen;
        private VolumeControl _volumeControl;
        private MovieTitleBar _movieBoard;
        private FrameworkElement _border;
        private MediaSlider _mediaslider;
        private MediaControlViewType controlviewtype = MediaControlViewType.LargeView;
        static MovieControl _currentInstance;

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

            RegisterDefaultCommands();
            RegisterInputGestures();
        }

        private static void RegisterInputGestures()
        {
            PausePlay.InputGestures.AddRange(new List<InputGesture>()
            { new KeyGesture(Key.MediaPlayPause), new KeyGesture(Key.Space) });
            Stop.InputGestures.Add(new KeyGesture(Key.MediaStop));
            Next.InputGestures.AddRange(new List<InputGesture>()
            { new KeyGesture(Key.MediaNextTrack) , new KeyGesture(Key.N, ModifierKeys.Control),});
            Previous.InputGestures.AddRange(new List<InputGesture>()
            {  new KeyGesture(Key.MediaPreviousTrack),new KeyGesture(Key.P, ModifierKeys.Control), });
            // FileExplorer.InputGestures.Add(new KeyGesture(Key.F,ModifierKeys.Control));
            VolUp.InputGestures.Add(new KeyGesture(Key.Up, ModifierKeys.Control));
            VolDown.InputGestures.Add(new KeyGesture(Key.Down, ModifierKeys.Control));
            Mute.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control));
            FullScreen.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));

            Rewind.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Control));
            FastForward.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Control));
            ShiftRewind.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Shift));
            ShiftFastForward.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Shift));
            VideoPlayerCommands.PlayList.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Alt));
        }
        #region Command Registration

        internal static void RegisterCommandBings(Type type, ICommand command, CommandBinding commandBinding, params InputGesture[] inputBinding)
        {
            CommandManager.RegisterClassCommandBinding(type, commandBinding);
            if (inputBinding.Length > 0)
            {
                foreach (var ipbing in inputBinding)
                {
                    CommandManager.RegisterClassInputBinding(type, new InputBinding(command, ipbing));
                }
            }

        }

        private static void RegisterDefaultCommands()
        {
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.CloseLastSeenCommand, new CommandBinding(CloseLastSeenCommand, CloseLastSeenCommand_Executed));
            RegisterCommandBings(typeof(MovieControl), MovieControl.ShowMenuCommand, new CommandBinding(ShowMenuCommand, ShowMenuCommand_Executed));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.SetLastSeenCommand, new CommandBinding(SetLastSeenCommand, SetLastSeenCommand_Executed));

           
            RegisterCommandBings(typeof(MovieControl), MovieControl.RepeatCommand, new CommandBinding(MovieControl.RepeatCommand, RepeatCommand_Executed, RepeatCommand_Enabled));
            RegisterCommandBings(typeof(MovieControl), MovieControl.ToggleMediaMenu, new CommandBinding(MovieControl.ToggleMediaMenu, ToggleMediaMenu_Executed, ToggleMediaMenu_Enabled));
            RegisterCommandBings(typeof(MovieControl), MovieControl.ToggleMediaOption, new CommandBinding(MovieControl.ToggleMediaOption, ToggleMediaOption_Executed, ToggleMediaOption_Enabled));
            RegisterCommandBings(typeof(MovieControl), MovieControl.ResizeMediaAlways, new CommandBinding(MovieControl.ResizeMediaAlways, ResizeMediaAlways_Executed));
            RegisterCommandBings(typeof(MovieControl), MovieControl.TopMost, new CommandBinding(MovieControl.TopMost, TopMostCommand_Executed));
            RegisterCommandBings(typeof(MovieControl), MovieControl.CloseMediaWindow, new CommandBinding(MovieControl.CloseMediaWindow, CloseMediaWindow_Executed));

          
            RegisterCommandBings(typeof(MovieControl), MovieControl.ControlViewChangeCommand, new CommandBinding(MovieControl.ControlViewChangeCommand, ControlViewChangeCommand_Executed));
            RegisterCommandBings(typeof(MovieControl), MovieControl.MinimizeControlCommand, new CommandBinding(MovieControl.MinimizeControlCommand, MinimizeControlCommand_Executed, MinimizeControlCommand_Enabled));

            RegisterCommandBings(typeof(MovieControl), MovieControl.PausePlay, new CommandBinding(MovieControl.PausePlay, PauseOrPlay_Executed, PauseOrPlay_Enabled));
            RegisterCommandBings(typeof(Window), VideoPlayerCommands.PlayList, new CommandBinding(VideoPlayerCommands.PlayList, PlayListToggle_Executed, Playlist_Enabled));
            RegisterCommandBings(typeof(Window), MovieControl.Mute, new CommandBinding(MovieControl.Mute, Mute_executed));
            RegisterCommandBings(typeof(Window), MovieControl.Rewind, new CommandBinding(MovieControl.Rewind, Rewind_executed, Rewind_enabled));
            RegisterCommandBings(typeof(Window), MovieControl.ShiftRewind, new CommandBinding(MovieControl.ShiftRewind, ShiftRewind_executed, Rewind_enabled));

            RegisterCommandBings(typeof(Window), MovieControl.FastForward, new CommandBinding(MovieControl.FastForward, FastForward_executed, Rewind_enabled));
            RegisterCommandBings(typeof(Window), MovieControl.ShiftFastForward, new CommandBinding(MovieControl.ShiftFastForward, ShiftFastForward_executed, Rewind_enabled));


            RegisterCommandBings(typeof(Window), MovieControl.VolUp, new CommandBinding(MovieControl.VolUp, VolUp_Executed, Volume_Enabled));
            RegisterCommandBings(typeof(Window), MovieControl.VolDown, new CommandBinding(MovieControl.VolDown, VolDown_Executed, Volume_Enabled));
            RegisterCommandBings(typeof(Window), MovieControl.FullScreen, new CommandBinding(MovieControl.FullScreen, FullScreen_Executed, FullScreen_Enabled));
            RegisterCommandBings(typeof(MovieControl), MovieControl.Next, new CommandBinding(MovieControl.Next, Next_Executed, Next_Enabled));
            RegisterCommandBings(typeof(MovieControl), MovieControl.Previous, new CommandBinding(MovieControl.Previous, Previous_Executed, Previous_Enabled));
        }

        private static void PlayListToggle_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = e.Parameter as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                mediaplayerelement.TogglePlaylistView();
            }
        }

        private static void Playlist_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = e.Parameter as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                e.CanExecute = mediaplayerelement.PlaylistManager != null && mediaplayerelement.MediaPlayerViewType == MediaPlayerViewType.FullMediaPanel;
            }

        }

        private static void PauseOrPlay_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = e.Parameter as MediaPlayerElement;
            if (mediaplayerelement != null)
                e.CanExecute = mediaplayerelement.CurrentStreamingitem != null || !mediaplayerelement.MediaPlayerServices.HasLoadedMedia;
            else
                e.CanExecute = false;
        }

        private static void PauseOrPlay_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = e.Parameter as MediaPlayerElement;
            if (mediaplayerelement != null)
                mediaplayerelement.PauseOrPlayAction();
        }

        private static void ShiftFastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            //MediaPlayerElement mediaplayerelement = sender as MediaPlayerElement;
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
            {
                _currentInstance.MediaPlayerElement.ReWindFastForward();
                _currentInstance.MediaPlayerElement.MediaPlayerServices.CurrentTimer += TimeSpan.FromMilliseconds(1500);
            }
        }

        private static void Rewind_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
                e.CanExecute = _currentInstance.MediaPlayerElement.MediaPlayerServices.IsSeekable;
            else
                e.CanExecute = false;
        }

        private static void FastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
            {
                _currentInstance.MediaPlayerElement.ReWindFastForward();
                _currentInstance.MediaPlayerElement.MediaPlayerServices.CurrentTimer += TimeSpan.FromMilliseconds(10000);
            }
        }

        private static void ShiftRewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
            {
                _currentInstance.MediaPlayerElement.ReWindFastForward();
                _currentInstance.MediaPlayerElement.MediaPlayerServices.CurrentTimer -= TimeSpan.FromMilliseconds(1500);
            }
        }

        private static void Rewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
            {
                _currentInstance.MediaPlayerElement.ReWindFastForward();
                _currentInstance.MediaPlayerElement.MediaPlayerServices.CurrentTimer -= TimeSpan.FromMilliseconds(10000);
            }
        }

        private static void Mute_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
                _currentInstance.MediaPlayerElement.MuteAction();
        }

        private static void Previous_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = e.Parameter as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                e.CanExecute = mediaplayerelement.PlaylistManager == null ? false : mediaplayerelement.PlaylistManager.CanPrevious;
            }
        }

        private static void Previous_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = e.Parameter as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                mediaplayerelement.PreviousAction();
            }
        }

        private static void Next_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = e.Parameter as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                e.CanExecute = mediaplayerelement.PlaylistManager == null ? false : mediaplayerelement.PlaylistManager.CanNext;
            }
        }

        private static void Next_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = e.Parameter as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                mediaplayerelement.NextAction();
            }
        }

        private static void FullScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //MediaPlayerElement mediaplayerelement = e.Parameter as MediaPlayerElement;
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
            {
                _currentInstance.MediaPlayerElement.WindowsFullScreenAction();
                _currentInstance.MediaPlayerElement.MovieControl.CloseAnyPopup();
            }
        }

        private static void FullScreen_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //MediaPlayerElement mediaplayerelement = e.Parameter as MediaPlayerElement;
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
                e.CanExecute = _currentInstance.MovieControlSettings.IsFullScreenToggleEnabled;
            else
                e.CanExecute = false;
        }

        private static void Volume_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = sender as MediaPlayerElement;
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
                e.CanExecute = _currentInstance.MediaPlayerElement.VolumeState != VolumeState.Muted;
            else
                e.CanExecute = false;
        }

        private static void VolDown_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = sender as MediaPlayerElement;
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
            {
                _currentInstance.VolumeControl.VolumeLevel -= 10;
                _currentInstance.MediaPlayerElement.PreviewControl();
            }
        }

        private static void VolUp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = sender as MediaPlayerElement;
            if (_currentInstance != null && _currentInstance.MediaPlayerElement != null)
            {
                _currentInstance.VolumeControl.VolumeLevel += 10;
                _currentInstance.MediaPlayerElement.PreviewControl();
            }
        }


        private static void ToggleMediaOption_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null && e.Parameter != null)
            {
                moviecontrol._popup = e.Parameter as Popup;
                if (moviecontrol._popup != null)
                {
                    moviecontrol._popup.IsOpen = true;
                }
            }
        }

        private static void ToggleMediaOption_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private static void MinimizeControlCommand_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
                e.CanExecute = moviecontrol.MovieControlSettings.IsFullScreenToggleEnabled;
            else
                e.CanExecute = false;
        }

        private static void MinimizeControlCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
                moviecontrol.MediaPlayerElement.MinimizeControlAction();
        }

        private static void CloseMediaWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
            {
                moviecontrol.MediaPlayerElement.WindowsClosedAction();
                moviecontrol.CloseAnyPopup();
            }
        }

        private static void ControlViewChangeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
            {
                moviecontrol.MediaPlayerElement.ControlViewChangeCommandAction();
            }
        }

       

        private static void ResizeMediaAlways_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
            {
                moviecontrol.MediaPlayerElement._allowMediaSizeEventExecute = !moviecontrol.MediaPlayerElement._allowMediaSizeEventExecute;
            }
        }

        private static void TopMostCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
            {
                moviecontrol.MediaPlayerElement.MediaPlayerElementRaiseEvent(MediaPlayerElement.SetWindowTopMostPropertyEvent);
            }
        }

        

        private static void ToggleMediaMenu_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
                moviecontrol.MediaPlayerElement._mediaMenu.ShowDialog();
        }

        private static void ToggleMediaMenu_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
                e.CanExecute = !moviecontrol.MediaPlayerElement.MediaPlayerServices.HasStopped;
            else
                e.CanExecute = false;
        }

       
        private static void RepeatCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
                moviecontrol.MediaPlayerElement.RepeatModeAction(); ;
        }

        private static void RepeatCommand_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MovieControl moviecontrol = sender as MovieControl;
            if (moviecontrol != null && moviecontrol.MediaPlayerElement != null)
                e.CanExecute = moviecontrol.MediaPlayerElement.PlaylistManager != null;
            else
                e.CanExecute = false;
        }

       
        #endregion

        private static void SetLastSeenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                mediaPlayerElement.MovieControl.SetLastSeenAction();
        }
        
        private static void ShowMenuCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovieControl movieControl = sender as MovieControl;
            if (movieControl != null)
            {
                movieControl._popup = e.Parameter as Popup;
                if (movieControl._popup != null)
                {
                    movieControl._popup.PlacementTarget = (e.OriginalSource as Button);
                    movieControl._popup.IsOpen = true;
                }
            }
        }

        private static void CloseLastSeenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                mediaPlayerElement.MovieControl.CloseLastSeenBoardAction(true);
        }

        #region Property Dependencies



        public MediaPlayerElement MediaPlayerElement
        {
            get { return (MediaPlayerElement)GetValue(MediaPlayerElementProperty); }
            private set { SetValue(MediaPlayerElementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaPlayerElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaPlayerElementProperty =
            DependencyProperty.Register("MediaPlayerElement", typeof(MediaPlayerElement), typeof(MovieControl), new PropertyMetadata(null));



        public MediaDetailProps MediaDetailProps
        {
            get { return (MediaDetailProps)GetValue(MediaDetailPropsProperty); }
            internal set { SetValue(MediaDetailPropsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaDetailProps.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaDetailPropsProperty =
            DependencyProperty.Register("MediaDetailProps", typeof(MediaDetailProps), typeof(MovieControl), new PropertyMetadata(null));



        public MovieControlSettings MovieControlSettings
        {
            get { return (MovieControlSettings)GetValue(MovieControlSettingsProperty); }
            private set { SetValue(MovieControlSettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MovieControlSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MovieControlSettingsProperty =
            DependencyProperty.Register("MovieControlSettings", typeof(MovieControlSettings), typeof(MovieControl), new PropertyMetadata(null));



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
        

        public MovieControl(MediaPlayerElement mediaPlayerElement)
            :this()
        {
            this.MediaPlayerElement = mediaPlayerElement;
            _currentInstance = this;
        }

        public MovieControl()
        {
            this.ControlViewType = MediaControlViewType.SmallView;
            MovieControlSettings = new MovieControlSettings() { MovieControl = this };
            MediaDetailProps = new MediaDetailProps();
            MovieControlSettings.DisableMovieBoardText = false;
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _volumeControl = (VolumeControl)this.GetTemplateChild("Part_VolumeControl");
            _mediaslider = (MediaSlider)this.GetTemplateChild("Part_MediaSlider");
            _movieBoard = (MovieTitleBar)this.GetTemplateChild("Part_MovieBoard");
            _border = (FrameworkElement)this.GetTemplateChild("Part_ParentRoot");

            _movieBoard.MovieTitleText = MediaDetailProps.movieBoardInfo;

            HookUpEvents();
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            if (MediaPlayerElement == null) return;

            if (MediaPlayerElement._isrewindorfastforward)
            {
                MediaPlayerElement.RestoreRewindFastforwardSettings();
            }
        }


        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (!IsKeyboardFocusWithin)
            {
                e.Handled = this.Focus();
            }

            if (e.Key == Key.Escape)
            {

                if (MediaPlayerElement.WindowFullScreenState == WindowFullScreenState.FullScreen)
                    MediaPlayerElement.WindowsFullScreenAction();
                else if (MediaPlayerElement.CanEscapeKeyCloseMedia)
                {
                    MediaPlayerElement.WindowsClosedAction();
                }
            }
        }

        internal void IntiSliderPosition(bool IsDisplayBoardEnabled)
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
            if (MediaPlayerElement != null && _volumeControl != null)
            {
                _volumeControl.VolumeLevel = MediaPlayerElement.MediaPlayerServices.Volume;

            }
            if (VolumeControl != null)
            {
                VolumeControl.VolumeControlValueChanged -= VolumeControl_VolumeControlValueChanged;
                VolumeControl.VolumeControlValueChanged += VolumeControl_VolumeControlValueChanged;
            }
            if (MediaPlayerElement !=null)
                MediaPlayerElement.HookUpEvents();
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
            if (!MovieControlSettings.IsMediaSliderEnabled)
                return;

            PositionSlideTimerTooltip.Start();
        }

        private void MediaSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!MovieControlSettings.IsMediaSliderEnabled)
                return;

            PositionSlideTimerTooltip.Stop();
            Slider slider = sender as Slider;
            slider.Cursor = Cursors.Arrow;
            MovieBoard.SecondMovieBarText = null;
        }

        private void MediaSlider_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!MovieControlSettings.IsMediaSliderEnabled)
                return;


            Slider slider = sender as Slider;
            if (MediaPlayerElement == null) return;

            if (MediaPlayerElement.MediaPlayerServices.State == MovieMediaState.Playing ||
                MediaPlayerElement.MediaPlayerServices.State == MovieMediaState.Paused)
            {
                slider.Cursor = Cursors.Hand;
                PositionSlideTimerTooltip.Start();
            }
        }

        private void MediaSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MovieControlSettings.IsMediaSliderEnabled)
                return;

            Slider slider = sender as Slider;

            if (MediaPlayerElement == null) return;

            if (MediaPlayerElement.MediaPlayerServices.State == MovieMediaState.Playing ||
               MediaPlayerElement.MediaPlayerServices.State == MovieMediaState.Paused)
            {
                slider.Cursor = Cursors.Hand;
            }
        }

        private void DragCompleted()
        {
            if (MediaPlayerElement == null) return;

            MediaPlayerElement.MediaPlayerServices.CurrentTimer = TimeSpan.FromSeconds(MediaSlider.Value);
        }

        private void UpdateText(double x, double ratio, string p)
        {
            if (MovieControlSettings.DisableMovieBoardText)
                return;

            MovieBoard.SecondMovieBarText = "Seeking/ " + p;
        }

        private void CloseLastSeenBoardAction(bool clear = false)
        {
            MediaDetailProps.LastSeenTextInfo = TimeSpan.Zero;
            MediaDetailProps.IsLastSeenEnabled = false;
            if (clear)
                PlayableLastSeen.LastPlayedPoisition.ProgressLastSeen = 0;
        }

        private void SetLastSeenAction()
        {
            if (MediaPlayerElement == null) return;

            MediaPlayerElement.MediaPlayerServices.CurrentTimer += MediaDetailProps.LastSeenTextInfo;
            CloseLastSeenBoardAction();
        }

        internal void LoadPlaylistControls()
        {
            MovieControlSettings.IsNextButtonEnabled = true;
            MovieControlSettings.IsPreviousButtonEnabled = true;
            MovieControlSettings.IsPlaylistToggleEnabled = true;
            MovieControlSettings.IsRepeatToggleEnabled = true;
        }

        internal void UnloadedPlaylistControls()
        {
            MovieControlSettings.IsNextButtonEnabled = false;
            MovieControlSettings.IsPreviousButtonEnabled = false;
            MovieControlSettings.IsPlaylistToggleEnabled = false;
            MovieControlSettings.IsRepeatToggleEnabled = false;
        }

        internal void MediaPlayerElementNotifyLastSeen(IMediaPlayabeLastSeen playablelastseen)
        {
            if (playablelastseen.Progress > 0)
            {
                if (MediaPlayerElement == null) return;

                var timeSpan = TimeSpan.FromSeconds((double.Parse(MediaPlayerElement.MediaPlayerServices.Duration.TotalSeconds.ToString()) * playablelastseen.LastPlayedPoisition.ProgressLastSeen) / 100);
                MediaDetailProps.LastSeenTextInfo = timeSpan;
                MediaDetailProps.IsLastSeenEnabled = true;
                this.PlayableLastSeen = playablelastseen;
            }
        }

        public void SetMediaVolume(double vol)
        {
            if (MediaPlayerElement == null)
                return;
            this.MediaPlayerElement.MediaPlayerServices.Volume = (int)vol;
        }

        public void SetMovieTitleBoard(string info)
        {
            MovieBoard.MovieTitleText = MediaDetailProps.movieBoardInfo = info;
        }

        public void NotifyLastSeen(IMediaPlayabeLastSeen playablelastseen)
        {
            if (!MediaDetailProps.IsLastSeenEnabled)
                throw new Exception("LastSeen Control is disabled");

            if (playablelastseen.Progress > 0)
            {
                var timeSpan = TimeSpan.FromSeconds((double.Parse(MediaPlayerElement.MediaPlayerServices.Duration.TotalSeconds.ToString()) * playablelastseen.LastPlayedPoisition.ProgressLastSeen) / 100);
                MediaDetailProps.LastSeenTextInfo = timeSpan;
                MediaDetailProps.IsLastSeenEnabled = true;
                this.PlayableLastSeen = playablelastseen;
            }
        }

        public void CloseLastSeenBoard()
        {
            CloseLastSeenBoardAction();
        }

        public void SetControlSettings(MovieControlSettings controlSettings)
        {
            if(controlSettings != null)
            {
                controlSettings.MovieControl = this;
                MovieControlSettings = controlSettings;
            }
        }

        public void InitializeMediaPlayerControl(MediaPlayerElement mediaPlayerService,bool ignoreMediaText = false)
        {
            if (mediaPlayerService != null && _volumeControl != null)
            {
                this._volumeControl.VolumeLevel = mediaPlayerService.MediaPlayerServices.Volume;
                this.MediaDetailProps.MediaDuration = mediaPlayerService.MediaPlayerServices.Duration.TotalSeconds;
                mediaPlayerService.HookUpEvents();
                _currentInstance = this;
                this.Focus();
                CommandManager.InvalidateRequerySuggested();
            }
            if (mediaPlayerService == null && !ignoreMediaText)
            {
                this.MediaDetailProps.MediaDuration = 0;
                this.SetMovieTitleBoard(MediaDetailProps.DefaultMediaText);
            }
            this.MediaPlayerElement = mediaPlayerService;
        }

        internal void CloseAnyPopup()
        {
            if(_popup != null)
            {
                _popup.IsOpen = false;
            }
        }
    }
}
