using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
using Movies.Enums;
using Movies.Models.Model;
using Movies.MoviesInterfaces;

namespace VideoPlayerControl.ViewModel
{
    public interface IMediaControllerViewModel2
    {
        bool AllowAutoResize { get; set; }
        bool CanAnimate { get; set; }
        DelegateCommand CloseLastSeenCommand { get; }
        VideoFolderChild CurrentVideoItem { get; }
        ExecuteCommand CurrentVideoItemChangedEvent { get; set; }
        IDispatcherService DispatcherService { get; }
        Slider DragPositionSlider { get; }
        bool HaslastSeen { get; set; }
        IMovieTitleBar IMovieTitle_Tab { get; }
        bool IsDragging { get; set; }
        bool IsfetchingRepeatItemAsync { get; }
        bool IsFullScreenMode { get; set; }
        bool IsMouseControlOver { get; set; }
        bool IsPlaying { get; set; }
        bool IsRewindOrFastForward { get; set; }
        IVideoElement IVideoElement { get; }
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
        SCREENSETTINGS ScreenSetting { get; set; }
        DelegateCommand SetLastSeenCommand { get; }
        DelegateCommand<Popup> ShowMenuCommand { get; }
        DelegateCommand ToFullScreenBtn { get; }
        Slider VolumeSlider { get; }
        VolumeState VolumeState { get; set; }

        event EventHandler SubtitleChanged;

        bool CanNext();
        bool CanPlay();
        bool CanPrev();
        void CloseMediaPlayer(bool wndClose = false);
        void RegisterCommandOnlyEnable();
        void FullScreenAction();
        void FullScreenSettings();
        object GetControllerNewView();
        IControllerView GetControllerView();
        double GetMousePointer(Control obj);
        void GetVideoItem(VideoFolderChild obj, bool frompl = false);
        void Loaded();
        void MainControl_MouseLeave(object sender, MouseEventArgs e);
        void MediaController_Loaded(object sender, RoutedEventArgs e);
        void MediaPlayStopAction();
        void MuteAction();
        void NextPlayAction();
        void NormalScreenSettings();
        void OnDrop(DragEventArgs e);
        void OnMouseDoubleClick(MouseButtonEventArgs e);
        void PlayAction(bool igonreMediaState = false);
        void PositionSlideTimerTooltipStop();
        void PrevPlayAction();
        void RestoreMediaState();
        void SetSubtitle(string filepath);
        void TimeChangeAction();
        void VisibilityAnimation();
    }
}