using Common.Util;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;

namespace VideoPlayerControl.ViewModel
{
    public partial class MediaControllerViewModel
    {

        public MediaControllerViewModel()
        {
            //  PlayListManager.Current.SetController(this);
            SetLastSeenCommand = new DelegateCommand(SetLastSeenAction);
            CloseLastSeenCommand = new DelegateCommand(CloseLastSeenAction);
        }

        private void RepeatBtnAction()
        {
            if (RepeatMode == RepeatMode.NoRepeat)
            {
                RepeatMode = RepeatMode.Repeat;
            }
            else
            if (RepeatMode == RepeatMode.Repeat)
            {
                RepeatMode = RepeatMode.RepeatOnce;
            }
            else
            if (RepeatMode == RepeatMode.RepeatOnce)
            {
                RepeatMode = RepeatMode.NoRepeat;
            }
        }

        private void CloseLastSeenAction()
        {
            HaslastSeen = false;
        }

        private void SetLastSeenAction()
        {
            SetLastSeen();
            HaslastSeen = false;
        }
        
        private void ConvertSubFilesToVLCSubFile()
        {
            MediaPlayerService.MediaDispatcher.BeginInvoke(new Action(() =>
            {
                if (MediaPlayerService.SubtitleManagement == null)
                    return;
                if (MediaPlayerService.SubtitleManagement.SubstituteCount > 0)
                {
                    MediaPlayerService.SubtitleManagement.SetSubtitle("");
                    return;
                }

                foreach (var item in CurrentVideoItem.SubPath)
                {
                    if (item.Directory == null) continue;
                    this.SetSubtitle(item.Directory);
                }
            }), DispatcherPriority.Background, null);
            
        }

        private void SetLastSeen()
        {
            MediaPlayerService.CurrentTimer = LastSeenTime;
        }
        
        private void RaiseCanPrevNext()
        {
            Previous.RaiseCanExecuteChanged();
            Next.RaiseCanExecuteChanged();
        }

        private void Init()
        {
            if (!HasSubcribed)
            {
                #region VolumeSliderEvents
                VolumeSlider.MouseDown += VolumeSlider_MouseDown;
                VolumeSlider.PreviewMouseDown += VolumeSlider_MouseDown;
                VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
                #endregion

                #region BorderEvents
                this.Border.MouseEnter += MainControl_MouseEnter;
                this.Border.MouseLeave += MainControl_MouseLeave;
                #endregion

                MediaPlayerService.EndReached += VlcMediaPlayer_EndReached;
                MediaPlayerService.OnMediaOpened += VlcMediaPlayer_MediaOpened;
                MediaPlayerService.OnSubItemAdded += MediaPlayer_OnSubItemAdded;
                MediaPlayerService.OnMediaOpening += VlcMediaPlayer_Opening;
                MediaPlayerService.EncounteredError += VlcMediaPlayer_EncounteredError;
                MediaPlayerService.OnTimeChanged += MediaPlayer_TimeChanged;
                MediaPlayerService.OnMediaStopped += (VlcMediaPlayer_Stoped);
                MediaPlayerService.OnStateChanged += (MediaPlayer_StateChanged);

                HasSubcribed = true;
            }
        }

        private void Unsubscribe()
        {
            #region VolumeSliderEvents
            VolumeSlider.MouseDown -= VolumeSlider_MouseDown;
            VolumeSlider.PreviewMouseDown -= VolumeSlider_MouseDown;
            VolumeSlider.ValueChanged -= VolumeSlider_ValueChanged;
            #endregion

            #region BorderEvents
            this.Border.MouseEnter -= MainControl_MouseEnter;
            this.Border.MouseLeave -= MainControl_MouseLeave;
            #endregion

            MediaPlayerService.OnMediaOpening -= VlcMediaPlayer_Opening;
            MediaPlayerService.EndReached -= VlcMediaPlayer_EndReached;
            MediaPlayerService.OnMediaOpened -= VlcMediaPlayer_MediaOpened;
            MediaPlayerService.OnSubItemAdded -= MediaPlayer_OnSubItemAdded;

            MediaPlayerService.EncounteredError -= VlcMediaPlayer_EncounteredError;
            MediaPlayerService.OnTimeChanged -= MediaPlayer_TimeChanged;
            MediaPlayerService.OnMediaStopped -= (VlcMediaPlayer_Stoped);
            MediaPlayerService.OnStateChanged -= (MediaPlayer_StateChanged);
        }

        private void PlayBackAction(string action, string playbtn = null)
        {
            (IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            {
                if (playbtn != null)
                {
                    this.PlayText = playbtn;
                }
                else { this.PlayText = "Play"; }
                IMovieTitle_Tab.MovieTitleText = CommonHelper.
                                                   SetPlayerTitle(action, CurrentVideoItem.MediaTitle);
            }), null);
        }

        private void MediaControlReset()
        {
            DragPositionSlider.Value = 0;
            CurrentVideoItem.Progress = 0.0;
            CurrentVideoItem.PlayCompletely();
        }

        private void MediaPlayerStop()
        {
            if (CurrentVideoItem == null)
            {
                return;
            }
            CurrentVideoItem.LastPlayedPoisition.ProgressLastSeen = (double)CurrentVideoItem.Progress;
            if (!CurrentVideoItem.HasLastSeen && CurrentVideoItem.Progress > 0)
            {
                ApplicationService.SavedLastSeenCollection.Add((PlayedFiles)CurrentVideoItem.LastPlayedPoisition);
            }

            //if (MediaPlayerService.State == MovieMediaState.Stopped)
            //{
            //    Stop();
            //    PlayBackAction(MediaPlayerService.State.ToString());
            //}
            CurrentVideoItem.IsActive = false;
            if (IsDirectoryChanged)
            {
                IsDirectoryChanged = false;
                ApplicationService.SaveLastSeenFile();
            }

            PlayBackAction(CommonHelper.SetPlayerTitle("Stopped",
                    CurrentVideoItem.MediaTitle + "..."),"Pause");
        }
        
        private void Stop()
        {
            if (MediaPlayerService.HasStopped)
                MediaPlayerService.Stop();
            DragPositionSlider.IsEnabled = false;
            DragPositionSlider.Value = 0;
        }
        
        private void NewVideoAction(VideoFolderChild obj, bool frompl = false)
        {
            MediaPlayerService.MediaDispatcher.Invoke(new Action(() =>
            {
                if (obj == null)
                {
                    return;
                }
                if (CurrentVideoItem != null)
                {
                    if (this.RepeatMode == RepeatMode.RepeatOnce)
                    {
                        if (this.IsPlaying)
                        {
                            this.MediaPlayerService.CurrentTimer
                                = TimeSpan.FromMilliseconds(0);
                            return;
                        }
                        PlayAction();
                        return;
                    }
                    if (obj.FileName == CurrentVideoItem.FileName)
                    {
                        return;
                    }

                    if (!obj.ParentDirectory.Directory.FullName.Equals(CurrentVideoItem.
                        ParentDirectory.Directory.FullName))
                    {
                        IsDirectoryChanged = true;
                    }
                }

                MediaPlayerService.Stop();

                this.currentvideoitem = obj;
                CurrentVideoItemChangedEvent(currentvideoitem, frompl);
                IVideoElement.Title = (obj.FileName);
                MediaPlayerService.LoadMedia(obj.FilePath, MediaPlayerService.VlcOption);
                PlayAction();
                CommandManager.InvalidateRequerySuggested();
            }), DispatcherPriority.ContextIdle, null);
        }

        private VideoFolderChild AsynSearchForNextItem()
        {
            if (FilePlayerManager.PlaylistManagerViewModel.CanNext)
            {
                return FilePlayerManager.PlaylistManagerViewModel.GetNextItem();
            }
            return null;
        }

        private void SetControllerDetails()
        {
            (IVideoElement as Window).Dispatcher.BeginInvoke(new Action(() =>
            {
                if (CurrentVideoItem == null) return;
                TimeSpan ts = new TimeSpan();
                ts = MediaPlayerService.Duration;
                DragPositionSlider.Maximum = ts.TotalSeconds;
                DragPositionSlider.SmallChange = 1;
                SetMediaVolume(VolumeSlider.Value);
            }), null);

        }

        private VideoFolderChild GetItem_for_Repeat()
        {
            Thread.Sleep(1000);
            VideoFolderChild item = null;
            (IVideoElement as Window).Dispatcher.Invoke(new Action(() => {
                item = this.AsynSearchForNextItem();
            }), null);
            return item;
        }

        private void StartRepeatAction(VideoFolderChild vfc)
        {
            if (vfc != null)
                this.GetVideoItem(vfc, true);
        }

        private void VlcMediaPlayer_Opening(object sender, EventArgs e)
        {
            Console.WriteLine("{0} - checking out {1}", MediaPlayerService.State.ToString(),CommonHelper.SetPlayerTitle("Opening",
                    CurrentVideoItem.MediaTitle+"..."));

            PlayBackAction(CommonHelper.SetPlayerTitle("Opening",
                    CurrentVideoItem.MediaTitle + "..."));
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            PlayAction();
        }

        private void MediaControllerVM_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IVideoElement == null || !(IVideoElement as Window).IsLoaded) return;
            Panel panel = (IVideoPlayer.MediaController as IControllerView).GroupedControls;
            if ((IVideoElement as Window).ActualWidth < 600 ||
                (IVideoPlayer as UserControl).ActualWidth < 600)
            {
                panel.SetValue(DockPanel.DockProperty, Dock.Bottom);
            }
            else
            {
                panel.SetValue(DockPanel.DockProperty, Dock.Right);
            }

        }

        private void _nextbtn_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            //MovieTitle_Tab.MovieText = "Next:- " + Playlist.WhatsNextItem();
            //Button tt = (Button)sender;
            //tt.ToolTip = MovieTitle_Tab.MovieText;
            // MovieTitle_Tab.MovieTitleText = CommonHelper.SetPlayerTitle("Next", Playlist.WhatsNextItem());
        }

        private void _prevbtn_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            //MovieTitle_Tab.MovieText = "Previous:- " + Playlist.WhatsPreviousItem();
            //Button tt = (Button)sender;
            //tt.ToolTip = MovieTitle_Tab.MovieText;
            //MovieTitle_Tab.MovieTitleText = CommonHelper.SetPlayerTitle("Previous", Playlist.WhatsPreviousItem());
        }

        private void _prevbtn_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            //MovieTitle_Tab.MovieText = null;
        }

        private void MediaPlayer_TimeChanged(object sender, EventArgs e)
        {
            TimeChangeAction();
        }

        public void TimeChangeAction()
        {
            if (!IsDragging)
            {
                DragPositionSlider.Value = MediaPlayerService.CurrentTimer.TotalSeconds;
                CurrentVideoItem.Progress = Math.Round((double.Parse(DragPositionSlider.Value.ToString()) / double.Parse(MediaPlayerService.Duration.TotalSeconds.ToString()) * 100), 2);
                //(sender as VlcPlayer).VlcMediaPlayer.SetFullScreen();
            }
        }

        private void VlcMediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            DragPositionSlider.IsEnabled = false;
            PlayBackAction("Failed to Play", "Stop");
            CloseMediaPlayer();
            throw new Exception(MediaPlayerService.MediaError);
        }

        private void VlcMediaPlayer_EndReached(object sender, EventArgs e)
        {
            //Stop();
            (IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            {
                DragPositionSlider.IsEnabled = false;
                DragPositionSlider.Value = 0;
                CurrentVideoItem.IsActive = false;
                if (this.RepeatMode != RepeatMode.NoRepeat)
                {
                    Task.Factory.StartNew(() => GetItem_for_Repeat())
                        .ContinueWith(t => StartRepeatAction(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }), null);
        }

        void MediaPlayer_OnSubItemAdded(object sender, EventArgs e)
        {

        }

        void MediaPlayer_StateChanged(object sender, EventArgs e)
        {
            var mediaState = MediaPlayerService.State;
            PlayBackAction(mediaState.ToString());
            Console.WriteLine("{0} - checking out",MediaPlayerService.State.ToString());
            //throw new NotImplementedException();

            IsPlaying = mediaState == MovieMediaState.Playing ? true : false;

            if (mediaState == MovieMediaState.Stopped && MediaPlayerService.Duration != TimeSpan.Zero)
            {
                MediaPlayerStop();
            }

            if (mediaState == MovieMediaState.Ended)
            {
                MediaControlReset();
            }
        }

        void VlcMediaPlayer_Stoped(object sender, EventArgs e)
        {
            MediaPlayStopAction();
            IsPlaying = false;
        }

        private void VlcMediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            (IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            {
                //MediaPlayerService.Pause();
                if (CurrentVideoItem.SubPath != null)
                {
                    ConvertSubFilesToVLCSubFile();
                }
                SetControllerDetails();
                CurrentVideoItem.IsActive = true;
                if (CurrentVideoItem.HasLastSeen && CurrentVideoItem.LastPlayedPoisition.ProgressLastSeen > 0)
                {
                    LastSeenTime = TimeSpan.
                      FromSeconds((double.Parse(MediaPlayerService.Duration.TotalSeconds.ToString()) * CurrentVideoItem.LastPlayedPoisition.ProgressLastSeen) / 100);
                    HaslastSeen = true;

                }
                else
                {
                    HaslastSeen = false;
                    LastSeenTime = TimeSpan.FromMilliseconds(0.0);
                    CurrentVideoItem.LastPlayedPoisition.ProgressLastSeen = 0;
                }

                CommandManager.InvalidateRequerySuggested();

                ((IVideoPlayer as SubtitleMediaController).DataContext as VideoPlayerVM).VisibilityAnimation();
                DragPositionSlider.IsEnabled = true;
                MediaPlayerService.Play();
                CommonHelper.SetPlayerTitle("Playing",
                    CurrentVideoItem.MediaTitle);

            }), DispatcherPriority.Background, null);

        }


        #region Volume Slider
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var vol = e.NewValue;
            SetMediaVolume(vol);
        }

        private void SetMediaVolume(double vol)
        {
            if (MediaPlayerService == null) return;
            MediaPlayerService.Volume = (int)vol;
        }

        private void VolumeSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Slider vol = (Slider)sender;
            var ratio = GetMousePointer(vol);
            vol.Value = Math.Round(ratio * vol.Maximum, 2);
        }

        public double GetMousePointer(Control obj)
        {
            var x = Mouse.GetPosition(obj).X;
            var ratio = x / obj.ActualWidth;
            return ratio;
        }
        #endregion

        #region BorderAnimation
        private void MainControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsPlaying)
            {
                MovieTitle_Tab.SetIsMouseDown(Border, true);
                IsMouseControlOver = true;
            }
        }

        public void MainControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsPlaying)
            {
                IsMouseControlOver = false;
                MovieTitle_Tab.SetIsMouseDown(Border, false);
                //MovieTitle_Tab.IsMouseMediaControlOver = false;
            }
        }
        #endregion

        public void MuteAction()
        {
            if (!MediaPlayerService.IsMute)
            {
                MediaPlayerService.ToggleMute();
                VolumeSlider.IsEnabled = false;
                VolumeState = VolumeState.Muted;
            }
            else
            {
                MediaPlayerService.ToggleMute();
                VolumeSlider.IsEnabled = true;
                VolumeState = VolumeState.Active;
            }
        }

        public void PlayAction()
        {
            if (MediaPlayerService.State == MovieMediaState.Playing)
            {
                MediaPlayerService.PauseOrResume();
                PlayText = "Play";
            }
            else if(MediaPlayerService.State == MovieMediaState.Ended || 
                MediaPlayerService.State == MovieMediaState.Paused || 
                MediaPlayerService.State == MovieMediaState.Stopped)
            {
                if (MediaPlayerService.State == MovieMediaState.Ended)
                {
                    MediaPlayerService.LoadMedia(CurrentVideoItem.FilePath);
                }
                MediaPlayerService.Play();
                PlayText = "Pause";
                // MediaPositionTimer.Start();
            }
        }

        public void PositionSlideTimerTooltipStop()
        {
            // if (ToolTipTxt.Visibility == System.Windows.Visibility.Visible) { ToolTipTxt.Visibility = System.Windows.Visibility.Collapsed; }
            if (IMovieTitle_Tab == null) return;
            IMovieTitle_Tab.MovieText = null;
            PositionSlideTimerTooltip.Stop();
        }

        public void MediaController_Loaded(object sender, RoutedEventArgs e)
        {
            PositionSlideTimerTooltipStop();
            DragPositionSlider.IsEnabled = false;
            if ((IVideoElement as Window).IsLoaded)
            {
                (IVideoElement as Window).SizeChanged += MediaControllerVM_SizeChanged;
                (IVideoPlayer as UserControl).SizeChanged += MediaControllerVM_SizeChanged;
            }

            Init();
        }

        public void SetSubtitle(string filepath)
        {
            MediaPlayerService.SubtitleManagement.SetSubtitle(filepath);
            if (SubtitleChanged != null)
                this.SubtitleChanged.Invoke(this, EventArgs.Empty);
        }

        public void NextPlayAction()
        {
            NewVideoAction(FilePlayerManager.PlaylistManagerViewModel.GetNextItem());
        }

        public bool CanNext()
        {
            if (IVideoElement == null)
            {
                return false;
            }
            return FilePlayerManager.PlaylistManagerViewModel == null ? false : FilePlayerManager.PlaylistManagerViewModel.CanNext;
            //return false;
        }

        public bool CanPlay()
        {
            if (IVideoElement == null)
            {
                return false;
            }
            return MediaPlayerService.Duration != TimeSpan.Zero;
        }

        public void PrevPlayAction()
        {
            NewVideoAction(FilePlayerManager.PlaylistManagerViewModel.GetPreviousItem());
        }

        public bool CanPrev()
        {
            if (IVideoElement == null)
            {
                return false;
            }
            return FilePlayerManager.PlaylistManagerViewModel == null ? false : FilePlayerManager.PlaylistManagerViewModel.CanPrevious;
            //return false;
        }

        public void MediaPlayStopAction()
        {
            Stop();
            MediaPlayerStop();
        }

        public void CloseMediaPlayer(bool wndClose = false)
        {
            IVideoElement.IVideoPlayerController.CommandBindings.Clear();
            MediaPlayStopAction();
            Unsubscribe();
            HasSubcribed = false;
            if (CurrentVideoItem != null)
            {
                ApplicationService.SaveLastSeenFile();
            }

            if (FilePlayerManager.PlaylistManagerViewModel.CurrentPlaylist != null)
            {
                FilePlayerManager.PlaylistManagerViewModel.CurrentPlaylist.SetIsActive(false);
            }
            currentvideoitem = null;
        }

        protected void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public void GetVideoItem(VideoFolderChild obj, bool frompl = false)
        {
            NewVideoAction(obj, frompl);
        }
    }
}
