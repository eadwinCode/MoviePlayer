using Common.ApplicationCommands;
using MovieHub.MediaPlayerElement.Interfaces;
using MovieHub.MediaPlayerElement.Service;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MovieHub.MediaPlayerElement.ViewModel;
using System.Windows.Threading;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using System.Windows.Data;
using MovieHub.MediaPlayerElement.Models;
using Movies.Enums;
using System.Collections.Generic;
using MovieHub.MediaPlayerElement.Util;
using System.Windows.Media;
using System.Dynamic;

namespace MovieHub.MediaPlayerElement
{
    public enum WindowFullScreenState
    {
        FullScreen,
        Normal
    };

    public enum MediaPlayerViewType
    {
        FullMediaPanel,
        MiniMediaPanel
    };
    
    [TemplatePart(Name = "MediaControlRegion", Type = typeof(ContentControl))]
    public sealed class MediaPlayerElement : Control
    {
        private ContentControl _mediaControlRegion;
        private ContentControl _mediaelementregion;
        private ContentControl _playlistregion;
        private WindowFullScreenState _windowFullScreenState = WindowFullScreenState.Normal;
        private dynamic _savedSettings;
        private IMediaPlayabeLastSeen _playlastableLastSeen;
        private IPlayable _currentstreamingitem;
        private bool _hasRegisteredEvents;
        private bool _hasInitialised;
        private bool _isDragging;
        private bool _isrewindorfastforward;
        private bool _awaitHostToRender;
        private bool _canAnimateControl = true;
        private bool _isPlaylistVisible = false;
        private bool _allowmediaAutodispose = true;
        private bool _allowMediaSizeEventExecute = true;
        private DispatcherTimer _controlAnimationTimer;

        internal IPlayable CurrentStreamingitem
        {
            get { return _currentstreamingitem; }
            set
            {
                if (_currentstreamingitem != null)
                    _currentstreamingitem.IsActive = false;
                _currentstreamingitem = value;
                if(CurrentlyStreamingChangedEvent != null)
                    CurrentlyStreamingChangedEvent.Invoke(value);
            }
        }

        internal IMediaPlayabeLastSeen PlayableLastSeen
        {
            get { return _playlastableLastSeen; }
            set
            {
                PlayableLastSeenSaveAction();
                _playlastableLastSeen = value;
            }
        }

        internal delegate void CurrentlyStreamingHandler(IPlayable playable);
        internal  CurrentlyStreamingHandler CurrentlyStreamingChangedEvent { get; set; }

        internal MediaMenu _mediaMenu;
        internal ContentControl _contentdockregion;

        /// <summary>
        /// Set to true if you want MediaPlayer to dispose on Unloaded event
        /// Set to false to manually dispose MediaPlayer resources.
        /// </summary>
        public bool AllowMediaPlayerAutoDispose { get { return _allowmediaAutodispose; } set { _allowmediaAutodispose = value; } }

        private MediaPlayerViewType mediaplayermode = MediaPlayerViewType.FullMediaPanel;

        public MediaPlayerViewType MediaPlayerViewType
        {
            get { return mediaplayermode; }
            set
            {
                mediaplayermode = value;
            }
        }


        static MediaPlayerElement()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaPlayerElement),
               new FrameworkPropertyMetadata(typeof(MediaPlayerElement)));
            RegisterControlCommands();
        }

        #region Dependency Properties
        public Stretch MediaStretch
        {
            get { return (Stretch)GetValue(MediaStretchProperty); }
            set { SetValue(MediaStretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaStretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaStretchProperty =
            DependencyProperty.Register("MediaStretch", typeof(Stretch), typeof(MediaPlayerElement), new PropertyMetadata(Stretch.Uniform,OnStretchPropertyChanged));

        private static void OnStretchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = d as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                mediaplayerelement.SetStretchProperty((Stretch)e.NewValue);
            }
        }
        
        public bool IsMediaContextMenuEnabled
        {
            get { return (bool)GetValue(IsMediaContextEnabledProperty); }
            set { SetValue(IsMediaContextEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMediaContextEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMediaContextEnabledProperty =
            DependencyProperty.Register("IsMediaContextMenuEnabled", typeof(bool), typeof(MediaPlayerElement), new PropertyMetadata(true));
        
        public bool AllowMovieControlAnimation
        {
            get { return (bool)GetValue(AllowMovieControlAnimationProperty); }
            set { SetValue(AllowMovieControlAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowMovieControlAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowMovieControlAnimationProperty =
            DependencyProperty.Register("AllowMovieControlAnimation", typeof(bool), typeof(MediaPlayerElement), new PropertyMetadata(false, OnAllowMovieControlAnimationChanged));

        private static void OnAllowMovieControlAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = d as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                mediaplayerelement.InitializeAnimationTimer((bool)e.NewValue);
            }
        }

        public bool CanMediaFastForwardOrRewind
        {
            get { return (bool)GetValue(CanMediaFastForwardOrRewindProperty); }
            set { SetValue(CanMediaFastForwardOrRewindProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanMediaFastForwardOrRewind.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanMediaFastForwardOrRewindProperty =
            DependencyProperty.Register("CanMediaFastForwardOrRewind", typeof(bool), typeof(MediaPlayerElement), new PropertyMetadata(true));
        
        public string MediaTitle
        {
            get { return (string)GetValue(MediaTitleProperty); }
            set { SetValue(MediaTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaTitleProperty =
            DependencyProperty.Register("MediaTitle", typeof(string), typeof(MediaPlayerElement), new PropertyMetadata("-No title-"));


        public VolumeState VolumeState
        {
            get { return (VolumeState)GetValue(VolumeStateProperty); }
            set { SetValue(VolumeStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMuted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeStateProperty =
            DependencyProperty.Register("VolumeState", typeof(VolumeState), typeof(MediaPlayerElement), new FrameworkPropertyMetadata(VolumeState.Active));

        
        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlaying.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(MediaPlayerElement), new PropertyMetadata(false));
        

        public bool IsCloseButtonVisible
        {
            get { return (bool)GetValue(IsCloseButtonVisibleProperty); }
            set { SetValue(IsCloseButtonVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFullScreenCloseButtonVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCloseButtonVisibleProperty =
            DependencyProperty.Register("IsCloseButtonVisible", typeof(bool), typeof(MediaPlayerElement),
                new PropertyMetadata(false, OnIsCloseButtonVisibleChanged));

        private static void OnIsCloseButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = d as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                mediaplayerelement.MovieControl.IsControlMediaCloseButtonEnabled = (bool)e.NewValue;
            }
        }

        public MoviesPlaylistManager PlaylistManager
        {
            get { return (MoviesPlaylistManager)GetValue(PlaylistManagerProperty); }
            private set { SetValue(PlaylistManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaylistManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaylistManagerProperty =
            DependencyProperty.Register("PlaylistManager", typeof(MoviesPlaylistManager), typeof(MediaPlayerElement), new PropertyMetadata(null, OnPlaylistManagerPropertyChanged));

        private static void OnPlaylistManagerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = d as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                if (mediaplayerelement.PlaylistManager != null)
                {
                    mediaplayerelement.LoadPlaylistComponents();

                }
                else
                {
                    mediaplayerelement.UnLoadPlaylistComponents();
                }
            }
        }

        public IMediaPlayerService MediaPlayerService
        {
            get { return (IMediaPlayerService)GetValue(MediaPlayerServiceProperty); }
            private set { SetValue(MediaPlayerServiceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaPlayerServiceProperty =
            DependencyProperty.Register("MediaPlayerService", typeof(IMediaPlayerService), typeof(MediaPlayerElement), new PropertyMetadata(null));

        public MovieControl MovieControl
        {
            get { return (MovieControl)GetValue(MovieControlProperty); }
            private set { SetValue(MovieControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MovieControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MovieControlProperty =
            DependencyProperty.Register("MovieControl", typeof(MovieControl), typeof(MediaPlayerElement), new PropertyMetadata(null));

        #endregion

        #region Events
        /// <summary>
        /// OnMediaSizeChanged is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnMediaSizeChangedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnMediaSizeChanged",
                          RoutingStrategy.Bubble,
                          typeof(EventHandler<MediaSizeChangedRoutedArgs>),
                          typeof(MediaPlayerElement));

        /// <summary>
        /// Raised On Media Size Changed.
        /// </summary>
        public event EventHandler<MediaSizeChangedRoutedArgs> OnMediaSizeChanged
        {
            add { AddHandler(OnMediaSizeChangedEvent, value); }
            remove { RemoveHandler(OnMediaSizeChangedEvent, value); }
        }

        /// <summary>
        /// OnFullScreenButtonToggle is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnFullScreenButtonToggleEvent =
          EventManager.RegisterRoutedEvent(
                          "OnFullScreenButtonToggle",
                          RoutingStrategy.Bubble,
                          typeof(EventHandler<WindowFullScreenRoutedEventArgs>),
                          typeof(MediaPlayerElement));

        /// <summary>
        /// Raised On FullScreen Button Toggled.
        /// </summary>
        public event EventHandler<WindowFullScreenRoutedEventArgs> OnFullScreenButtonToggle
        {
            add { AddHandler(OnFullScreenButtonToggleEvent, value); }
            remove { RemoveHandler(OnFullScreenButtonToggleEvent, value); }
        }

        /// <summary>
        /// OnMinimizedControlExecuted is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnMinimizedControlExecutedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnMinimizedControlExecuted",
                          RoutingStrategy.Bubble,
                          typeof(EventHandler<MediaPlayerViewTypeRoutedEventArgs>),
                          typeof(MediaPlayerElement));

        /// <summary>
        /// Raised when Minimized Control button is executed
        /// </summary>
        public event EventHandler<MediaPlayerViewTypeRoutedEventArgs> OnMinimizedControlExecuted
        {
            add { AddHandler(OnMinimizedControlExecutedEvent, value); }
            remove { RemoveHandler(OnMinimizedControlExecutedEvent, value); }
        }

        /// <summary>
        /// SetWindowTopMostProperty is a routed event.
        /// </summary>
        public static readonly RoutedEvent SetWindowTopMostPropertyEvent =
          EventManager.RegisterRoutedEvent(
                          "SetWindowTopMostProperty",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerElement));

        /// <summary>
        /// Raised when the topMost button is toggled
        /// </summary>
        public event RoutedEventHandler SetWindowTopMostProperty
        {
            add { AddHandler(SetWindowTopMostPropertyEvent, value); }
            remove { RemoveHandler(SetWindowTopMostPropertyEvent, value); }
        }

        /// <summary>
        /// OnMediaTitleChanged is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnMediaTitleChangedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnMediaTitleChanged",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerElement));

        /// <summary>
        /// Raised On Media Title Changed
        /// </summary>
        public event RoutedEventHandler OnMediaTitleChanged
        {
            add { AddHandler(OnMediaTitleChangedEvent, value); }
            remove { RemoveHandler(OnMediaTitleChangedEvent, value); }
        }

        /// <summary>
        /// OnCloseWindowToggled is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnCloseWindowToggledEvent =
          EventManager.RegisterRoutedEvent(
                          "OnCloseWindowToggled",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerElement));

        /// <summary>
        /// Raised On closewindow button pressed
        /// </summary>
        public event RoutedEventHandler OnCloseWindowToggled
        {
            add { AddHandler(OnCloseWindowToggledEvent, value); }
            remove { RemoveHandler(OnCloseWindowToggledEvent, value); }
        }

        #endregion

        public MediaPlayerElement()
        {
            InitializeComponents();
            _savedSettings = new ExpandoObject();
            this.Unloaded += (s, e) => UnloadComponents();
        }

        private void InitializeComponents()
        {
            MediaPlayerService = new MediaPlayerService();
            MovieControl = new MovieControl(MediaPlayerService);
            AllowMovieControlAnimation = true;
            
        }

        private void InitializeAnimationTimer(bool canAnimate)
        {
            if (canAnimate)
            {
                _controlAnimationTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(1200) };
                _controlAnimationTimer.Tick += _controlAnimationTimer_Tick;
                return;
            }
            _controlAnimationTimer = new DispatcherTimer(DispatcherPriority.Background);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mediaControlRegion = (ContentControl)this.GetTemplateChild("MediaControlRegion");
            _mediaelementregion = (ContentControl)this.GetTemplateChild("MediaElementRegion");
            _contentdockregion = (ContentControl)this.GetTemplateChild("ContentDockRegion");
            _playlistregion = (ContentControl)this.GetTemplateChild("PlaylistRegion");

            var videoplayer = (MediaPlayerService as MediaPlayerService).VideoPlayer;
            videoplayer.ApplyTemplate();
            this._mediaelementregion.Content = videoplayer;

            //if (MovieControl.IsPlaylistToggleEnabled)
            //    this.PlaylistRegion.Content = IPlaylistManager.GetPlaylistView();

            MovieControl.ApplyTemplate();
            _mediaControlRegion.Content = MovieControl;
            _hasInitialised = true;
            _awaitHostToRender = true;
            MediaPlayerUtil.ExecuteTimerAction(()=> HookUpEvents(),20);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (!IsKeyboardFocusWithin)
            {
                e.Handled = this.Focus();
            }

            if(e.Key == Key.Escape && _windowFullScreenState == WindowFullScreenState.FullScreen)
            {
                WindowsFullScreenAction();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (_isrewindorfastforward)
            {
                RestoreRewindFastforwardSettings();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Source == this)
                StartControlAnimation();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            e.Handled = this.Focus();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            this.Focus();
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            e.Handled = true;
        }

        #region Command Management

        private static void RegisterCommandBings(Type type, ICommand command, CommandBinding commandBinding, params InputGesture[] inputBinding)
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

        private static void RegisterControlCommands()
        {
            //Each Control shud have the commands
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.PausePlay, new CommandBinding(MovieControl.PausePlay, PauseOrPlay_Executed, PauseOrPlay_Enabled));
            RegisterCommandBings(typeof(MediaPlayerElement), VideoPlayerCommands.PlayList, new CommandBinding(VideoPlayerCommands.PlayList, PlayListToggle_Executed,Playlist_Enabled));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.RepeatCommand, new CommandBinding(MovieControl.RepeatCommand, RepeatCommand_Executed));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.ToggleMediaMenu, new CommandBinding(MovieControl.ToggleMediaMenu, ToggleMediaMenu_Executed, ToggleMediaMenu_Enabled));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.ResizeMediaAlways, new CommandBinding(MovieControl.ResizeMediaAlways, ResizeMediaAlways_Executed));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.TopMost, new CommandBinding(MovieControl.TopMost, TopMostCommand_Executed));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.CloseMediaWindow, new CommandBinding(MovieControl.CloseMediaWindow, CloseMediaWindow_Executed));

            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.Mute, new CommandBinding(MovieControl.Mute, Mute_executed));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.Rewind, new CommandBinding(MovieControl.Rewind, Rewind_executed, Rewind_enabled));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.ShiftRewind, new CommandBinding(MovieControl.ShiftRewind, ShiftRewind_executed, Rewind_enabled));

            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.FastForward, new CommandBinding(MovieControl.FastForward, FastForward_executed, Rewind_enabled));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.ShiftFastForward, new CommandBinding(MovieControl.ShiftFastForward, ShiftFastForward_executed, Rewind_enabled));


            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.VolUp, new CommandBinding(MovieControl.VolUp, VolUp_Executed, Volume_Enabled));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.VolDown, new CommandBinding(MovieControl.VolDown, VolDown_Executed, Volume_Enabled));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.FullScreen, new CommandBinding(MovieControl.FullScreen, FullScreen_Executed, FullScreen_Enabled));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.Next, new CommandBinding(MovieControl.Next, Next_Executed, Next_Enabled));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.Previous, new CommandBinding(MovieControl.Previous, Previous_Executed, Previous_Enabled));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.ControlViewChangeCommand, new CommandBinding(MovieControl.ControlViewChangeCommand, ControlViewChangeCommand_Executed));
            RegisterCommandBings(typeof(MediaPlayerElement), MovieControl.MinimizeControlCommand, new CommandBinding(MovieControl.MinimizeControlCommand, MinimizeControlCommand_Executed, MinimizeControlCommand_Enabled));

        }

        private static void MinimizeControlCommand_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                e.CanExecute = mediaPlayerElement.MediaPlayerService.CanPause;
            else
                e.CanExecute = false;
        }

        private static void MinimizeControlCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                mediaPlayerElement.MinimizeControlAction();
        }
        
        private static void CloseMediaWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                mediaPlayerElement.WindowsClosedAction(); 
        }
        
        private static void ControlViewChangeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.ControlViewChangeCommandAction();
            }
        }
        
        private static void Previous_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                e.CanExecute = mediaPlayerElement.PlaylistManager == null ? false : mediaPlayerElement.PlaylistManager.CanPrevious;
            }
        }

        private static void Previous_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.PreviousAction();
            }
        }
        
        private static void Next_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                e.CanExecute = mediaPlayerElement.PlaylistManager == null? false : mediaPlayerElement.PlaylistManager.CanNext ;
            }
        }

        private static void Next_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.NextAction();
            }
        }
        
        private static void FullScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                mediaPlayerElement.WindowsFullScreenAction();
        }

        private static void FullScreen_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                e.CanExecute = mediaPlayerElement.MovieControl.IsFullScreenToggleEnabled;
            else
                e.CanExecute = false;
        }

        private static void ResizeMediaAlways_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement._allowMediaSizeEventExecute = !mediaPlayerElement._allowMediaSizeEventExecute;
            }
        }

        private static void TopMostCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.MediaPlayerElementRaiseEvent(SetWindowTopMostPropertyEvent);
            }
        }

        private static void Volume_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                e.CanExecute = mediaPlayerElement.VolumeState != VolumeState.Muted;
            else
                e.CanExecute = false;
        }

        private static void VolDown_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.MovieControl.VolumeControl.VolumeLevel -= 10;
                mediaPlayerElement.PreviewControl();
            }
        }

        private static void VolUp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.MovieControl.VolumeControl.VolumeLevel += 10;
                mediaPlayerElement.PreviewControl();
            }
        }

        private static void ToggleMediaMenu_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                mediaPlayerElement._mediaMenu.ShowDialog();
        }

        private static void ToggleMediaMenu_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                e.CanExecute = !mediaPlayerElement.MediaPlayerService.HasStopped;
            else
                e.CanExecute = false;
        }

        private static void Mute_executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                mediaPlayerElement.MuteAction();
        }

        private static void RepeatCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                mediaPlayerElement.RepeatModeAction(); ;
        }

        private static void PlayListToggle_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.TogglePlaylistView();
            }
        }

        private static void Playlist_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                e.CanExecute = mediaPlayerElement.PlaylistManager != null && mediaPlayerElement.MediaPlayerViewType == MediaPlayerViewType.FullMediaPanel;
            }
            
        }

        private static void PauseOrPlay_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                e.CanExecute = mediaPlayerElement.CurrentStreamingitem != null || !mediaPlayerElement.MediaPlayerService.HasLoadedMedia;
            else
                e.CanExecute = false;
        }

        private static void PauseOrPlay_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                mediaPlayerElement.PauseOrPlayAction();
        }

        private static void ShiftFastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.ReWindFastForward();
                mediaPlayerElement.MediaPlayerService.CurrentTimer += TimeSpan.FromMilliseconds(1500);
            }
        }

        private static void Rewind_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
                e.CanExecute = mediaPlayerElement.MediaPlayerService.IsSeekable;
            else
                e.CanExecute = false;
        }

        private static void FastForward_executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.ReWindFastForward();
                mediaPlayerElement.MediaPlayerService.CurrentTimer += TimeSpan.FromMilliseconds(10000);
            }
        }

        private static void ShiftRewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.ReWindFastForward();
                mediaPlayerElement.MediaPlayerService.CurrentTimer -= TimeSpan.FromMilliseconds(1500);
            }
        }

        private static void Rewind_executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayerElement mediaPlayerElement = sender as MediaPlayerElement;
            if (mediaPlayerElement != null)
            {
                mediaPlayerElement.ReWindFastForward();
                mediaPlayerElement.MediaPlayerService.CurrentTimer -= TimeSpan.FromMilliseconds(10000);
            }
        }
        #endregion

        private void HookUpEvents()
        {
            _mediaMenu = new MediaMenu(this);

            MovieControl.MouseEnter += (s, e) =>
            {
                _canAnimateControl = false;
            };
            MovieControl.MouseLeave += (s, e) =>
            {
                _canAnimateControl = true;
            };
            if (MediaPlayerViewType == MediaPlayerViewType.FullMediaPanel)
            {
                _mediaelementregion.MouseDoubleClick += _mediaelementregion_MouseDoubleClick;
            }

            if (MovieControl.IsMediaSliderEnabled)
            {
                MovieControl.MediaSlider.ThumbDragStarted += MediaSlider_ThumbDragStarted;
                MovieControl.MediaSlider.ThumbDragCompleted += MediaSlider_ThumbDragCompleted;
            }
            MediaPlayerService.OnMediaOpening += MediaPlayerService_OnMediaOpening;
            MediaPlayerService.OnMediaOpened += MediaPlayerService_OnMediaOpened;
            MediaPlayerService.OnMediaStopped += MediaPlayerService_OnMediaStopped;
            MediaPlayerService.OnStateChanged += MediaPlayerService_OnStateChanged;
            MediaPlayerService.OnTimeChanged += MediaPlayerService_OnTimeChanged;
            MediaPlayerService.EncounteredError += MediaPlayerService_EncounteredError;
            MediaPlayerService.EndReached += MediaPlayerService_EndReached;
            MediaPlayerService.Buffering += MediaPlayerService_Buffering;
            _hasRegisteredEvents = true;

        }

        private void _controlAnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!IsPlaying)
            {
                _controlAnimationTimer.Stop();
                return;
            }
            if ( _canAnimateControl && !_isPlaylistVisible)
            {
                _controlAnimationTimer.Stop();
                HideControl();
            }
            else if (!_canAnimateControl)
            {
                this._controlAnimationTimer.Stop();
            }
        }

        private void MediaSlider_ThumbDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //start animation
            MediaPlayerService.CurrentTimer = TimeSpan.FromSeconds(MovieControl.MediaSlider.Value);
            _isDragging = false;
        }

        private void MediaSlider_ThumbDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _isDragging = true;
            //stop animation
        }

        private void _mediaelementregion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WindowsFullScreenAction();
        }
        
        private void MediaPlayerService_Buffering(object sender, MediaBufferingEventArgs e)
        {
            SetMovieBoardText(string.Format("Buffering {0} %", e.NewCache));
            if (e.NewCache == 100)
                SetMovieBoardText(string.Format("Playing - {0} ", CurrentStreamingitem.MediaTitle));
        }

        private void MediaPlayerService_EndReached(object sender, EventArgs e)
        {
            OnMediaFinishedAction();
        }

        private void MediaPlayerService_EncounteredError(object sender, EventArgs e)
        {
            OnStopAction();
        }

        private void MediaPlayerService_OnTimeChanged(object sender, EventArgs e)
        {
            TimeChangeAction();
        }

        private void MediaPlayerService_OnStateChanged(object sender, EventArgs e)
        {
            SetMovieBoardText(string.Format("{0} - {1}", MediaPlayerService.State, CurrentStreamingitem.MediaTitle));
            IsPlaying = MediaPlayerService.State == MovieMediaState.Playing ? true : false;
            if (IsPlaying) StartControlAnimation(); else ResetControlAnimation();
        }

        private void MediaPlayerService_OnMediaStopped(object sender, EventArgs e)
        {
            OnStopAction();
        }

        private void MediaPlayerService_OnMediaOpened(object sender, EventArgs e)
        {
            SetMediaControlDetails();

            if (_mediaMenu != null)
                _mediaMenu.Dispose();

            CurrentStreamingitem.IsActive = true;
            IsPlaying = true;

            StartControlAnimation();
            if (_allowMediaSizeEventExecute)
                MediaPlayerElementRaiseEvent(new MediaSizeChangedRoutedArgs(OnMediaSizeChangedEvent, this, MediaPlayerService.PixelHeight, MediaPlayerService.PixelWidth));

            this.MovieControl.MediaPlayerElementNotifyLastSeen(PlayableLastSeen);
        }

        private void MediaPlayerService_OnMediaOpening(object sender, EventArgs e)
        {
            SetMovieBoardText(string.Format("Opening - {0}", CurrentStreamingitem.MediaTitle));
        }
        
        private void UnloadComponents()
        {
            if (!AllowMediaPlayerAutoDispose || MediaPlayerService == null)
                return;

            if (!MediaPlayerService.HasStopped)
                MediaPlayerService.Stop();

            MediaPlayerService.Dispose();
            BindingOperations.ClearAllBindings(this);
            //CurrentStreamingitem.IsActive = false;
            //PlayableLastSeen = null;
            #region Useless
            //MovieControl.Dispose();
            //_mediaMenu = null;
            //MediaPlayerService = null;
            //MovieControl = null;


            #endregion
        }

        private void MediaPlayerElementRaiseEvent(RoutedEventArgs _event)
        {
            RaiseEvent(_event);
        }

        private void MediaPlayerElementRaiseEvent(RoutedEvent _event)
        {
            RaiseEvent(new RoutedEventArgs(_event, this));
        }

        private void SetMediaControlDetails()
        {
            this.MovieControl.MediaDuration = MediaPlayerService.Duration.TotalSeconds;
            this.MovieControl.SetMediaVolume(MovieControl.VolumeControl.VolumeLevel);
        }
        
        private void TimeChangeAction()
        {
            if (!_isDragging)
            {
                MovieControl.CurrentMediaTime = MediaPlayerService.CurrentTimer.TotalSeconds;
                PlayableLastSeen.Progress = Math.Round(((MovieControl.CurrentMediaTime / MovieControl.MediaDuration) * 100), 2);
            }
        }

        private void UnLoadPlaylistComponents()
        {
            MovieControl.UnloadedPlaylistControls();
            _playlistregion.Content = null;
        }

        private void LoadPlaylistComponents()
        {
            MovieControl.LoadPlaylistControls();
            _playlistregion.Content = PlaylistManager;
        }

        private void OnStopAction()
        {
            this.IsPlaying = false;
            //reset Animation
            ResetControlAnimation();
            MovieControl.CloseLastSeenBoard();
            PlayableLastSeenSaveAction();
            CurrentStreamingitem.IsActive = false;
            MovieControl.CurrentMediaTime = 0.0;
        }

        private void ControlViewChangeCommandAction()
        {
            MovieControl.ControlViewType = MovieControl.ControlViewType == MediaControlViewType.LargeView ? MediaControlViewType.MiniView : MediaControlViewType.LargeView;
        }

        private void PlayableLastSeenSaveAction()
        {
            if (_playlastableLastSeen != null && !string.IsNullOrEmpty(_playlastableLastSeen.LastPlayedPoisition.FileName))
            {
                _playlastableLastSeen.SetProgress();
                if (!_playlastableLastSeen.LastPlayedPoisition.Exist && _playlastableLastSeen.Progress > 0)
                {
                    _playlastableLastSeen.LastPlayedPoisition.Add();
                }
                _playlastableLastSeen.LastPlayedPoisition.Save();
            }
        }

        private void OnMediaFinishedAction()
        {
            ResetControlAnimation();
            PlayableLastSeen.PlayCompletely();
            if(PlaylistManager != null && MovieControl.RepeatMode != RepeatMode.NoRepeat)
                MediaPlayerUtil.ExecuteTimerAction(() => StartRepeatAction(), 50);
        }

        private void StartRepeatAction()
        {
            if (MovieControl.RepeatMode == RepeatMode.RepeatOnce)
            {
                Source(CurrentStreamingitem);
                return;
            }

            var vfc = PlaylistManager.GetNextItem();
            if (vfc != null)
                this.Source(vfc);
        }

        private void ReWindFastForward()
        {
            if (!_isrewindorfastforward)
            {
                _isrewindorfastforward = true;

                if (VolumeState == VolumeState.Active)
                {
                    MediaPlayerService.ToggleMute();
                }
                ResetControlAnimation();
            }
        }

        private void RestoreRewindFastforwardSettings()
        {
            if (VolumeState == VolumeState.Active)
            {
                MediaPlayerService.ToggleMute();
            }
            _isrewindorfastforward = false;

            StartControlAnimation();
        }

        private void RepeatModeAction()
        {
            if (MovieControl.RepeatMode == RepeatMode.NoRepeat)
            {
                MovieControl.RepeatMode = RepeatMode.Repeat;
            }
            else
            if (MovieControl.RepeatMode == RepeatMode.Repeat)
            {
                MovieControl.RepeatMode = RepeatMode.RepeatOnce;
            }
            else
            if (MovieControl.RepeatMode == RepeatMode.RepeatOnce)
            {
                MovieControl.RepeatMode = RepeatMode.NoRepeat;
            }
        }

        private void MuteAction()
        {
            if (!MediaPlayerService.IsMute)
            {
                MediaPlayerService.ToggleMute();
                //MovieControl.VolumeControl.IsEnabled = false;
                VolumeState = VolumeState.Muted;
            }
            else
            {
                MediaPlayerService.ToggleMute();
               // MovieControl.VolumeControl.IsEnabled = true;
                VolumeState = VolumeState.Active;
            }
        }

        private void PreviousAction()
        {
            if (!MediaPlayerService.HasStopped)
                MediaPlayerService.Stop();
            Source(PlaylistManager.GetPreviousItem());
        }

        private void NextAction()
        {
            if (!MediaPlayerService.HasStopped)
                MediaPlayerService.Stop();
            Source(PlaylistManager.GetNextItem());
        }

        private void WindowsFullScreenAction()
        {
            _windowFullScreenState = _windowFullScreenState == WindowFullScreenState.Normal ? WindowFullScreenState.FullScreen : WindowFullScreenState.Normal;
            var routedevent = new WindowFullScreenRoutedEventArgs(OnFullScreenButtonToggleEvent, this, _windowFullScreenState);
            this.MediaPlayerElementRaiseEvent(routedevent);
        }

        private void SetStretchProperty(Stretch newValue)
        {
            MediaPlayerService.VideoStretch = newValue;
        }

        private void MinimizeControlAction()
        {
            if (this.MediaPlayerViewType == MediaPlayerViewType.FullMediaPanel)
            {
                SaveMediaDefinitedSettings();
                if (_isPlaylistVisible)
                    TogglePlaylistView();
                this.MovieControl.ControlViewType = MediaControlViewType.MiniView;
                this.MediaPlayerViewType = MediaPlayerViewType.MiniMediaPanel;
                this.MediaStretch = Stretch.UniformToFill;
                _allowMediaSizeEventExecute = false;
                
            }
            else
            {
                RestoreMediaDefinitedSetting();
            }

            var routedevent = new MediaPlayerViewTypeRoutedEventArgs(OnMinimizedControlExecutedEvent, this, this.MediaPlayerViewType);
            this.MediaPlayerElementRaiseEvent(routedevent);

            PreviewControl();
        }

        private void RestoreMediaDefinitedSetting()
        {
            this.MovieControl.ControlViewType = _savedSettings.MovieControlViewType;
            this.MediaPlayerViewType = _savedSettings.MediaPlayerMode;
            this.IsMediaContextMenuEnabled = _savedSettings.AllowContextMenu;
            this._allowMediaSizeEventExecute = _savedSettings.AllowMediaResize;
            this.MediaStretch = _savedSettings.stretchValue;

            //if(_savedSettings.PlaylistContent != null)
            //{
            //    PlaylistManager.ApplyTemplate();
            //    this._playlistregion.Content = PlaylistManager;
            //}
        }

        private void SaveMediaDefinitedSettings()
        {
            _savedSettings.MovieControlViewType = this.MovieControl.ControlViewType;
            _savedSettings.AllowContextMenu = this.IsMediaContextMenuEnabled;
            _savedSettings.MediaPlayerMode = MediaPlayerViewType;
            _savedSettings.AllowMediaResize = _allowMediaSizeEventExecute;
            _savedSettings.stretchValue = this.MediaStretch;
        }

        private void WindowsClosedAction()
        {
            this.MediaPlayerElementRaiseEvent(OnCloseWindowToggledEvent);
        }
        
        private void ShowControl()
        {
            MovieControl.SetIsMouseOverMediaElement(MovieControl as UIElement, null);
            this.Cursor = Cursors.Arrow;
        }

        private void HideControl()
        {
            MovieControl.SetIsMouseOverMediaElement(MovieControl as UIElement, false);
            this.Cursor = Cursors.None;
        }

        private void PreviewControl()
        {
            _controlAnimationTimer.Stop();
            ShowControl();
            _controlAnimationTimer.Start();
        }

        private void ResetControlAnimation()
        {
            _controlAnimationTimer.Stop();
            ShowControl();
        }

        private void StartControlAnimation()
        {
            ShowControl();
            _controlAnimationTimer.Start();
        }

        private void InitializePlaylist()
        {
            if (PlaylistManager == null)
                PlaylistManager = new MoviesPlaylistManager(this);
        }

        private void ExecuteLater(Action action, long milliseconds)
        {
            MediaPlayerUtil.ExecuteTimerAction(action, milliseconds);
        }

        private void PauseOrPlayAction(bool igonreMediaState = false)
        {
            if (igonreMediaState)
            {
                MediaPlayerService.Play();
                return;
            }

            if (MediaPlayerService.State == MovieMediaState.Playing)
            {
                MediaPlayerService.PauseOrResume();
                IsPlaying = false;
            }
            else if (MediaPlayerService.State == MovieMediaState.Ended ||
                MediaPlayerService.State == MovieMediaState.Paused ||
                MediaPlayerService.State == MovieMediaState.Stopped)
            {
                if (MediaPlayerService.State == MovieMediaState.Ended)
                {
                    this.Source(CurrentStreamingitem);
                    return;
                }
                MediaPlayerService.Play();
                IsPlaying = true;
            }
        }

        internal void TogglePlaylistView()
        {
            if (PlaylistManager.TogglePlayVisibility())
            {
                PlaylistManager.Focus();
                ResetControlAnimation();
                _isPlaylistVisible = true;
                return;
            }
            StartControlAnimation();
            _isPlaylistVisible = false;
        }

        internal void SetMovieBoardText(string info)
        {
            MediaTitle = info;
            this.MovieControl.SetMovieTitleBoard(info);
            this.MediaPlayerElementRaiseEvent(new RoutedEventArgs(OnMediaTitleChangedEvent, this));
        }
        
        internal void SourceFromPlaylist(IPlayable file_to_pay)
        {
            Source(file_to_pay);
        }

        public void Source(IPlayable file_to_pay)
        {
            if (!_hasInitialised && !_awaitHostToRender)
            {
                this.ApplyTemplate();
            }

            if (!MediaPlayerService.HasStopped)
                MediaPlayerService.Stop();

            this.CurrentStreamingitem = file_to_pay;
            if (file_to_pay.Url == null)
                throw new NotImplementedException("No media Url");

            if (!_hasInitialised)
                throw new Exception("MediaPlayerElement not Initialized");

            (MediaPlayerService as MediaPlayerService).LoadMedia(file_to_pay.Url);

            PlayableLastSeen = file_to_pay is IMediaPlayabeLastSeen ? file_to_pay as IMediaPlayabeLastSeen : DummyIMediaPlayabeLastSeen.CreateDummtObject();

        }
        
        public void Source(Uri file_to_pay)
        {
            Source(DummyPlayableFile.Parse(file_to_pay));
        }

        public void Source(IPlaylistModel plm)
        {
            //if (!_hasInitialised && !_awaitHostToRender)
            //{
            //    ExecuteLater(() => this.Source(plm), 50);
            //    _awaitHostToRender = true;
            //    return;
            //}
            if (!_hasInitialised && !_awaitHostToRender)
            {
                this.ApplyTemplate();
            }

            InitializePlaylist();
            PlaylistManager.PlayFromAList(plm);
        }
        
        public void Source(IPlayable playFile, IEnumerable<IPlayable> TemperalList)
        {
            if (!_hasInitialised && !_awaitHostToRender)
            {
                this.ApplyTemplate();
            }

            Source(playFile);
            InitializePlaylist();
            PlaylistManager.PlayFromTemperalList(TemperalList);
        }

        public void AddToPlaylist(IPlayable vfc)
        {
            if (!_hasInitialised && !_awaitHostToRender)
            {
                this.ApplyTemplate();
            }

            InitializePlaylist();
            PlaylistManager.Add(vfc);
        }

        public void AddRangeToPlaylist(IEnumerable<IPlayable> EnumerableVfc)
        {
            if (!_hasInitialised && !_awaitHostToRender)
            {
                this.ApplyTemplate();
            }

            InitializePlaylist();
            PlaylistManager.AddRange(EnumerableVfc);
        }
        
        public void Dispose()
        {
            AllowMediaPlayerAutoDispose = true;
            this.UnloadComponents();
        }
        
    }
}
