using System;
using System.Windows.Input;
using System.Windows.Threading;
using Movies.Enums;
using Movies.MediaService.Models;

namespace Movies.MediaService.Interfaces
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
        object VideoPlayer { get; }
        int Volume { get; set; }
        string[] VlcOption { get; set; }
        string MediaError { get; }


        AudioTracksManagement AudioTracksManagement { get; }
        ChapterManagement ChapterManagement { get; }
        SubtitleManagement SubtitleManagement { get; }
        VideoAdjustManagement VideoAdjustManagement { get; }

        VideoTracksManagement VideoTracksManagement { get; }

        event EventHandler Buffering;
        event EventHandler EncounteredError;
        event EventHandler EndReached;
        event MouseEventHandler MouseMove;
        event EventHandler OnDurationChanged;
        event EventHandler OnMediaChanged;
        event EventHandler OnMediaMouseEnter;
        event EventHandler OnMediaMouseLeave;
        event EventHandler OnMediaOpened;
        event EventHandler OnMediaOpening;
        event EventHandler OnMediaPaused;
        event EventHandler OnMediaPlaying;
        event EventHandler OnMediaStopped;
        event EventHandler OnStateChanged;
        event EventHandler OnSubItemAdded;
        event EventHandler OnTimeChanged;
        event EventHandler SubItemChanged;
        event EventHandler TitleChanged;

        void LoadMedia(string filepath, string[] option = null);
        void LoadMedia(Uri UrlPath, string[] option = null);
        void Pause();
        void PauseOrResume();
        void Play();
        void SetSubtitle(int subtitle_id);
        void Stop();
        void ToggleMute();
        void Dispose();
    }
}