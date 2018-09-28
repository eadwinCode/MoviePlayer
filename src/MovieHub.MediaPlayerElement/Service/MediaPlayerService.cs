using Meta.Vlc.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MovieHub.MediaPlayerElement.Interfaces;
using MovieHub.MediaPlayerElement.Models;
using Movies.Enums;
using System.Reflection;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace MovieHub.MediaPlayerElement.Service
{
    public sealed partial class MediaPlayerService : FrameworkElement, IMediaPlayerService
    {
        private readonly DirectoryInfo vlcLibDirectory;
        private VlcPlayer _vlcPlayer;
        private AudioTracksManagement audiotracksmanagement;
        private VideoAdjustManagement videoadjustmanagement; 
        private VideoTracksManagement videotracksmanagement; 
        private SubtitleManagement subtitlemanagement;
        private ChapterManagement chaptermanagement;
        private MovieMediaState state = MovieMediaState.NothingSpecial;
        private static bool MediaplayerVlclibLoaded = false;
        private static object _lock = new object();
        
        #region Events
        public event RoutedEventHandler OnMediaMouseEnter;
        public event RoutedEventHandler OnMediaMouseLeave;
        public event RoutedEventHandler TitleChanged;
        /// <summary>
        /// OnTimeChanged is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnTimeChangedEvent =
           EventManager.RegisterRoutedEvent(
                           "OnTimeChanged",
                           RoutingStrategy.Bubble,
                           typeof(RoutedEventHandler),
                           typeof(MediaPlayerService));

        /// <summary>
        /// Raised OnTimeChanged.
        /// </summary>
        public event RoutedEventHandler OnTimeChanged
        {
            add { AddHandler(OnTimeChangedEvent, value); }
            remove { RemoveHandler(OnTimeChangedEvent, value); }
        }

        /// <summary>
        /// OnMediaStopped is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnMediaStoppedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnMediaStopped",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised On MediaStopped.
        /// </summary>
        public event RoutedEventHandler OnMediaStopped
        {
            add { AddHandler(OnMediaStoppedEvent, value); }
            remove { RemoveHandler(OnMediaStoppedEvent, value); }
        }

        /// <summary>
        /// OnStateChanged is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnStateChangedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnStateChanged",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised On media StateChanged.
        /// </summary>
        public event RoutedEventHandler OnStateChanged
        {
            add { AddHandler(OnStateChangedEvent, value); }
            remove { RemoveHandler(OnStateChangedEvent, value); }
        }

        /// <summary>
        /// EndReached is a routed event.
        /// </summary>
        public static readonly RoutedEvent EndReachedEvent =
          EventManager.RegisterRoutedEvent(
                          "EndReached",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised on Media ended.
        /// </summary>
        public event RoutedEventHandler EndReached
        {
            add { AddHandler(EndReachedEvent, value); }
            remove { RemoveHandler(EndReachedEvent, value); }
        }

        /// <summary>
        /// EncounteredError is a routed event.
        /// </summary>
        public static readonly RoutedEvent EncounteredErrorEvent =
          EventManager.RegisterRoutedEvent(
                          "EncounteredError",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised EncounteredError on media.
        /// </summary>
        public event RoutedEventHandler EncounteredError
        {
            add { AddHandler(EncounteredErrorEvent, value); }
            remove { RemoveHandler(EncounteredErrorEvent, value); }
        }

        /// <summary>
        /// Buffering is a routed event.
        /// </summary>
        public static readonly RoutedEvent BufferingEvent =
          EventManager.RegisterRoutedEvent(
                          "Buffering",
                          RoutingStrategy.Bubble,
                          typeof(EventHandler<MediaBufferingEventArgs>),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised Buffering.
        /// </summary>
        public event EventHandler<MediaBufferingEventArgs> Buffering
        {
            add { AddHandler(BufferingEvent, value); }
            remove { RemoveHandler(BufferingEvent, value); }
        }

        /// <summary>
        /// OnDurationChanged is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnDurationChangedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnDurationChanged",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised On DurationChanged.
        /// </summary>
        public event RoutedEventHandler OnDurationChanged
        {
            add { AddHandler(OnDurationChangedEvent, value); }
            remove { RemoveHandler(OnDurationChangedEvent, value); }
        }

        /// <summary>
        /// OnMediaChanged is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnMediaChangedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnMediaChanged",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised On MediaChanged.
        /// </summary>
        public event RoutedEventHandler OnMediaChanged
        {
            add { AddHandler(OnMediaChangedEvent, value); }
            remove { RemoveHandler(OnMediaChangedEvent, value); }
        }
        /// <summary>
        /// OnMediaOpening is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnMediaOpeningEvent =
          EventManager.RegisterRoutedEvent(
                          "OnMediaOpening",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised On MediaOpening.
        /// </summary>
        public event RoutedEventHandler OnMediaOpening
        {
            add { AddHandler(OnMediaOpeningEvent, value); }
            remove { RemoveHandler(OnMediaOpeningEvent, value); }
        }
        /// <summary>
        /// OnMediaPaused is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnMediaPausedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnMediaPaused",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised On MediaPaused.
        /// </summary>
        public event RoutedEventHandler OnMediaPaused
        {
            add { AddHandler(OnMediaPausedEvent, value); }
            remove { RemoveHandler(OnMediaPausedEvent, value); }
        }
        /// <summary>
        /// OnMediaPlaying is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnMediaPlayingEvent =
          EventManager.RegisterRoutedEvent(
                          "OnMediaPlaying",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised On MediaPlaying.
        /// </summary>
        public event RoutedEventHandler OnMediaPlaying
        {
            add { AddHandler(OnMediaPlayingEvent, value); }
            remove { RemoveHandler(OnMediaPlayingEvent, value); }
        }
        /// <summary>
        /// SubItemChanged is a routed event.
        /// </summary>
        public static readonly RoutedEvent SubItemChangedEvent =
          EventManager.RegisterRoutedEvent(
                          "SubItemChanged",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised on SubItemChanged.
        /// </summary>
        public event RoutedEventHandler SubItemChanged
        {
            add { AddHandler(SubItemChangedEvent, value); }
            remove { RemoveHandler(SubItemChangedEvent, value); }
        }

        /// <summary>
        /// OnSubItemAdded is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnSubItemAddedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnSubItemAdded",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised On SubItemAdded.
        /// </summary>
        public event RoutedEventHandler OnSubItemAdded
        {
            add { AddHandler(OnSubItemAddedEvent, value); }
            remove { RemoveHandler(OnSubItemAddedEvent, value); }
        }

        /// <summary>
        /// OnMediaOpened is a routed event.
        /// </summary>
        public static readonly RoutedEvent OnMediaOpenedEvent =
          EventManager.RegisterRoutedEvent(
                          "OnMediaOpened",
                          RoutingStrategy.Bubble,
                          typeof(RoutedEventHandler),
                          typeof(MediaPlayerService));

        /// <summary>
        /// Raised On MediaOpened.
        /// </summary>
        public event RoutedEventHandler OnMediaOpened
        {
            add { AddHandler(OnMediaOpenedEvent, value); }
            remove { RemoveHandler(OnMediaOpenedEvent, value); }
        } 
        #endregion
        
        public string  MediaError
        {
	        get { return "Failed to open media file"; }
        }
        public TimeSpan CurrentTimer { get { return _vlcPlayer.Time; } set { _vlcPlayer.Time = value; } }

        public TimeSpan Duration { get { return _vlcPlayer.Length; } }

        public bool HasVideo { get { return _vlcPlayer.VlcMediaPlayer.HasVideo; } }

        public bool IsDisposed { get { return _isdispoed; } }

        public bool CanPlay { get { return _vlcPlayer.VlcMediaPlayer.CanPlay; } }

        public bool CanPause { get { return _vlcPlayer.VlcMediaPlayer.HasVideo; } }
        
        public float Rate { get { return _vlcPlayer.Rate; } }

        public MovieMediaState State { get { return state; } }
        
        public int Volume { get { return _vlcPlayer.Volume; }
            set { _vlcPlayer.Volume = value; } }

        public bool IsSeekable { get { return _vlcPlayer.IsSeekable; } }

        public bool IsMute { get { return _vlcPlayer.IsMute; }
            set { _vlcPlayer.IsMute = value; } }
        
        public double PixelHeight { get { return _vlcPlayer.VlcMediaPlayer.PixelHeight; } }

        public double PixelWidth { get { return _vlcPlayer.VlcMediaPlayer.PixelWidth; } }

        public AudioTracksManagement AudioTracksManagement { get { return audiotracksmanagement; } }
       
        public VideoAdjustManagement VideoAdjustManagement { get { return videoadjustmanagement; } }
        public VideoTracksManagement VideoTracksManagement { get { return videotracksmanagement; } }
        
        public SubtitleManagement SubtitleManagement { get { return subtitlemanagement; } }
        
        public ChapterManagement ChapterManagement { get { return chaptermanagement; } }
        
        public Stretch VideoStretch
        {
            get { return _vlcPlayer.Stretch; }
            set { _vlcPlayer.Stretch = value; }
        }


        internal VlcPlayer VideoPlayer { get { return _vlcPlayer; } }

        public bool HasStopped
        {
            get { return this.State == MovieMediaState.Ended || this.State == MovieMediaState.NothingSpecial || this.state == MovieMediaState.Stopped; }
        }

        public Dispatcher MediaDispatcher
        {
            get { return this.Dispatcher; }
        }
        string[] vlcoption = {
            "-I", "dummy", "--ignore-config", "--no-video-title","--verbose=2"
        };
        private bool _isdispoed = false;

        public string[] VlcOption { get { return vlcoption; } set { vlcoption = value; } }

        public double FPS
        {
            get { return _vlcPlayer.FPS; }
        }
        bool hasloadedmedia = MediaplayerVlclibLoaded;


        public bool HasLoadedMedia { get { return hasloadedmedia; } }

        internal MediaPlayerService()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            vlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "LibVlc",
                IntPtr.Size == 4 ? "win-x86" : "win-x86"));
            this.Dispatcher.Invoke(DispatcherPriority.Background,new Action(() => { 
                _vlcPlayer = new VlcPlayer(true)
                {
                    EndBehavior = EndBehavior.Nothing,
                    Stretch = System.Windows.Media.Stretch.Uniform
                    ,VlcOption = VlcOption,
                    LibVlcPath = vlcLibDirectory.FullName
                };
            }));
           
        }
        
        private void InitializeComponent()
        {
            audiotracksmanagement = new AudioTracksManagement(_vlcPlayer);
            audiotracksmanagement.LoadData();
            videoadjustmanagement = new VideoAdjustManagement(_vlcPlayer);
            subtitlemanagement = new SubtitleManagement(_vlcPlayer,this);
            subtitlemanagement.ReloadData();
            chaptermanagement = new ChapterManagement(_vlcPlayer);

            videotracksmanagement = new VideoTracksManagement(_vlcPlayer);
            videotracksmanagement.LoadData();
        }
        
        private MovieMediaState GetMediaState()
        {
            switch (_vlcPlayer.VlcMediaPlayer.State)
            {
                case Meta.Vlc.Interop.Media.MediaState.NothingSpecial:
                    return MovieMediaState.NothingSpecial;
                case Meta.Vlc.Interop.Media.MediaState.Opening:
                    return MovieMediaState.Opening;
                case Meta.Vlc.Interop.Media.MediaState.Buffering:
                    return MovieMediaState.Buffering;
                case Meta.Vlc.Interop.Media.MediaState.Playing:
                    return MovieMediaState.Playing;
                case Meta.Vlc.Interop.Media.MediaState.Paused:
                    return MovieMediaState.Paused;
                case Meta.Vlc.Interop.Media.MediaState.Stopped:
                    return MovieMediaState.Stopped;
                case Meta.Vlc.Interop.Media.MediaState.Ended:
                    return MovieMediaState.Ended;
                case Meta.Vlc.Interop.Media.MediaState.Error:
                    return MovieMediaState.Error;

                default:
                    return MovieMediaState.NothingSpecial;
            }
        }


        internal void LoadMedia(string filepath, string[] option = null)
        {
            lock (_lock)
            {
                if (!MediaplayerVlclibLoaded)
                {
                    DispatcherTimer ActionTimer = new DispatcherTimer(DispatcherPriority.Background)
                    {
                        Interval = TimeSpan.FromMilliseconds(2000)
                    };
                    ActionTimer.Tick += (s, e) => {
                        ActionTimer.Stop();
                        this.MediaPlayerServiceLoadMedia(filepath, option);
                        hasloadedmedia = true;
                    };
                    MediaplayerVlclibLoaded = true;
                    ActionTimer.Start();
                    return;
                }

                this.MediaPlayerServiceLoadMedia(filepath, option);
            }
        }

        void MediaPlayerServiceLoadMedia(string filepath, string[] option = null)
        {
            if (option == null)
                _vlcPlayer.LoadMedia(filepath);
            else
                _vlcPlayer.LoadMediaWithOptions(filepath, option);

            // InitializeComponent();
            HookUpEvents();

            Play();
        }

        void MediaPlayerServiceLoadMedia(Uri UrlPath, string[] option = null)
        {
            if (option == null)
                _vlcPlayer.LoadMedia(UrlPath);
            else
                _vlcPlayer.LoadMediaWithOptions(UrlPath, option);

            // InitializeComponent();
            HookUpEvents();

            Play();
        }

        internal void LoadMedia(Uri UrlPath, string[] option = null)
        {
            lock (_lock)
            {
                if (!MediaplayerVlclibLoaded)
                {
                    DispatcherTimer ActionTimer = new DispatcherTimer(DispatcherPriority.Background)
                    {
                        Interval = TimeSpan.FromMilliseconds(2000)
                    };
                    ActionTimer.Tick += (s, e) => {
                        ActionTimer.Stop();
                        this.MediaPlayerServiceLoadMedia(UrlPath, option);
                        hasloadedmedia = true;
                    };
                    ActionTimer.Start();
                    MediaplayerVlclibLoaded = true;
                    return;
                }

                this.MediaPlayerServiceLoadMedia(UrlPath, option);
            }
        }

        public void Pause()
        {
            _vlcPlayer.Pause();
        }

        public void PauseOrResume()
        {
            _vlcPlayer.PauseOrResume();
        }

        public void Play()
        {
            _vlcPlayer.Play();
        }

        public void Stop()
        {
            CleanManagements();
            _vlcPlayer.Stop();
        }

        private void CleanManagements()
        {
            audiotracksmanagement = null;
            videoadjustmanagement = null;
            subtitlemanagement = null;
            chaptermanagement = null;
        }

        public void SetSubtitle(int subtitle_id)
        {
            
        }

        public void ToggleMute()
        {
            _vlcPlayer.ToggleMute();
        }

        public void Dispose()
        {
            _vlcPlayer.Dispose();
            ApiManager.ReleaseAll();
            _isdispoed = true;
        }
    
    }
}
