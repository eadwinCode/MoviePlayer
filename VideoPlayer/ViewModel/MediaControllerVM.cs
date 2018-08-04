using Common.ApplicationCommands;
using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using VideoComponent.BaseClass;
using VideoPlayerControl.PlayList;
using System.Threading.Tasks;
using Meta.Vlc.Wpf;

namespace VideoPlayerControl.ViewModel
{
    public partial class MediaControllerVM : INotifyPropertyChanged
    {
        private static void InitInstance()
        {
            currentInstance = new MediaControllerVM();
        }

        private bool HasSubcribed = false;
        private static MediaControllerVM currentInstance;
        private VideoFolderChild currentvideoitem;
        private bool ismousecontrolover;
        private MediaState mediaState = MediaState.Stopped;
        private DelegateCommand playbtn;
        private DelegateCommand repeatbtn;
        private DelegateCommand mute;
        private string playtext;
        private DelegateCommand _next;
        private DelegateCommand _prev;
        private bool isplaying;
        private DelegateCommand tofullscreenbtn;
        private bool cananimate;
        private bool IsDirectoryChanged;
        private RepeatMode repeatmode = RepeatMode.NoRepeat;
        private TimeSpan lastseentime;
        private bool haslastseen;
        public bool IsDragging = false;
        public ExecuteCommand CurrentVideoItemChangedEvent;
        public DispatcherTimer positionSlideTimerTooltip;
        
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SubtitleChanged;
        public VolumeState VolumeState = VolumeState.Active;
        public DelegateCommand CloseLastSeenCommand { get; private set; }
        public DelegateCommand SetLastSeenCommand { get; private set; }
        public bool IsRewindOrFastForward { get; set; }

        public IMediaController IVideoPlayer
        {
            get
            {
                return IVideoElement.IVideoPlayer;
            }
        }

        public IVideoElement IVideoElement
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>().VideoElement;
            }
        }

        public MediaState MediaState
        {
            get { return mediaState; }
            set
            {
                mediaState = value;
                IsPlaying = value == MediaState.Playing ? true : false;

                if (value == MediaState.Stopped && IVideoElement.MediaPlayer.VlcMediaPlayer.Length != TimeSpan.Zero)
                {
                    MediaPlayerStop();
                }

                if (value == MediaState.Finished)
                {
                    MediaControlReset();
                    PlayBackAction(value.ToString());
                }

            }
        }
        public VideoFolderChild CurrentVideoItem
        {
            get { return currentvideoitem; }
        }

        public DelegateCommand Next
        {
            get
            {
                if (_next == null)
                {
                    _next = new DelegateCommand(NextPlayAction, CanNext);
                }
                return _next;
            }
        }

        public DelegateCommand ToFullScreenBtn
        {
            get
            {
                if (tofullscreenbtn == null)
                {
                    tofullscreenbtn = new DelegateCommand(() =>
                    {
                        (((SubtitleMediaController)IVideoPlayer).DataContext as VideoPlayerVM)
                        .FullScreenAction();
                    });
                }
                return tofullscreenbtn;
            }
        }

        public DelegateCommand RepeatBtn
        {
            get
            {
                if (repeatbtn == null)
                {
                    repeatbtn = new DelegateCommand(RepeatBtnAction);
                }
                return repeatbtn;
            }
        }

        public DelegateCommand Mute
        {
            get
            {
                if (mute == null)
                {
                    mute = new DelegateCommand(MuteAction);
                }
                return mute;
            }
            set
            {
                mute = value;
            }
        }
        
        public bool HaslastSeen
        {
            get { return haslastseen; }
            set { haslastseen = value; OnPropertyChanged("HaslastSeen"); }
        }

        public TimeSpan LastSeenTime
        {
            get { return lastseentime; }
            set { lastseentime = value; OnPropertyChanged("LastSeenTime"); }
        }

        public RepeatMode RepeatMode
        {
            get { return repeatmode; }
            set { repeatmode = value; OnPropertyChanged("RepeatMode"); }
        }

        public static MediaControllerVM MediaControllerInstance
        {
            get
            {
                return currentInstance;
            }
        }


        public delegate void ExecuteCommand(object sender, bool frompl);


        public PlayListManager Playlist
        {
            get
            {
                return (IVideoElement.PlayListView
                    as UserControl).DataContext as PlayListManager;
            }
        }
        public MovieTitle_Tab MovieTitle_Tab
        {
            get { return (IVideoPlayer.MediaController as IController).MovieTitle_Tab; }
        }
        public Slider DragPositionSlider
        {
            get { return DragProgressSliderPart.ProgressSlider; }
        }

        private Border Border
        {
            get
            {
                return BorderPart.Border;
            }
        }

        public Slider VolumeSlider
        {
            get { return VolumeControl.CurrentVolumeSlider; }
        }

        public DelegateCommand PlayBtn
        {
            get
            {
                if (playbtn == null)
                {
                    playbtn = new DelegateCommand(() =>
                    {
                        PlayAction();
                    }, CanPlay);
                }

                return playbtn;
            }
            set
            {
                playbtn = value;
            }
        }

        public DelegateCommand Previous
        {
            get
            {
                if (_prev == null)
                {
                    _prev = new DelegateCommand(PrevPlayAction, CanPrev);
                }
                return _prev;
            }
        }
        
        public string PlayText
        {
            get { return playtext; }
            set
            {
                playtext = value; OnPropertyChanged("PlayText");
            }
        }

        public bool IsPlaying
        {
            get { return isplaying; }
            set { isplaying = value; OnPropertyChanged("IsPlaying"); }
        }

        public bool IsMouseControlOver
        {
            get
            {
                return ismousecontrolover;
            }
            set
            {
                ismousecontrolover = value;
                OnPropertyChanged("IsMouseControlOver");
            }
        }

        public bool CanAnimate
        {
            get { return cananimate; }
            set
            {
                cananimate = value; OnPropertyChanged("CanAnimate");
                //PositioSliderInit(HasSubcribed); 
            }
        }
        
        public MediaControllerVM()
        {
            //  PlayListManager.Current.SetController(this);
            currentInstance = this;
            SetLastSeenCommand = new DelegateCommand(SetLastSeenAction);
            CloseLastSeenCommand = new DelegateCommand(CloseLastSeenAction);
        }
        
    }

    public partial class MediaControllerVM
    {
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

                IVideoElement.MediaPlayer.VlcMediaPlayer.EndReached += VlcMediaPlayer_EndReached;
                IVideoElement.MediaPlayer.MediaOpened += VlcMediaPlayer_MediaOpened;
                IVideoElement.MediaPlayer.OnSubItemAdded += MediaPlayer_OnSubItemAdded;

                IVideoElement.MediaPlayer.VlcMediaPlayer.EncounteredError += VlcMediaPlayer_EncounteredError;
                IVideoElement.MediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
                IVideoElement.MediaPlayer.VlcMediaPlayer.Stoped += new EventHandler<Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState>>(VlcMediaPlayer_Stoped);
                IVideoElement.MediaPlayer.StateChanged += new EventHandler<Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState>>(MediaPlayer_StateChanged);

                HasSubcribed = true;
            }
        }

        void MediaPlayer_OnSubItemAdded(object sender, EventArgs e)
        {

        }

        void MediaPlayer_StateChanged(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            //throw new NotImplementedException();
        }

        void VlcMediaPlayer_Stoped(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {

        }

        private void VlcMediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            (IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            {
                //if (IVideoPlayer == null) return;
                //if (IVideoElement.MediaPlayer.VlcMediaPlayer.IsPlaying)
                //{
                IVideoElement.MediaPlayer.VlcMediaPlayer.Pause();
                if (CurrentVideoItem.SubPath != null)
                {
                    ConvertSubFilesToVLCSubFile();
                }
                SetControllerDetails();
                MediaState = MediaState.Playing;
                CurrentVideoItem.IsActive = true;
                if (CurrentVideoItem.HasLastSeen)
                {
                    HaslastSeen = true;
                    LastSeenTime = TimeSpan.
                      FromSeconds((double.Parse(IVideoElement.MediaPlayer.VlcMediaPlayer.Length.TotalSeconds.ToString())*CurrentVideoItem.LastPlayedPoisition.ProgressLastSeen)/100);
                }
                else
                {
                    HaslastSeen = false;
                    LastSeenTime = TimeSpan.FromMilliseconds(0.0);
                }

                CommandManager.InvalidateRequerySuggested();

                ((IVideoPlayer as SubtitleMediaController).DataContext as VideoPlayerVM).VisibilityAnimation();
                DragPositionSlider.IsEnabled = true;
                UpdateHardCodedSubs();
                IVideoElement.MediaPlayer.VlcMediaPlayer.Play();
                //}

            }), DispatcherPriority.Background, null);

        }

        private void ConvertSubFilesToVLCSubFile()
        {
            foreach (var item in CurrentVideoItem.SubPath)
            {
                if (item.Directory == null) continue;
                this.SetSubtitle(item.Directory);
            }
            if (CurrentVideoItem.SubPath != null && CurrentVideoItem.SubPath.Count > 0)
            {
                Thread.Sleep(100);
                IVideoElement.MediaPlayer.VlcMediaPlayer.Subtitle = -1;
            }
        }

        public void UpdateHardCodedSubs()
        {
            (IVideoElement.MediaPlayer).Dispatcher.Invoke(new Action(() =>
            {
                Thread.SpinWait(10000);
                var subtracks = IVideoElement.MediaPlayer.VlcMediaPlayer.SubtitleDescription;
                var subtitle = IVideoElement.MediaPlayer.VlcMediaPlayer.Subtitle;
                var newsub = new ObservableCollection<SubtitleFilesModel>();
                if (CurrentVideoItem.SubPath == null)
                    CurrentVideoItem.SubPath = new ObservableCollection<SubtitleFilesModel>();
                //if there are no subtitle files but the movie has one coded inside
                if (CurrentVideoItem.SubPath != null && CurrentVideoItem.SubPath.Count == 0)
                {
                    foreach (var item in subtracks)
                    {
                        newsub.Add(new SubtitleFilesModel(item, subtitle));
                    }
                }
                else
                {
                    if (CurrentVideoItem.SubPath != null && CurrentVideoItem.SubPath.First().TrackDescription != null)
                    {
                        CurrentVideoItem.SubPath.RemoveAt(0);
                    }
                    int i = 0;
                    foreach (var item in subtracks)
                    {
                        string dir = null;
                        if (item.Id > -1)
                        {
                            dir = CurrentVideoItem.SubPath[i].Directory;
                            i++;
                        }
                        newsub.Add(new SubtitleFilesModel(item, subtitle, dir));
                    }
                }
                if (newsub.Count > 0)
                {
                    CurrentVideoItem.SubPath = new ObservableCollection<SubtitleFilesModel>(newsub);
                    if (SubtitleChanged != null)
                        SubtitleChanged(this, new EventArgs());
                }
            }), DispatcherPriority.Background, null);
        }

        private void SetLastSeen()
        {
            IVideoElement.MediaPlayer.VlcMediaPlayer.Time = LastSeenTime;
        }

        public void SetSubtitle(string filepath)
        {
            IVideoElement.MediaPlayer.VlcMediaPlayer.SetSubtitleFile(filepath);
        }

        public void NextPlayAction()
        {
            NewVideoAction(Playlist.GetNextItem());
        }

        private void RaiseCanPrevNext()
        {
            Previous.RaiseCanExecuteChanged();
            Next.RaiseCanExecuteChanged();
        }

        public bool CanNext()
        {
            if (IVideoElement == null)
            {
                return false;
            }
            return Playlist == null ? false : Playlist.CanNext;
        }

        public bool CanPlay()
        {
            if (IVideoElement == null)
            {
                return false;
            }
            return IVideoElement.MediaPlayer.VlcMediaPlayer.Length != TimeSpan.Zero;
        }

        public void PrevPlayAction()
        {
            NewVideoAction(Playlist.GetPreviousItem());
        }

        public bool CanPrev()
        {
            if (IVideoElement == null)
            {
                return false;
            }
            return Playlist == null ? false : Playlist.CanPrevious;
        }



        public void MediaPlayStopAction()
        {
            Stop();
            MediaPlayerStop();
        }

        private void Stop()
        {
            if (IVideoElement.MediaPlayer.HasStopped)
                IVideoElement.MediaPlayer.Stop();
            DragPositionSlider.IsEnabled = false;
            DragPositionSlider.Value = 0;
        }

        public void CloseMediaPlayer()
        {
            //(IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            //{
            IVideoElement.CommandBindings.Clear();
            MediaPlayStopAction();
            Unsubscribe();
            HasSubcribed = false;
            if (CurrentVideoItem != null)
            {
                ApplicationService.SaveLastSeenFile();
            }

            if (Playlist.CurrentPlaylist != null)
            {
                Playlist.CurrentPlaylist.SetIsActive(false);
            }
            currentvideoitem = null;
            currentInstance = null;
            //  }), null);
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

            IVideoElement.MediaPlayer.VlcMediaPlayer.Opening -= VlcMediaPlayer_Opening;
            IVideoElement.MediaPlayer.VlcMediaPlayer.EndReached -= VlcMediaPlayer_EndReached;
            IVideoElement.MediaPlayer.MediaOpened -= VlcMediaPlayer_MediaOpened;
            IVideoElement.MediaPlayer.OnSubItemAdded -= MediaPlayer_OnSubItemAdded;

            IVideoElement.MediaPlayer.VlcMediaPlayer.EncounteredError -= VlcMediaPlayer_EncounteredError;
            IVideoElement.MediaPlayer.TimeChanged -= MediaPlayer_TimeChanged;
            IVideoElement.MediaPlayer.VlcMediaPlayer.Stoped -= new EventHandler<Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState>>(VlcMediaPlayer_Stoped);
            IVideoElement.MediaPlayer.StateChanged -= new EventHandler<Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState>>(MediaPlayer_StateChanged);
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
                MovieTitle_Tab.MovieTitleText = CommonHelper.
                                                   SetPlayerTitle(action, CurrentVideoItem.FileName);
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

            if (mediaState == MediaState.Stopped)
            {
                Stop();
                PlayBackAction(MediaState.ToString());
            }
            CurrentVideoItem.IsActive = false;
            if (IsDirectoryChanged)
            {
                IsDirectoryChanged = false;
                ApplicationService.SaveLastSeenFile();
            }
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

        private void NewVideoAction(VideoFolderChild obj, bool frompl = false)
        {
            (IVideoElement.MediaPlayer).Dispatcher.Invoke(new Action(() =>
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
                            this.IVideoElement.MediaPlayer.Time
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

                this.MediaState = MediaState.Stopped;

                this.currentvideoitem = obj;
                CurrentVideoItemChangedEvent(currentvideoitem, frompl);
                IVideoElement.Title = (obj.FileName);
                IVideoElement.MediaPlayer.LoadMediaWithOptions(obj.FilePath, IVideoElement.MediaPlayer.VlcOption);
                PlayAction();
            }), DispatcherPriority.ContextIdle, null);
        }

        private void MediaPlayer_TimeChanged(object sender, EventArgs e)
        {
            if (!IsDragging)
            {
                DragPositionSlider.Value = IVideoElement.MediaPlayer.Time.TotalSeconds;
                CurrentVideoItem.Progress= Math.Round((double.Parse(DragPositionSlider.Value.ToString()) / double.Parse(IVideoElement.MediaPlayer.VlcMediaPlayer.Length.TotalSeconds.ToString()) * 100),2);
                //(sender as VlcPlayer).VlcMediaPlayer.SetFullScreen();
            }
        }

        private void VlcMediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            DragPositionSlider.IsEnabled = false;
            PlayBackAction("Failed to Play", "Stop");
            this.MediaState = MediaState.Failed;
            CloseMediaPlayer();
            throw new Exception(IVideoElement.MediaPlayer.VlcMediaPlayer.Media.Error);
        }

        private void VlcMediaPlayer_EndReached(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
            //Stop();
            (IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            {
                DragPositionSlider.IsEnabled = false;
                DragPositionSlider.Value = 0;
                this.MediaState = MediaState.Finished;
                CurrentVideoItem.IsActive = false;
                if (this.RepeatMode != RepeatMode.NoRepeat)
                {
                    Task.Factory.StartNew(() => GetItem_for_Repeat())
                        .ContinueWith(t => StartRepeatAction(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
                }
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

        private void VlcMediaPlayer_Opening(object sender, Meta.Vlc.ObjectEventArgs<Meta.Vlc.Interop.Media.MediaState> e)
        {
        }


        private VideoFolderChild AsynSearchForNextItem()
        {
            if (Playlist.CanNext)
            {
                return Playlist.GetNextItem();
            }
            return null;
        }

        private void SetControllerDetails()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (CurrentVideoItem == null) return;
                TimeSpan ts = new TimeSpan();
                ts = IVideoElement.MediaPlayer.VlcMediaPlayer.Length;
                DragPositionSlider.Maximum = ts.TotalSeconds;
                DragPositionSlider.SmallChange = 1;
                SetMediaVolume(VolumeSlider.Value);
            }), null);
        }

        #region Volume Slider
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var vol = e.NewValue;
            SetMediaVolume(vol);
        }

        private void SetMediaVolume(double vol)
        {
            if (IVideoElement.MediaPlayer == null) return;
            IVideoElement.MediaPlayer.Volume = (int)vol;
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
            if (!IVideoElement.MediaPlayer.VlcMediaPlayer.IsMute)
            {
                IVideoElement.MediaPlayer.VlcMediaPlayer.ToggleMute();
                VolumeSlider.IsEnabled = false;
                VolumeState = VolumeState.Muted;
            }
            else
            {
                IVideoElement.MediaPlayer.VlcMediaPlayer.ToggleMute();
                VolumeSlider.IsEnabled = true;
                VolumeState = VolumeState.Active;
            }
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            PlayAction();
        }

        public void PlayAction()
        {
            if (MediaState == MediaState.Playing)
            {
                IVideoElement.MediaPlayer.PauseOrResume();
                this.MediaState = MediaState.Paused;
                PlayText = "Play";
                MovieTitle_Tab.MovieTitleText = CommonHelper.SetPlayerTitle("Paused",
                    CurrentVideoItem.MediaTitle);
                //MediaPositionTimer.Stop();
            }
            else
            {
                if (IVideoElement.MediaPlayer.VlcMediaPlayer.State == Meta.Vlc.Interop.Media.MediaState.Ended)
                {
                    IVideoElement.MediaPlayer.LoadMedia(CurrentVideoItem.FilePath);
                }
                IVideoElement.MediaPlayer.Play();
                this.MediaState = MediaState.Playing;
                PlayText = "Pause";
                MovieTitle_Tab.MovieTitleText = CommonHelper.SetPlayerTitle("Playing",
                   CurrentVideoItem.MediaTitle);
                // MediaPositionTimer.Start();
            }
        }

        public void positionSlideTimerTooltipStop()
        {
            // if (ToolTipTxt.Visibility == System.Windows.Visibility.Visible) { ToolTipTxt.Visibility = System.Windows.Visibility.Collapsed; }
            if (MovieTitle_Tab == null) return;
            MovieTitle_Tab.MovieText = null;
            positionSlideTimerTooltip.Stop();
        }

        public void MediaController_Loaded(object sender, RoutedEventArgs e)
        {
            positionSlideTimerTooltipStop();
            DragPositionSlider.IsEnabled = false;
            if ((IVideoElement as Window).IsLoaded)
            {
                (IVideoElement as Window).SizeChanged += MediaControllerVM_SizeChanged;
                (IVideoPlayer as UserControl).SizeChanged += MediaControllerVM_SizeChanged;
            }

            Init();
        }

        private void MediaControllerVM_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IVideoElement == null || !(IVideoElement as Window).IsLoaded) return;
            Panel panel = (IVideoPlayer.MediaController as IController).GroupedControls;
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
    }
}
