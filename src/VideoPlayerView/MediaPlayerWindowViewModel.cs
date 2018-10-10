using Common.ApplicationCommands;
using Delimon.Win32.IO;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using MovieHub.MediaPlayerElement;
using MovieHub.MediaPlayerElement.Models;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VideoPlayerView
{
    public class MediaPlayerWindow : MovieBase,INotifyPropertyChanged
    {
        private MediaPlayerWindowView mediaplayerwindowview;
        private MediaPlayerElement mediaplayerelement;
        private IPlayable CurrentPlayingItem;
        private WindowState _previousState;
        private dynamic _previousSettings;

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

        private MetroWindow GetWindow
        {
            get { return ShellWindowService.ShellWindow; }
        }

        public MediaPlayerElement MediaPlayerElement
        {
            get { return mediaplayerelement; }
            set { mediaplayerelement = value;RaisePropertyChanged(() => this.MediaPlayerElement); }
        }
        
        private DelegateCommand closemediaplayerwindow;

        public DelegateCommand CloseMediaPlayerWindow
        {
            get
            {
                if(closemediaplayerwindow == null)
                {
                    closemediaplayerwindow = new DelegateCommand(() => ClosePlayerActionFromPage());
                }
                return closemediaplayerwindow;
            }
        }

        private bool ismediaplayerwindowenabled = false;

        public bool IsMediaPlayerWindowEnabled
        {
            get { return ismediaplayerwindowenabled; }
            set { ismediaplayerwindowenabled = value; RaisePropertyChanged(() => this.IsMediaPlayerWindowEnabled); }
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
            MediaPlayerElement.MovieControl.IsMinimizeControlButtonEnabled = true;
            this.MediaPlayerElement.AllowMediaPlayerAutoDispose = false;
            MediaPlayerElement.SetWindowTopMostProperty += MediaPlayerElement_SetWindowTopMostProperty;
            MediaPlayerElement.OnFullScreenButtonToggle += MediaPlayerElement_OnFullScreenButtonToggle;
            MediaPlayerElement.OnMediaSizeChanged += MediaPlayerElement_OnMediaSizeChanged;
            MediaPlayerElement.OnMediaTitleChanged += MediaPlayerElement_OnMediaTitleChanged;
            MediaPlayerElement.OnCloseWindowToggled += MediaPlayerElement_OnCloseWindowToggled;
            MediaPlayerElement.OnMinimizedControlExecuted += MediaPlayerElement_OnMinimizedControlExecuted; 
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
            ClosePlayerAction();
        }
        
        private void ClosePlayerActionFromPage()
        {
            this.GetWindow.Title = ApplicationConstants.SHELLWINDOWTITLE;
            CanUnload = true;

            PageNavigatorHost.RemoveView(typeof(MediaPlayerWindow).Name);
            if (GetWindow.Topmost)
                GetWindow.Topmost = false;
            MediaPlayerElement.Dispose();
        }

        private void ClosePlayerAction()
        {
            this.GetWindow.Title = ApplicationConstants.SHELLWINDOWTITLE;
            FullScreenAction(WindowFullScreenState.Normal);
            CanUnload = true;

            ShellWindowService.RemoveView(typeof(MediaPlayerWindow).Name);
            if (GetWindow.Topmost)
                GetWindow.Topmost = false;
            MediaPlayerElement.Dispose();
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
        }

        private void TrackWindowState()
        {
            _previousState = this.GetWindow.WindowState;
        }

        private void MediaPlayerElement_OnMediaSizeChanged(object sender, MediaSizeChangedRoutedArgs e)
        {
            GetWindow.Width = e.MediaWidth;
            GetWindow.Height = e.MediaHeight;
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
            this.CurrentPlayingItem = playablefile;
            MediaPlayerElement.Source(playablefile);
        }

        public void LoadMediaFile(Uri FileUrl)
        {
            MediaPlayerElement.Source(FileUrl);
        }

        public void LoadMediaFile(IPlaylistModel plm)
        {
            MediaPlayerElement.Source(plm);
        }

        public void LoadMediaFile(IPlayable playFile, IEnumerable<IPlayable> TemperalList)
        {
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
