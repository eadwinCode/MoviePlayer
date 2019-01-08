using Common.ApplicationCommands;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using MovieHub.MediaPlayerElement;
using MovieHub.MediaPlayerElement.Models;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace VideoPlayerView
{
    public sealed class MediaPlayerWindow : MovieBase
    {
        private MediaPlayerWindowView mediaplayerwindowview;
        private MediaPlayerElement mediaplayerelement;
        private IPlayable CurrentPlayingItem;
        private WindowState _previousState;
        private IHomeControl _defaultPlayerControl;
        private MediaPlayerInfoView _mediaPlayerInfo;

        private IShellWindowService ShellWindowService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShellWindowService>();
            }
        }

        IPageNavigatorHost PageNavigatorHost
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPageNavigatorHost>();
            }
        }

        IEventManager EventManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventManager>();
            }
        }

        IDispatcherService DispatcherService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDispatcherService>();
            }
        }

        private MetroWindow GetWindow
        {
            get { return ShellWindowService.ShellWindow; }
        }

        public MediaPlayerElement MediaPlayerElement
        {
            get { return mediaplayerelement; }
            private set { mediaplayerelement = value;RaisePropertyChanged(() => this.MediaPlayerElement); }
        }
        
        private DelegateCommand closemediaplayerwindow;

        public DelegateCommand CloseMediaPlayerWindow
        {
            get
            {
                if(closemediaplayerwindow == null)
                {
                    closemediaplayerwindow = new DelegateCommand(() =>
                    DispatcherService.ExecuteTimerAction(() =>
                        ClosePlayerActionFromPage(), 50));
                }
                return closemediaplayerwindow;
            }
        }

        private bool ismediaplayerwindowenabled = false;

        public bool IsMediaPlayerWindowEnabled
        {
            get { return ismediaplayerwindowenabled; }
            private set { ismediaplayerwindowenabled = value; RaisePropertyChanged(() => this.IsMediaPlayerWindowEnabled); }
        }

        private MediaInformation _mediainformation;

        public MediaInformation MediaInformation
        {
            get { return _mediainformation; }
            private set
            {
                _mediainformation = value;
                HasMediaInfo = value != null && value.NowPlaying != null ? true : false;
                RaisePropertyChanged(() => this.MediaInformation);
            }
        }

        private bool _hasmediainfo;

        public bool HasMediaInfo
        {
            get { return _hasmediainfo; }
            private set { _hasmediainfo = value; RaisePropertyChanged(() => this.HasMediaInfo); }
        }

        internal bool CanUnload { get; private set; }

        public MediaPlayerWindow()
        {
            InitializationComponents();
            //this.Resources = (ResourceDictionary)Application.LoadComponent(new Uri("/VideoPlayerView;component/Themes/Generic.xaml", UriKind.Relative));
            HookupEvents();
            CanUnload = true;
        }

        private void InitializationComponents()
        {
            mediaplayerwindowview = new MediaPlayerWindowView() { DataContext = this };
            MediaPlayerElement = new MediaPlayerElement();
            this.Content = mediaplayerwindowview;
            TrackWindowState();
            _mediaPlayerInfo = new MediaPlayerInfoView() { DataContext = this };
            InitiControl();
        }

        private void InitiControl()
        {
            _defaultPlayerControl = ServiceLocator.Current.GetInstance<IHomeControl>();
            var control = _defaultPlayerControl.MovieControl as MovieControl;
            MediaPlayerElement.UseSecondaryControl = control;
            control.SetControlSettings(new MovieControlSettings());
            control.MovieControlSettings.DisableMovieBoardText = false;
        }

        internal void Close()
        {
            if (!CanUnload)
                this.ClosePlayerActionFromPage();
            else
                ClosePlayerAction();
        }

        private void HookupEvents()
        {
            MediaPlayerElement.IsCloseButtonVisible = false;
            MediaPlayerElement.CanEscapeKeyCloseMedia = true;
            MediaPlayerElement.MovieControl.MovieControlSettings.IsMinimizeControlButtonEnabled = true;
            this.MediaPlayerElement.AllowMediaPlayerAutoDispose = false;
            MediaPlayerElement.SetWindowTopMostProperty += MediaPlayerElement_SetWindowTopMostProperty;
            MediaPlayerElement.OnFullScreenButtonToggle += MediaPlayerElement_OnFullScreenButtonToggle;
            MediaPlayerElement.OnMediaSizeChanged += MediaPlayerElement_OnMediaSizeChanged;
            MediaPlayerElement.OnMediaTitleChanged += MediaPlayerElement_OnMediaTitleChanged;
            MediaPlayerElement.OnCloseWindowToggled += MediaPlayerElement_OnCloseWindowToggled;
            MediaPlayerElement.OnMinimizedControlExecuted += MediaPlayerElement_OnMinimizedControlExecuted;
            this.MediaPlayerElement.OnMediaInfoChanged += MediaPlayerElement_OnMediaInfoChanged;
        }

        private void MediaPlayerElement_OnMediaInfoChanged(object sender, MediaInfoChangedEventArgs e)
        {
            MediaInformation = e.MediaInformation;
        }

        private void MediaPlayerElement_OnMinimizedControlExecuted(object sender, MediaPlayerViewTypeRoutedEventArgs e)
        {
            switch (e.MediaPlayerViewTypeState)
            {
                case MediaPlayerViewType.FullMediaPanel:
                    FullMediaPanelAction();
                    break;
                case MediaPlayerViewType.MiniMediaPanel:
                    MiniMediaPanelAction();
                    break;
                default:
                    break;
            }
        }

        private void FullMediaPanelAction()
        {
            CanUnload = false;
            PageNavigatorHost.RemoveView(typeof(MediaPlayerWindow).Name);

            ShellWindowService.AddView(this, typeof(MediaPlayerWindow).Name);
            IsMediaPlayerWindowEnabled = false;
            MediaPlayerElement.CanEscapeKeyCloseMedia = true;

            this.Width = double.NaN;
            this.Height = double.NaN;
            this.VerticalAlignment = VerticalAlignment.Stretch;
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        private void MiniMediaPanelAction()
        {
            FullScreenAction(WindowFullScreenState.Normal);
            CanUnload = false;
            IsMediaPlayerWindowEnabled = true;
            ShellWindowService.RemoveView(typeof(MediaPlayerWindow).Name);
            PageNavigatorHost.AddView(this,typeof(MediaPlayerWindow).Name);
            MediaPlayerElement.CanEscapeKeyCloseMedia = false;

            this.Width = 600;
            this.Height = 300;
            this.VerticalAlignment = VerticalAlignment.Bottom;
            this.HorizontalAlignment = HorizontalAlignment.Right;
        }

        private void MediaPlayerElement_OnCloseWindowToggled(object sender, RoutedEventArgs e)
        {
            if (this.MediaPlayerElement.MediaPlayerViewType == MediaPlayerViewType.FullMediaPanel)
                ClosePlayerAction();
            else
                ClosePlayerActionFromPage();
        }

        private void ClosePlayerActionFromPage()
        {
            PageNavigatorHost.RemoveView(typeof(MediaPlayerWindow).Name);
            ClosePlayerCommon();
        }

        private void ClosePlayerCommon()
        {
            this.GetWindow.Title = ApplicationConstants.SHELLWINDOWTITLE;
            CanUnload = true;
            ShellWindowService.ClearAdditionalStatusItem();
            if (GetWindow.Topmost)
                GetWindow.Topmost = false;

            MediaPlayerElement.Dispose();
            (_defaultPlayerControl.MovieControl as MovieControl).InitializeMediaPlayerControl(null);
        }

        private void ClosePlayerAction()
        {
            FullScreenAction(WindowFullScreenState.Normal);
            ShellWindowService.RemoveView(typeof(MediaPlayerWindow).Name);
            ClosePlayerCommon();
        }

        private void MediaPlayerElement_OnMediaTitleChanged(object sender, RoutedEventArgs e)
        {
            this.GetWindow.Title = MediaPlayerElement.MediaTitle;
        }

        private void MediaPlayerElement_OnFullScreenButtonToggle(object sender, WindowFullScreenRoutedEventArgs e)
        {
            FullScreenAction(e.FullScreenState);
        }

        private void FullScreenAction(WindowFullScreenState fullScreenState)
        {
            switch (fullScreenState)
            {
                case WindowFullScreenState.FullScreen:
                    this.GetWindow.UseNoneWindowStyle = true;
                    TrackWindowState();
                    this.GetWindow.WindowState = WindowState.Maximized;
                    this.GetWindow.IgnoreTaskbarOnMaximize = true;
                    break;
                case WindowFullScreenState.Normal:
                    GetWindow.UseNoneWindowStyle = false;
                    GetWindow.ShowTitleBar = true;
                    GetWindow.IgnoreTaskbarOnMaximize = false;
                    GetWindow.WindowState = _previousState;
                    break;
                default:
                    break;
            }
            ActiveMediaPlayerControl(fullScreenState);
        }

        private void ActiveMediaPlayerControl(WindowFullScreenState fullScreenState)
        {
            if(fullScreenState == WindowFullScreenState.FullScreen)
            {
                MediaPlayerElement.UseSecondaryControl = null;
                EventManager.GetEvent<FullScreenNotice>().Publish(true);
            }
            else
            {
                MediaPlayerElement.UseSecondaryControl = _defaultPlayerControl.MovieControl as MovieControl;
                EventManager.GetEvent<FullScreenNotice>().Publish(false);
            }
        }

        private void TrackWindowState()
        {
            _previousState = this.GetWindow.WindowState;
        }

        private void MediaPlayerElement_OnMediaSizeChanged(object sender, MediaSizeChangedRoutedArgs e)
        {
            //GetWindow.Width = e.MediaWidth;
            //GetWindow.Height = e.MediaHeight;
        }

        private void MediaPlayerElement_SetWindowTopMostProperty(object sender, RoutedEventArgs e)
        {
            this.GetWindow.Topmost = !GetWindow.Topmost;
        }

        private void AddSubtitleFileAction(string[] filePathInfo)
        {
            for (int i = 0; i < filePathInfo.Length; i++)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(filePathInfo[i]);
                if (file.Extension == ".srt")
                {
                    SetSubtitle(file.FullName);
                }
            }
        }

        public void SetSubtitle(string filepath)
        {
            MediaPlayerElement.MediaPlayerServices.SubtitleManagement.SetSubtitle(filepath);
        }

        public void LoadMediaFile(IPlayable playablefile)
        {
            MaximizeWindow();

            this.CurrentPlayingItem = playablefile;
            MediaPlayerElement.Source(playablefile);
        }

        private void MaximizeWindow()
        {
            ShellWindowService.ShellWindow.WindowState = WindowState.Maximized;
        }

        public void LoadMediaFile(Uri FileUrl)
        {
            MaximizeWindow();

            MediaPlayerElement.Source(FileUrl);
        }

        public void LoadMediaFile(IPlaylistModel plm)
        {
            MaximizeWindow();

            MediaPlayerElement.Source(plm);
        }

        public void LoadMediaFile(IPlayable playFile, IEnumerable<IPlayable> TemperalList)
        {
            MaximizeWindow();

            //LoadMediaFile(playFile);
            MediaPlayerElement.Source(playFile, TemperalList);
        }

        public void AddToPlaylist(IPlayable vfc)
        {
            MediaPlayerElement.AddToPlaylist(vfc);
        }

        public void AddRangeToPlaylist(IEnumerable<IPlayable> EnumerableVfc)
        {
            MediaPlayerElement.AddRangeToPlaylist(EnumerableVfc);
        }
        
        public void Show()
        {
            if (!IsMediaPlayerWindowEnabled)
            {
                ShellWindowService.SetAdditionalStatusItem(_mediaPlayerInfo);

                ShellWindowService.AddView(this, typeof(MediaPlayerWindow).Name);
                this.Focus();
                FocusManager.SetFocusedElement(this, MediaPlayerElement);
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

             String[] filePathInfo = (String[])e.Data.GetData("FileName", false);
            if(filePathInfo != null)
                AddSubtitleFileAction(filePathInfo);
               
            CommandManager.InvalidateRequerySuggested();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            //this.MediaPlayerElement.Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //this.MediaPlayerElement.Focus();
        }

    }
}
