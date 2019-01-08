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
using Common.Util;

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
    public sealed class MediaPlayerElement : Control, IMediaPlayerElement
    {
        private ContentControl _mediaControlRegion;
        private ContentControl _mediaelementregion;
        private ContentControl _playlistregion;
        private dynamic _savedSettings;
        private IMediaPlayabeLastSeen _playlastableLastSeen;
        private IPlayable _currentstreamingitem;
        private bool _hasInitialised;
        private bool _isDragging;
        internal bool _isrewindorfastforward;
        private bool _awaitHostToRender;
        private bool _canAnimateControl = true;
        private bool _isPlaylistVisible = false;
        private bool _allowmediaAutodispose = true;
        private MovieControl _savedSecondaryControl = null;
        internal bool _allowMediaSizeEventExecute = true;
        private DispatcherTimer _controlAnimationTimer;
        private static bool IscheckingForRepeating = false;
        private static MediaPlayerElement _current;
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
        
        private bool canescapekeyclosemedia;

        public bool CanEscapeKeyCloseMedia
        {
            get { return canescapekeyclosemedia; }
            set { canescapekeyclosemedia = value; }
        }
        
        static MediaPlayerElement()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaPlayerElement),
               new FrameworkPropertyMetadata(typeof(MediaPlayerElement)));
            RegisterMediaPlayerServiceEvent();
            RegisterControlCommands();
        }

        #region Dependency Properties
        
        public MediaPlayerViewType MediaPlayerViewType
        {
            get { return (MediaPlayerViewType)GetValue(MediaPlayerViewTypeProperty); }
            set { SetValue(MediaPlayerViewTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaPlayerViewType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaPlayerViewTypeProperty =
            DependencyProperty.Register("MediaPlayerViewType", typeof(MediaPlayerViewType), typeof(MediaPlayerElement), new PropertyMetadata(MediaPlayerViewType.FullMediaPanel));



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
            private set { SetValue(MediaTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MediaTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaTitleProperty =
            DependencyProperty.Register("MediaTitle", typeof(string), typeof(MediaPlayerElement), new PropertyMetadata("-No title-"));


        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            private set { SetValue(IsPlayingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlaying.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(MediaPlayerElement), new PropertyMetadata(false));

        
        public WindowFullScreenState WindowFullScreenState
        {
            get { return (WindowFullScreenState)GetValue(WindowFullScreenStateProperty); }
            private set { SetValue(WindowFullScreenStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WindowFullScreenState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowFullScreenStateProperty =
            DependencyProperty.Register("WindowFullScreenState", typeof(WindowFullScreenState), typeof(MediaPlayerElement), new FrameworkPropertyMetadata() { DefaultValue = WindowFullScreenState.Normal});



        public bool CanRenderControl
        {
            get { return (bool)GetValue(CanRenderControlProperty); }
            set { SetValue(CanRenderControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanRenderControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanRenderControlProperty =
            DependencyProperty.Register("CanRenderControl", typeof(bool), typeof(MediaPlayerElement), new PropertyMetadata(true,OnCanrenderControlChanged));

        private static void OnCanrenderControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = d as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                var newValue = (Boolean)e.NewValue;

                mediaplayerelement.AllowMovieControlAnimation = newValue;
                if (mediaplayerelement._mediaControlRegion == null) return;

                if (newValue)
                {
                    mediaplayerelement._mediaControlRegion.Content = mediaplayerelement.MovieControl;
                    return;
                }
                mediaplayerelement._mediaControlRegion.Content = null;
            }
        }

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
                mediaplayerelement.MovieControl.MovieControlSettings.IsControlMediaCloseButtonEnabled = (bool)e.NewValue;
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

        public IMediaPlayerService MediaPlayerServices
        {
            get { return (IMediaPlayerService)GetValue(MediaPlayerServiceProperty); }
            private set { SetValue(MediaPlayerServiceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaPlayerServiceProperty =
            DependencyProperty.Register("MediaPlayerService", typeof(IMediaPlayerService), typeof(MediaPlayerElement), new PropertyMetadata(null));
        
        public MovieControl UseSecondaryControl
        {
            get { return (MovieControl)GetValue(UseSecondaryControlProperty); }
            set { SetValue(UseSecondaryControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseSecondaryControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseSecondaryControlProperty =
            DependencyProperty.Register("UseSecondaryControl", typeof(MovieControl), typeof(MediaPlayerElement), new PropertyMetadata(null,OnSecondaryControlChanged));

        private static void OnSecondaryControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MediaPlayerElement mediaplayerelement = d as MediaPlayerElement;
            if (mediaplayerelement != null)
            {
                MovieControl movieControl = e.NewValue as MovieControl;
                if (e.NewValue != null)
                {
                    mediaplayerelement.CanRenderControl = false;
                    movieControl.InitializeMediaPlayerControl(mediaplayerelement);
                    mediaplayerelement._savedSecondaryControl = movieControl;
                }

                bool shouldRender = false;
                if (movieControl == null)
                {
                    movieControl = new MovieControl(mediaplayerelement);
                    shouldRender = true;
                    var oldcontrol = e.OldValue as MovieControl;
                    if (oldcontrol != null)
                    {
                        oldcontrol.InitializeMediaPlayerControl(null,true);
                    }
                    movieControl.SetControlSettings(oldcontrol.MovieControlSettings);
                    movieControl.MediaDetailProps = oldcontrol.MediaDetailProps;
                }
                movieControl.ApplyTemplate();
                mediaplayerelement.MovieControl = movieControl;
                if (shouldRender)
                {
                    mediaplayerelement.CanRenderControl = shouldRender;
                }
            }
        }

        public MovieControl MovieControl
        {
            get { return (MovieControl)GetValue(MovieControlProperty); }
            private set { SetValue(MovieControlProperty, value); }
        }

        public VolumeState VolumeState
        {
            get { return MovieControl.VolumeControl.VolumeState; }
            private set { MovieControl.VolumeControl.VolumeState = value; }
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

        /// <summary>
        /// OnMediaInfoChanged is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnMediaInfoChangedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnMediaInfoChanged",
                          RoutingStrategy.Bubble,
                          typeof(EventHandler<MediaInfoChangedEventArgs>),
                          typeof(MediaPlayerElement));

        /// <summary>
        /// Raised On Media Information Changed.
        /// </summary>
        public event EventHandler<MediaInfoChangedEventArgs> OnMediaInfoChanged
        {
            add { AddHandler(OnMediaInfoChangedEvent, value); }
            remove { RemoveHandler(OnMediaInfoChangedEvent, value); }
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
            MediaPlayerServices = new MediaPlayerService();
            MovieControl = new MovieControl(this);
            AllowMovieControlAnimation = true;
            _current = this;
        }
        
        private void InitializeAnimationTimer(bool canAnimate)
        {
            if (canAnimate)
            {
                _controlAnimationTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(1200) };
                _controlAnimationTimer.Tick += _controlAnimationTimer_Tick;
                return;
            }
            if (_controlAnimationTimer != null)
            {
                _controlAnimationTimer.Tick -= _controlAnimationTimer_Tick;
                _controlAnimationTimer = new DispatcherTimer(DispatcherPriority.Background);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mediaControlRegion = (ContentControl)this.GetTemplateChild("MediaControlRegion");
            _mediaelementregion = (ContentControl)this.GetTemplateChild("MediaElementRegion");
            _contentdockregion = (ContentControl)this.GetTemplateChild("ContentDockRegion");
            _playlistregion = (ContentControl)this.GetTemplateChild("PlaylistRegion");

            var videoplayer = (MediaPlayerServices as MediaPlayerService).VideoPlayer;
            videoplayer.ApplyTemplate();
            this._mediaelementregion.Content = videoplayer;

            //if (MovieControl.IsPlaylistToggleEnabled)
            //    this.PlaylistRegion.Content = IPlaylistManager.GetPlaylistView();

            MovieControl.ApplyTemplate();
            if (CanRenderControl)
            {
                _mediaControlRegion.Content = MovieControl;
            }
            _hasInitialised = true;
            _awaitHostToRender = true;
            if (MediaPlayerViewType == MediaPlayerViewType.FullMediaPanel)
            {
                _mediaelementregion.MouseDoubleClick += _mediaelementregion_MouseDoubleClick;
            }
            //MediaPlayerUtil.ExecuteTimerAction(()=> HookUpEvents(),50);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

    
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            e.Handled = this.MovieControl.Focus();
        }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    base.OnRender(drawingContext);
        //    this.Focus();
        //}

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            e.Handled = true;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Source == this)
                StartControlAnimation();
        }

        #region Command Management

        #region MediaPlayerServices
        private static void RegisterMediaPlayerServiceEvent()
        {
            EventManager.RegisterClassHandler(typeof(MediaPlayerService), MediaPlayerService.OnMediaOpeningEvent, new RoutedEventHandler(MediaPlayerService_OnMediaOpeningEvent));
            EventManager.RegisterClassHandler(typeof(MediaPlayerService), MediaPlayerService.OnMediaOpenedEvent, new RoutedEventHandler(MediaPlayerService_OnMediaOpenedEvent));
            EventManager.RegisterClassHandler(typeof(MediaPlayerService), MediaPlayerService.OnMediaStoppedEvent, new RoutedEventHandler(MediaPlayerService_OnMediaStoppedEvent));
            EventManager.RegisterClassHandler(typeof(MediaPlayerService), MediaPlayerService.OnStateChangedEvent, new RoutedEventHandler(MediaPlayerService_OnStateChangedEvent));
            EventManager.RegisterClassHandler(typeof(MediaPlayerService), MediaPlayerService.OnTimeChangedEvent, new RoutedEventHandler(MediaPlayerService_OnTimeChangedEvent));
            EventManager.RegisterClassHandler(typeof(MediaPlayerService), MediaPlayerService.EncounteredErrorEvent, new RoutedEventHandler(MediaPlayerService_EncounteredErrorEvent));
            EventManager.RegisterClassHandler(typeof(MediaPlayerService), MediaPlayerService.EndReachedEvent, new RoutedEventHandler(MediaPlayerService_EndReachedEvent));
            EventManager.RegisterClassHandler(typeof(MediaPlayerService), MediaPlayerService.BufferingEvent, new EventHandler<MediaBufferingEventArgs>(MediaPlayerService_BufferingEvent));
            EventManager.RegisterClassHandler(typeof(MediaPlayerService), MediaPlayerService.OnMediaInfoChangedEvent, new EventHandler<MediaInfoChangedEventArgs>(MediaPlayerService_OnMediaInfoChangedEvent));
        }

        private static void MediaPlayerService_OnMediaInfoChangedEvent(object sender, MediaInfoChangedEventArgs e)
        {
            if(_current !=null)
                 _current.Dispatcher.BeginInvoke((Action)(() => _current.RaiseEvent(new MediaInfoChangedEventArgs(OnMediaInfoChangedEvent, e.Source, e.MediaInformation))));
        }

        private static void MediaPlayerService_BufferingEvent(object sender, MediaBufferingEventArgs e)
        {
            if (_current != null && _current.IsLoaded)
                _current.MediaPlayerService_Buffering(e);
        }

        private static void MediaPlayerService_OnMediaStoppedEvent(object sender, RoutedEventArgs e)
        {
            if (_current != null && _current.IsLoaded)
                _current.MediaPlayerService_OnMediaStopped();
        }

        private static void MediaPlayerService_OnStateChangedEvent(object sender, RoutedEventArgs e)
        {
            if(_current != null && _current.IsLoaded)
                _current.MediaPlayerService_OnStateChanged();
        }

        private static void MediaPlayerService_OnTimeChangedEvent(object sender, RoutedEventArgs e)
        {
            if (_current != null && _current.IsLoaded)
                _current.MediaPlayerService_OnTimeChanged();
        }

        private static void MediaPlayerService_EncounteredErrorEvent(object sender, RoutedEventArgs e)
        {
            if (_current != null && _current.IsLoaded)
                _current.MediaPlayerService_EncounteredError();
        }

        private static void MediaPlayerService_EndReachedEvent(object sender, RoutedEventArgs e)
        {
            if (_current != null && _current.IsLoaded)
                _current.MediaPlayerService_EndReached();
        }

        private static void MediaPlayerService_OnMediaOpenedEvent(object sender, RoutedEventArgs e)
        {
            if (_current != null && _current.IsLoaded)
                _current.MediaPlayerService_OnMediaOpened();
        }

        private static void MediaPlayerService_OnMediaOpeningEvent(object sender, RoutedEventArgs e)
        {
            if (_current != null && _current.IsLoaded)
                _current.MediaPlayerService_OnMediaOpening();
        }

        private void MediaPlayerService_Buffering(MediaBufferingEventArgs e)
        {
            SetMovieBoardText(string.Format("Buffering {0} %", e.NewCache));
            if (e.NewCache == 100)
                SetMovieBoardText(string.Format("Playing - {0} ", CurrentStreamingitem.MediaTitle));
        }

        private void MediaPlayerService_EndReached()
        {
            OnMediaFinishedAction();
        }

        private void MediaPlayerService_EncounteredError()
        {
            OnStopAction();
        }

        private void MediaPlayerService_OnTimeChanged()
        {
            TimeChangeAction();
        }

        private void MediaPlayerService_OnStateChanged()
        {
            SetMovieBoardText(string.Format("{0} - {1}", MediaPlayerServices.State, CurrentStreamingitem.MediaTitle));
            IsPlaying = MediaPlayerServices.State == MovieMediaState.Playing ? true : false;
            if (IsPlaying) StartControlAnimation(); else ResetControlAnimation();
        }

        private void MediaPlayerService_OnMediaStopped()
        {
            OnStopAction();
        }

        private void MediaPlayerService_OnMediaOpened()
        {
            SetMediaControlDetails();

            if (_mediaMenu != null)
                _mediaMenu.Dispose();

            CurrentStreamingitem.IsActive = true;
            IsPlaying = true;

            StartControlAnimation();
            if (_allowMediaSizeEventExecute)
                MediaPlayerElementRaiseEvent(new MediaSizeChangedRoutedArgs(OnMediaSizeChangedEvent, this, MediaPlayerServices.PixelHeight, MediaPlayerServices.PixelWidth));

            this.MovieControl.MediaPlayerElementNotifyLastSeen(PlayableLastSeen);
        }

        private void MediaPlayerService_OnMediaOpening()
        {
            SetMovieBoardText(string.Format("Opening - {0}", CurrentStreamingitem.MediaTitle));
        }

        #endregion

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
           
        }

      

        #endregion

        internal void HookUpEvents()
        {
            _mediaMenu = new MediaMenu(this);

            HookUpControllerEvents();
           // _hasRegisteredEvents = true;
        }

        private void HookUpControllerEvents()
        {
            MovieControl.MouseEnter -= MovieControl_MouseEnter;
            MovieControl.MouseLeave -= MovieControl_MouseLeave;
            MovieControl.MouseEnter += MovieControl_MouseEnter;
            MovieControl.MouseLeave += MovieControl_MouseLeave;
            

            if (MovieControl.MovieControlSettings.IsMediaSliderEnabled && MovieControl.MediaSlider != null)
            {
                MovieControl.MediaSlider.ThumbDragStarted -= MediaSlider_ThumbDragStarted;
                MovieControl.MediaSlider.ThumbDragCompleted -= MediaSlider_ThumbDragCompleted;
                MovieControl.MediaSlider.ThumbDragStarted += MediaSlider_ThumbDragStarted;
                MovieControl.MediaSlider.ThumbDragCompleted += MediaSlider_ThumbDragCompleted;
            }
        }

        private void MovieControl_MouseLeave(object sender, MouseEventArgs e)
        {
            _canAnimateControl = true;
        }

        private void MovieControl_MouseEnter(object sender, MouseEventArgs e)
        {
            _canAnimateControl = false;
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
            MediaPlayerServices.CurrentTimer = TimeSpan.FromSeconds(MovieControl.MediaSlider.Value);
            _isDragging = false;
        }

        private void MediaSlider_ThumbDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _isDragging = true;
            //stop animation
        }

        private void _mediaelementregion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(this.MediaPlayerViewType == MediaPlayerViewType.FullMediaPanel)
                WindowsFullScreenAction();
        }
        
        private void UnloadComponents()
        {
            if (!AllowMediaPlayerAutoDispose || MediaPlayerServices == null)
                return;

            if (!MediaPlayerServices.HasStopped)
                MediaPlayerServices.Stop();

            MediaPlayerServices.Dispose();
            BindingOperations.ClearAllBindings(this);
            _current = null;
            //CurrentStreamingitem.IsActive = false;
            //PlayableLastSeen = null;
            #region Useless
            //MovieControl.Dispose();
            //_mediaMenu = null;
            //MediaPlayerService = null;
            //MovieControl = null;


            #endregion
        }

        internal void MediaPlayerElementRaiseEvent(RoutedEventArgs _event)
        {
            RaiseEvent(_event);
        }

        internal void MediaPlayerElementRaiseEvent(RoutedEvent _event)
        {
            RaiseEvent(new RoutedEventArgs(_event, this));
        }

        private void SetMediaControlDetails()
        {
            this.MovieControl.MediaDetailProps.MediaDuration = MediaPlayerServices.Duration.TotalSeconds;
            this.MovieControl.SetMediaVolume(MovieControl.VolumeControl.VolumeLevel);
        }
        
        private void TimeChangeAction()
        {
            if (!_isDragging)
            {
                MovieControl.MediaDetailProps.CurrentMediaTime = MediaPlayerServices.CurrentTimer.TotalSeconds;
                PlayableLastSeen.Progress = Math.Round(((MovieControl.MediaDetailProps.CurrentMediaTime / MovieControl.MediaDetailProps.MediaDuration) * 100), 2);
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
            MovieControl.MediaDetailProps.CurrentMediaTime = 0.0;
        }

        internal void ControlViewChangeCommandAction()
        {
            // MovieControl.ControlViewType = MovieControl.ControlViewType == MediaControlViewType.LargeView ? MediaControlViewType.MiniView : MediaControlViewType.LargeView;
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

            CheckForRepeat();
            
        }

        private void CheckForRepeat()
        {
            if (!IscheckingForRepeating)
            {
                IscheckingForRepeating = true;
                if (PlaylistManager != null && MovieControl.MediaDetailProps.RepeatMode != RepeatMode.NoRepeat)
                    MediaPlayerUtil.ExecuteTimerAction(() => StartRepeatAction(), 50);
                else
                    IscheckingForRepeating = false;
            }
        }

        private void StartRepeatAction()
        {
            IscheckingForRepeating = false;

            if (MovieControl.MediaDetailProps.RepeatMode == RepeatMode.RepeatOnce)
            {
                Source(CurrentStreamingitem);
                return;
            }

            var vfc = PlaylistManager.GetNextItem();
            if (vfc != null)
                this.Source(vfc);
        }

        internal void ReWindFastForward()
        {
            if (!_isrewindorfastforward)
            {
                _isrewindorfastforward = true;

                if (VolumeState == VolumeState.Active)
                {
                    MediaPlayerServices.ToggleMute();
                }
                ResetControlAnimation();
            }
        }

        internal void RestoreRewindFastforwardSettings()
        {
            if (VolumeState == VolumeState.Active)
            {
                MediaPlayerServices.ToggleMute();
            }
            _isrewindorfastforward = false;

            StartControlAnimation();
        }

        internal void RepeatModeAction()
        {
            if (MovieControl.MediaDetailProps.RepeatMode == RepeatMode.NoRepeat)
            {
                MovieControl.MediaDetailProps.RepeatMode = RepeatMode.Repeat;
            }
            else
            if (MovieControl.MediaDetailProps.RepeatMode == RepeatMode.Repeat)
            {
                MovieControl.MediaDetailProps.RepeatMode = RepeatMode.RepeatOnce;
            }
            else
            if (MovieControl.MediaDetailProps.RepeatMode == RepeatMode.RepeatOnce)
            {
                MovieControl.MediaDetailProps.RepeatMode = RepeatMode.NoRepeat;
            }
        }

        internal void MuteAction()
        {
            if (!MediaPlayerServices.IsMute)
            {
                MediaPlayerServices.ToggleMute();
                //MovieControl.VolumeControl.IsEnabled = false;
                VolumeState = VolumeState.Muted;
            }
            else
            {
                MediaPlayerServices.ToggleMute();
               // MovieControl.VolumeControl.IsEnabled = true;
                VolumeState = VolumeState.Active;
            }
        }

        internal void PreviousAction()
        {
            if (!MediaPlayerServices.HasStopped)
                MediaPlayerServices.Stop();
            Source(PlaylistManager.GetPreviousItem());
        }

        internal void NextAction()
        {
            if (!MediaPlayerServices.HasStopped)
                MediaPlayerServices.Stop();
            Source(PlaylistManager.GetNextItem());
        }

        internal void WindowsFullScreenAction()
        {
            WindowFullScreenState = WindowFullScreenState == WindowFullScreenState.Normal ? WindowFullScreenState.FullScreen : WindowFullScreenState.Normal;
            var routedevent = new WindowFullScreenRoutedEventArgs(OnFullScreenButtonToggleEvent, this, WindowFullScreenState);
            this.MediaPlayerElementRaiseEvent(routedevent);
        }

        private void SetStretchProperty(Stretch newValue)
        {
            MediaPlayerServices.VideoStretch = newValue;
        }

        internal void MinimizeControlAction()
        {
            if (this.MediaPlayerViewType == MediaPlayerViewType.FullMediaPanel)
            {
                SaveMediaDefinitedSettings();
                if (_isPlaylistVisible)
                    TogglePlaylistView();

                if(_savedSecondaryControl != null)
                {
                    _savedSecondaryControl.SetControlSettings(this.MovieControl.MovieControlSettings);
                    this.UseSecondaryControl = _savedSecondaryControl;
                }
                else
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
            if(_savedSecondaryControl == null)
                this.MovieControl.ControlViewType = _savedSettings.MovieControlViewType;

            this.MediaPlayerViewType = _savedSettings.MediaPlayerMode;
            this.IsMediaContextMenuEnabled = _savedSettings.AllowContextMenu;
            this._allowMediaSizeEventExecute = _savedSettings.AllowMediaResize;
            this.MediaStretch = _savedSettings.stretchValue;
            this.IsMediaContextMenuEnabled = _savedSettings.IsContextMenuEnabled;
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
            _savedSettings.IsContextMenuEnabled = IsMediaContextMenuEnabled;
        }

        internal void WindowsClosedAction()
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

        internal void PreviewControl()
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

        internal void PauseOrPlayAction(bool igonreMediaState = false)
        {
            if (igonreMediaState)
            {
                MediaPlayerServices.Play();
                return;
            }

            if (MediaPlayerServices.State == MovieMediaState.Playing)
            {
                MediaPlayerServices.PauseOrResume();
                IsPlaying = false;
            }
            else if (MediaPlayerServices.State == MovieMediaState.Ended ||
                MediaPlayerServices.State == MovieMediaState.Paused ||
                MediaPlayerServices.State == MovieMediaState.Stopped)
            {
                if (MediaPlayerServices.State == MovieMediaState.Ended)
                {
                    this.Source(CurrentStreamingitem);
                    return;
                }
                MediaPlayerServices.Play();
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

            if (!MediaPlayerServices.HasStopped)
                MediaPlayerServices.Stop();

            this.CurrentStreamingitem = file_to_pay;
            if (file_to_pay.Url == null)
                throw new NotImplementedException("No media Url");

            if (!_hasInitialised)
                throw new Exception("MediaPlayerElement not Initialized");

            (MediaPlayerServices as MediaPlayerService).LoadMedia(file_to_pay.Url);

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
