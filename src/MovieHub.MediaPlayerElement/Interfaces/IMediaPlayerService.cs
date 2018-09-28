using System;
using System.Windows.Input;
using System.Windows.Threading;
using Movies.Enums;
using MovieHub.MediaPlayerElement.Models;
using System.Windows;
using System.Windows.Media;

namespace MovieHub.MediaPlayerElement.Interfaces
{
    public interface IMediaPlayerService
    {
        bool CanPause { get; }
        bool IsDisposed { get; }
        bool CanPlay { get; }
        TimeSpan CurrentTimer { get; set; }
        bool HasVideo { get; }
        bool HasStopped { get; }
        bool IsMute { get; set; }
        bool IsSeekable { get; }
        double PixelHeight { get; }
        double PixelWidth { get; }
        float Rate { get; }
        MovieMediaState State { get; }
        Dispatcher MediaDispatcher { get; }
        TimeSpan Duration { get; }
        //object VideoPlayer { get; }
        int Volume { get; set; }
        string[] VlcOption { get; set; }
        string MediaError { get; }
        double FPS { get; }
        Stretch VideoStretch { get; set; }

        AudioTracksManagement AudioTracksManagement { get; }
        ChapterManagement ChapterManagement { get; }
        SubtitleManagement SubtitleManagement { get; }
        VideoAdjustManagement VideoAdjustManagement { get; }

        VideoTracksManagement VideoTracksManagement { get; }
        bool HasLoadedMedia { get; }

        event EventHandler<MediaBufferingEventArgs> Buffering;
        event RoutedEventHandler EncounteredError;
        event RoutedEventHandler EndReached;
        event MouseEventHandler MouseMove;
        event RoutedEventHandler OnDurationChanged;
        event RoutedEventHandler OnMediaChanged;
        event RoutedEventHandler OnMediaMouseEnter;
        event RoutedEventHandler OnMediaMouseLeave;
        event RoutedEventHandler OnMediaOpened;
        event RoutedEventHandler OnMediaOpening;
        event RoutedEventHandler OnMediaPaused;
        event RoutedEventHandler OnMediaPlaying;
        event RoutedEventHandler OnMediaStopped;
        event RoutedEventHandler OnStateChanged;
        event RoutedEventHandler OnSubItemAdded;
        event RoutedEventHandler OnTimeChanged;
        event RoutedEventHandler SubItemChanged;
        event RoutedEventHandler TitleChanged;
        
        void Pause();
        void PauseOrResume();
        void Play();
        void SetSubtitle(int subtitle_id);
        void Stop();
        void ToggleMute();
        void Dispose();
    }
}