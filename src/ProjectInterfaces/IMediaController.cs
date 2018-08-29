using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using Meta.Vlc.Wpf;
using System.Windows.Threading;
using Movies.MediaService.Interfaces;

namespace Movies.MoviesInterfaces
{
    public delegate void ExecuteCommand(object sender, bool frompl);
    public delegate void AddPlaylistCollectionHandler(IPlaylistModel ipl, bool addtomovieplaylist = true);

    public interface IMediaControllerViewModel
    {
        bool CanAnimate { get; set; }
        DelegateCommand CloseLastSeenCommand { get; }
        VideoFolderChild CurrentVideoItem { get; }
        ExecuteCommand CurrentVideoItemChangedEvent { get; set; }
        Slider DragPositionSlider { get; }
        bool HaslastSeen { get; set; }
        IMovieTitle_Tab IMovieTitle_Tab { get; }
        bool IsDragging { get; set; }
        bool IsMouseControlOver { get; set; }
        bool IsPlaying { get; set; }
        bool IsRewindOrFastForward { get; set; }
        IVideoElement IVideoElement { get; }
        IMediaController IVideoPlayer { get; }
        TimeSpan LastSeenTime { get; set; }
        MovieMediaState MediaState { get; }
        DelegateCommand Mute { get; set; }
        DelegateCommand Next { get; }
        DelegateCommand PlayBtn { get; set; }
        string PlayText { get; set; }
        DispatcherTimer PositionSlideTimerTooltip { get; set; }
        DelegateCommand Previous { get; }
        DelegateCommand RepeatBtn { get; }
        RepeatMode RepeatMode { get; set; }
        DelegateCommand SetLastSeenCommand { get; }
        DelegateCommand ToFullScreenBtn { get; }
        Slider VolumeSlider { get; }
        VolumeState VolumeState { get; set; }
        bool IsfetchingRepeatItemAsync { get; }
        event PropertyChangedEventHandler PropertyChanged;
        event EventHandler SubtitleChanged;

        bool CanNext();
        bool CanPlay();
        bool CanPrev();
        void CloseMediaPlayer(bool wndClose = false);
        double GetMousePointer(Control obj);
        void GetVideoItem(VideoFolderChild obj, bool frompl = false);
        void MainControl_MouseLeave(object sender, MouseEventArgs e);
        void MediaController_Loaded(object sender, RoutedEventArgs e);
        void MediaPlayStopAction();
        void MuteAction();
        void NextPlayAction();
        void PlayAction(bool igonreMediaState = false);
        void PositionSlideTimerTooltipStop();
        void PrevPlayAction();
        void SetSubtitle(string filepath);
        void TimeChangeAction();
    }


    public interface IMovieTitle_Tab
    {
        bool IsCanvasDrag { get; set; }
        bool IsTextTrimmed { get; set; }
        double MarqueeTimeInSeconds { get; set; }
        string MovieText { get; set; }
        string MovieTitleText { get; set; }
        Visibility ShowOtherText { get; }
        TextBlock TextMovieTitle { get; }
        Window WindowMovieTitle { get; }

        event PropertyChangedEventHandler PropertyChanged;
    }
    public interface IVideoElement
    {
        CommandBindingCollection CommandBindings { get; }
        IPlaylistViewMediaPlayerView PlayListView { get; }
        IMediaController IVideoPlayerController { get; }
        //UIElement WindowsTab { get; }
        //UIElement WindowsTabDock { get; }
        // VlcPlayer MediaPlayer { get; }
        ContentControl ContentDockRegion { get; }
        UIElement ParentGrid { get; }
        string Title { get; set; }
        void SetTopMost();
    }

    public interface IControllerView
    {
        IMovieTitle_Tab MovieTitle_Tab { get; }
        Panel GroupedControls { get; }
        //VolumeControl VolumeControl { get; }
    }



    public interface IMediaController
    {
        CommandBindingCollection CommandBindings { get; }

        // MediaUriElement MediaPlayer { get; }
        //Canvas CanvasEnvironment { get; }
        //ISubtitle Subtitle { get; }
        IControllerView MediaController { get; }
        event EventHandler ScreenSettingsChanged;
    }

   
}