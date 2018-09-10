﻿using Meta.Vlc.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Movies.MediaService.Interfaces;
using Movies.MediaService.Models;
using Movies.Enums;
using System.Reflection;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;

namespace Movies.MediaService.Service
{
    public partial class MediaPlayerService : FrameworkElement, IMediaPlayerService
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

        public event EventHandler OnMediaOpened;
        public event EventHandler OnSubItemAdded;
        public event EventHandler OnTimeChanged;
        public event EventHandler OnMediaStopped;
        public event EventHandler OnStateChanged;
        public event EventHandler EndReached;
        public event EventHandler EncounteredError;
        public event EventHandler OnMediaMouseEnter;
        public event EventHandler<MediaBufferingEventArgs> Buffering;
        public event EventHandler OnDurationChanged;
        public event EventHandler OnMediaChanged;
        public event EventHandler OnMediaOpening;
        public event EventHandler OnMediaPaused;
        public event EventHandler OnMediaPlaying;
        public event EventHandler SubItemChanged;
        public event EventHandler OnMediaMouseLeave;
        public event EventHandler TitleChanged;
        public event MouseEventHandler MouseMove;
        

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

        public object VideoPlayer { get { return _vlcPlayer; } }

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

        public MediaPlayerService()
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


        public void LoadMedia(string filepath, string[] option = null)
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

        public void LoadMedia(Uri UrlPath, string[] option = null)
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
