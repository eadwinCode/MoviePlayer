using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using MovieHub.MediaPlayerElement.Interfaces;
using MovieHub.MediaPlayerElement.Models;
using Movies.Enums;
using Movies.Models.Interfaces;

namespace MovieHub.MediaPlayerElement.Interfaces
{
    public interface IMediaPlayerElement
    {
        bool AllowMediaPlayerAutoDispose { get; set; }
        bool AllowMovieControlAnimation { get; set; }
        bool CanEscapeKeyCloseMedia { get; set; }
        bool CanMediaFastForwardOrRewind { get; set; }
        bool IsCloseButtonVisible { get; set; }
        bool IsMediaContextMenuEnabled { get; set; }
        bool IsPlaying { get; }
        IMediaPlayerService MediaPlayerServices { get; }
        MediaPlayerViewType MediaPlayerViewType { get; set; }
        Stretch MediaStretch { get; set; }
        string MediaTitle { get; }
        MovieControl MovieControl { get; }
        MoviesPlaylistManager PlaylistManager { get; }
        VolumeState VolumeState { get; }
        WindowFullScreenState WindowFullScreenState { get; }

        event RoutedEventHandler OnCloseWindowToggled;
        event EventHandler<WindowFullScreenRoutedEventArgs> OnFullScreenButtonToggle;
        event EventHandler<MediaInfoChangedEventArgs> OnMediaInfoChanged;
        event EventHandler<MediaSizeChangedRoutedArgs> OnMediaSizeChanged;
        event RoutedEventHandler OnMediaTitleChanged;
        event EventHandler<MediaPlayerViewTypeRoutedEventArgs> OnMinimizedControlExecuted;
        event RoutedEventHandler SetWindowTopMostProperty;

        void AddRangeToPlaylist(IEnumerable<IPlayable> EnumerableVfc);
        void AddToPlaylist(IPlayable vfc);
        void Dispose();
        void OnApplyTemplate();
        void Source(IPlayable file_to_pay);
        void Source(IPlayable playFile, IEnumerable<IPlayable> TemperalList);
        void Source(IPlaylistModel plm);
        void Source(Uri file_to_pay);
    }
}