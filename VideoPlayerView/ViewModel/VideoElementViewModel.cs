using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using VideoComponent.BaseClass;
using VideoPlayer;
using VideoPlayer.ViewModel;

namespace VideoPlayerView.ViewModel
{
    public class VideoElementViewModel:NotificationObject
    {
        private WindowState IntialWindowsState;
        public VideoElementViewModel()
        {
           // SetStyleOnWindowState((Application.Current.MainWindow.WindowState));
        }
        
        private string maxbtntooltip;
        public string MaxbtnTooltip
        {
            get { return maxbtntooltip; }
            private set { maxbtntooltip = value; this.RaisePropertyChanged(() => this.MaxbtnTooltip); }
        }

        void Plv_OnPlaylistClose(object sender, EventArgs e)
        {
            if (MediaControlExtension.GetFileexpVisiblity(IVideoElement.PlayListView as UIElement) == System.Windows.Visibility.Visible)
            {
                MediaControlExtension.SetFileexpVisiblity(IVideoElement.PlayListView as UIElement, System.Windows.Visibility.Collapsed);
            }
            else
            {
                MediaControlExtension.SetFileexpVisiblity(IVideoElement.PlayListView as UIElement, System.Windows.Visibility.Visible);
            }
        }

        private Visibility fullscreenbtn = Visibility.Collapsed;
        private bool IsSuspended;

        public Visibility FullScreenBtn
        {
            get { return fullscreenbtn ; }
            set { fullscreenbtn = value;RaisePropertyChanged(() => this.FullScreenBtn); }
        }

        //public Style StyleChanger
        //{
        //    get { return stylechanger; }
        //    private set
        //    {
        //        stylechanger = value;
        //        this.RaisePropertyChanged(() => this.StyleChanger);
        //    }
        //}
        //public void SetStyleOnWindowState(WindowState state)
        //{
        //    if (state == WindowState.Normal)
        //    {
        //        StyleChanger = (Style)Application.Current.Resources["maxbtn"];
        //        MaxbtnTooltip = "Maximize";
        //    }
        //    else
        //    {
        //        StyleChanger = (Style)Application.Current.Resources["normbtn"];
        //        MaxbtnTooltip = "Restore Down";
        //    }
        //}

        internal void Loaded()
        {
            IVideoElement.PlayListView.OnPlaylistClose += Plv_OnPlaylistClose;
            IVideoElement.IVideoPlayer.ScreenSettingsChanged += IVideoPlayer_ScreenSettingsChanged;
            MediaControlExtension.SetFileexpVisiblity(IVideoElement.PlayListView as UIElement, System.Windows.Visibility.Collapsed);
            // FocusManager.SetFocusedElement(IVideoElement as DependencyObject,Mouse.Captured);

            SystemEvents.PowerModeChanged += this.SystemEvents_PowerModeChanged;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    if (IsSuspended)
                    {
                        MediaControllerVM.Current.PlayAction(); IsSuspended = false;
                    }
                    break;
                case PowerModes.StatusChange:
                    break;
                case PowerModes.Suspend:
                    if (MediaControllerVM.Current.IsPlaying)
                    {
                        MediaControllerVM.Current.PlayAction();
                        IsSuspended = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void IVideoPlayer_ScreenSettingsChanged(object sender, EventArgs e)
        {
            //SCREENSETTINGS ss = (SCREENSETTINGS)sender;
            var args = sender as object[];
            //if (((SCREENSETTINGS)args[0])== SCREENSETTINGS.Fullscreen && ((SCREENSETTINGS)args[1]) == SCREENSETTINGS.Fullscreen)
            //{
            //    FullScreenBtn = Visibility.Visible;
            //    IVideoElement.WindowsTab.Visibility = Visibility.Collapsed;
            //    IntialWindowsState = (IVideoElement as Window).WindowState;
            //    (IVideoElement as Window).WindowState = WindowState.Maximized;
            //}
            //else if(((SCREENSETTINGS)args[0]) == SCREENSETTINGS.Normal && ((SCREENSETTINGS)args[1]) == SCREENSETTINGS.Fullscreen)
            //{
            //    FullScreenBtn = Visibility.Collapsed;
            //    IVideoElement.WindowsTab.Visibility = Visibility.Visible;
            //    (IVideoElement as Window).WindowState = IntialWindowsState;
            //}
            //else
            //{
            if(args[0] == null)
            {
                if (FullScreenBtn == Visibility.Visible)
                {
                    FullScreenBtn = Visibility.Collapsed;
                }
                else
                {
                    FullScreenBtn = Visibility.Visible;
                }

                return;
            }

            if (((SCREENSETTINGS)args[0]) == SCREENSETTINGS.Fullscreen)
            {
                FullScreenBtn = Visibility.Visible;
                IVideoElement.WindowsTab.Visibility = Visibility.Collapsed;
                IntialWindowsState = (IVideoElement as Window).WindowState;
                (IVideoElement as Window).WindowState = WindowState.Maximized;
            }
            else if(((SCREENSETTINGS)args[0]) == SCREENSETTINGS.Normal)
            {
                FullScreenBtn = Visibility.Collapsed;
                IVideoElement.WindowsTab.Visibility = Visibility.Visible;
                (IVideoElement as Window).WindowState = IntialWindowsState;
            }
            
          //  }
            
        }

        private IVideoElement IVideoElement
        {
            get { return ServiceLocator.Current.GetInstance<IPlayFile>().VideoElement; }
        }

        public List<SubtitleFilesModel> SubtitleTitleCollection { get
            {
                return (MediaControllerVM.Current.CurrentVideoItem as VideoFolderChild).SubPath;
            }
        }
    }
}
