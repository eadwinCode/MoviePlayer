using Common.FileHelper;
using Common.Interfaces;
using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VideoComponent.BaseClass;
using VideoPlayer.PlayList;

namespace VideoPlayer.ViewModel
{
    public class MediaControllerVM : INotifyPropertyChanged
    {
        private static void InitInstance()
        {
            currentInstance = new MediaControllerVM();
        }

        public bool IsRewindOrFastForward { get; set; }
        private static MediaControllerVM currentInstance;
        public static MediaControllerVM Current { get { if (currentInstance == null) { InitInstance(); } return currentInstance; }
            set { currentInstance = value; } }
        private bool HasSubcribed = false;

        private VideoFolderChild currentvideoitem;
        //private object mediapositiontracker;
        private string playtext;
        private DelegateCommand _next;
        public delegate void ExecuteCommand(object sender,bool frompl);
        public ExecuteCommand CurrentVideoItemChangedEvent;
        private DelegateCommand _prev;
        private bool isplaying;
        public bool IsDragging = false;
        private DispatcherTimer MediaPositionTimer;
        public DispatcherTimer positionSlideTimerTooltip;
        
        public event PropertyChangedEventHandler PropertyChanged;
        private bool ismousecontrolover;
        private MediaState mediaState = MediaState.Stopped;
        private DelegateCommand playbtn;
        private DelegateCommand mute;

        public PlayListManager Playlist { get { return (IVideoElement.PlayListView as UserControl).DataContext as PlayListManager; } }
        //private static Slider positionslider;

        public MovieTitle_Tab MovieTitle_Tab
        {
            get { return (IVideoPlayer.MediaController as IMediaController).MovieTitle_Tab; }
        }


        //public Slider PositionSlider
        //{
        //    get { return ProgressSliderPart.ProgressSlider; }
        //   // set { positionslider = value; OnPropertyChanged("PositionSlider"); }
        //}

        public Slider DragPositionSlider {
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
            get { return VolumeSliderPart.VolumeSlider; }
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
                    },CanPlay);
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
        

        private bool cananimate;
        public bool CanAnimate
        {
            get { return cananimate; }
            set { cananimate = value; OnPropertyChanged("CanAnimate");
                //PositioSliderInit(HasSubcribed); 
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
        private DelegateCommand tofullscreenbtn;
        private bool IsDirectoryChanged;

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

        public MediaControllerVM()
        {
            //  PlayListManager.Current.SetController(this);
           
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

                IVideoElement.MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
                IVideoElement.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
                IVideoElement.MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
                MediaPositionTimer = new DispatcherTimer();
                MediaPositionTimer.Tick += MediaPositionTimer_Tick;
                MediaPositionTimer.Interval = TimeSpan.FromMilliseconds(200);
                // IVideoPlayer.MediaPlayer.MediaUriPlayer.MediaPositionChanged += MediaUriPlayer_MediaPositionChanged;
                //IVideoPlayer.MediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
                //IVideoPlayer.MediaPlayer.MediaClosed += MediaPlayer_MediaClosed;


                HasSubcribed = true;
            }
        }

        private void MediaPlayer_MediaClosed(object sender, RoutedEventArgs e)
        {
            
        }

        private void MediaUriPlayer_MediaPositionChanged(object sender, EventArgs e)
        {
            //if (IsClosing)
            //    return;

            //if (!IsDragging)
            //{
            //    (IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            //    {
            //        if (IsClosing)
            //            return;

            //        CurrentVideoItem.Progress = PositionSlider.Value = (double)IVideoPlayer.MediaPlayer.MediaPosition/ 10000000;
            //        if (IVideoPlayer.Subtitle.HasSub)
            //        {
            //            IVideoPlayer.Subtitle.SetText(IVideoPlayer.MediaPlayer.MediaPosition/ 10000000);
            //        }
            //    }), null);
            //}
            
        }

        private void MediaPlayer_MediaFailed(object sender, WPFMediaKit.DirectShow.MediaPlayers.MediaFailedEventArgs e)
        {
            (IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            {
                PlayBackAction("Failed to Play", "Stop");
                MediaState = MediaState.Failed;
            }), null);
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
            return IVideoElement.MediaPlayer.HasVideo;
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
        
        public MediaState MediaState
        {
            get { return mediaState; }
            set
            {
                mediaState = value;
                IsPlaying = value == MediaState.Playing ? true : false;

                if (value == MediaState.Stopped && IVideoElement.MediaPlayer.HasVideo)
                {
                    MediaPlayerStop();
                }

                if (value == MediaState.Finished)
                {
                    MediaControlReset();
                    PlayBackAction(value.ToString());
                    IVideoElement.MediaPlayer.Close();
                }

            }
        }

        public void MediaPlayStopAction()
        {
            IVideoElement.MediaPlayer.Stop();
            MediaPlayerStop();
        }

        public void CloseMediaPlayer()
        {
            //(IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            //{
            //IVideoPlayer.MediaPlayer.Close();
            //IsClosing = true;
            //HasSubcribed = false;
            //CreateHelper.SaveLastSeenFile(CurrentVideoItem.ParentDirectory.Directory);
            //currentvideoitem = null;
            //Current = null;
            MediaPlayStopAction();
            MediaPositionTimer.Stop();
            HasSubcribed = false;
            if (CurrentVideoItem != null)
            {
                CreateHelper.SaveLastSeenFile(CurrentVideoItem.ParentDirectory);
            }
            currentvideoitem = null;
            Current = null;
            if (Playlist.CurrentPlaylist != null)
            {
                Playlist.CurrentPlaylist.SetIsActive(false);
            }
            

            //}), null);
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

                MovieTitle_Tab.MovieTitleText = CommonHelper.SetPlayerTitle(action, IVideoElement.MediaPlayer.Source.ToString());
               // MediaPositionTimer.Stop();
            }),null);
        }

        private void MediaControlReset()
        {
            MediaPositionTimer.Stop();
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
                LastSeenHelper.AddLastSeen(CurrentVideoItem.ParentDirectory,CurrentVideoItem.LastPlayedPoisition);
            }

            if (mediaState == MediaState.Stopped)
            {
                IVideoElement.MediaPlayer.Stop();
                PlayBackAction(MediaState.ToString());
            }
            CurrentVideoItem.IsActive = false;
            if (IsDirectoryChanged)
            {
                IsDirectoryChanged = false;
                CreateHelper.SaveLastSeenFile(CurrentVideoItem.ParentDirectory);
            }
        }
        
        protected void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public void GetVideoItem(VideoFolderChild obj,bool frompl = false)
        {
            NewVideoAction(obj, frompl);
        }

        private void NewVideoAction(VideoFolderChild obj, bool frompl = false)
        {
            if (obj == null)
            {
                return;
            }
            if (CurrentVideoItem != null)
            {
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

            MediaState = MediaState.Stopped;
            if (obj.SubPath.Count > 0)
            {
                IVideoPlayer.Subtitle.LoadSub(obj.SubPath.First());
            }
            else
            {
                IVideoPlayer.Subtitle.Clear();
                //CSubtitle.ClearSubstitute(); OutlineTextSub.Visibility = 
                //System.Windows.Visibility.Collapsed;
            }

            this.currentvideoitem = obj;
            CurrentVideoItemChangedEvent(currentvideoitem, frompl);

            IVideoElement.MediaPlayer.Source = new Uri(obj.FilePath);
            PlayAction();



            RaiseCanPrevNext();
        }

        void MediaPositionTimer_Tick(object sender, EventArgs e)
        {
            if (!IsDragging)
            {
                CurrentVideoItem.Progress = DragPositionSlider.Value = 
                    IVideoElement.MediaPlayer.Position.TotalSeconds;
                if (IVideoPlayer.Subtitle.HasSub)
                {
                    IVideoPlayer.Subtitle.SetText(IVideoElement.MediaPlayer.Position.TotalSeconds);
                }
            }

        }

        void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            DragPositionSlider.IsEnabled = false;
            PlayBackAction("Failed to Play", "Stop");
            MediaState = MediaState.Failed;
        }

        void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {

            IVideoElement.MediaPlayer.Close();
            MediaState = MediaState.Finished;
            //Task.Factory.StartNew(() => this.AsynSearchForNextItem()).
            //ContinueWith(t => GetVideoItem(t.Result), 
            //TaskScheduler.FromCurrentSynchronizationContext()).Wait(200);
            GetVideoItem(this.AsynSearchForNextItem());
        }

        private VideoFolderChild AsynSearchForNextItem()
        {
            if (Playlist.CanNext)
            {
                return Playlist.GetNextItem();
            }
            return null;
        }

        void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            //(IVideoElement as Window).Dispatcher.Invoke(new Action(() =>
            //{
            if (IVideoPlayer == null) return;
            if (IVideoElement.MediaPlayer.HasVideo)
            {
                SetControllerDetails();
                MediaState = MediaState.Playing;
                MediaPositionTimer.Start();
                CurrentVideoItem.IsActive = true;
                if (CurrentVideoItem.HasLastSeen)
                {
                    IVideoElement.MediaPlayer.Position = TimeSpan.
                        FromSeconds(CurrentVideoItem.LastPlayedPoisition.ProgressLastSeen);
                }
                //if (CurrentVideoItem.HasLastSeen)
                //{
                //    IVideoPlayer.MediaPlayer.MediaPosition = 
                //    (long)CurrentVideoItem.LastPlayedPoisition.ProgressLastSeen * 10000000;
                //}
                CommandManager.InvalidateRequerySuggested();

                ((IVideoPlayer as SubtitleMediaController).DataContext as VideoPlayerVM)
                    .VisibilityAnimation();
                DragPositionSlider.IsEnabled = true;
            }
            // }), null);
        }

        private void SetControllerDetails()
        {
            if (CurrentVideoItem == null) return;
            if (IVideoElement.MediaPlayer.HasVideo)
            {
                TimeSpan ts = IVideoElement.MediaPlayer.NaturalDuration.TimeSpan;
                DragPositionSlider.Maximum = ts.TotalSeconds;
                DragPositionSlider.SmallChange = 1;
            }
            //Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            //{
            //    if (IVideoPlayer.MediaPlayer.MediaUriPlayer.HasVideo)
            //    {
            //        PositionSlider.Maximum = (double)IVideoPlayer.MediaPlayer.MediaDuration/ 10000000;
            //        PositionSlider.SmallChange = 1;
            //    }
            //}), null);
            // VideoPlayer.Subtitle.AdjustFontSize(15 * (96.0 / 72.0), .75);
        }

        //private void LoadSub(string FilePath)
        //{
        //    CSubtitle.SetSubtitle(FilePath);
        //    IsSubAvailable = true;
        //    OutlineTextSub.Visibility = System.Windows.Visibility.Visible;
        //}

        #region Volume Slider
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var vol = e.NewValue;
            if (IVideoElement.MediaPlayer == null) return;

            IVideoElement.MediaPlayer.Volume = vol / 100;
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
            if (!MovieTitle_Tab.IsCanvasDrag && IsPlaying)
            {
                MovieTitle_Tab.SetIsMouseDown(Border, true);
                IsMouseControlOver = true;
            }
        }

        public void MainControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!MovieTitle_Tab.IsCanvasDrag && IsPlaying)
            {
                IsMouseControlOver = false;
                MovieTitle_Tab.SetIsMouseDown(Border, false);
                //MovieTitle_Tab.IsMouseMediaControlOver = false;
            }
        }
        #endregion
        
        public void MuteAction()
        {
            if (!IVideoElement.MediaPlayer.IsMuted)
            {
                IVideoElement.MediaPlayer.IsMuted = true;
                VolumeSlider.IsEnabled = false;
            }
            else
            {
                IVideoElement.MediaPlayer.IsMuted = false;
                VolumeSlider.IsEnabled = true;
            }

            //if (IVideoElement.MediaPlayer.IsMuted)
            //{
            //    IVideoElement.MediaPlayer.Volume = 0;
            //    VolumeSlider.IsEnabled = false;
            //}
            //else
            //{
            //    IVideoElement.MediaPlayer.Volume = VolumeSlider.Value/100;
            //    VolumeSlider.IsEnabled = true;
            //}
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            PlayAction();
        }

        public void PlayAction()
        {
            if (MediaState == MediaState.Playing)
            {
                IVideoElement.MediaPlayer.Pause();
                MediaState = MediaState.Paused;
                PlayText = "Play";
                MovieTitle_Tab.MovieTitleText = CommonHelper.SetPlayerTitle("Paused", 
                    IVideoElement.MediaPlayer.Source.ToString());
                MediaPositionTimer.Stop();
            }
            else
            {
                IVideoElement.MediaPlayer.Play();
                MediaState = MediaState.Playing;
                PlayText = "Pause";
                MovieTitle_Tab.MovieTitleText = CommonHelper.SetPlayerTitle("Playing", 
                    IVideoElement.MediaPlayer.Source.ToString());
                MediaPositionTimer.Start();
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
            }
            //if (CanAnimate)
            //{
            //    movietitle_tab = (MovieTitle_Tab)BorderPart.SCMovieTitle_Tab;
            //}
            //else
            //{

            //}
            //if (IVideoPlayer.MediaPlayer.HasVideo)
            //{
            //    MovieTitle_Tab.MovieTitleText = CommonHelper.SetPlayerTitle("Playing", IVideoPlayer.MediaPlayer.Source.ToString());
            //}
            //MovieTitle_Tab = null;

            //MovieTitle_Tab = new MovieTitle_Tab(IVideoPlayer.CanvasEnvironment as DragCanvas, this)
            //{
            //    Background = Brushes.Gray
            //};

            //this.titlesubtxet.Content = MovieTitle_Tab;

            Init();
        }

        private void MediaControllerVM_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IVideoElement == null || !(IVideoElement as Window).IsLoaded) return;
            Panel panel = (IVideoPlayer.MediaController as IMediaController).GroupedControls;
            if ((IVideoElement as Window).ActualWidth < 600)
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
        
        public ISubtitleMediaController IVideoPlayer
        {
            get {
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

        private bool issizechanged;

        public bool IsSizeChanged { get { return issizechanged; }
            set { issizechanged = value;
                // OnPropertyChanged("IsSizeChanged"); 
            }
        }
    }
}
