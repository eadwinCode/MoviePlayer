using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using Movies.InternetRadio.StreamManager;
using Movies.InternetRadio.Util;
using Movies.InternetRadio.Views;
using Movies.MediaService.Interfaces;
using Movies.MediaService.Models;
using Movies.MediaService.Service;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VideoPlayerControl;
using VideoPlayerControl.View;
using VideoPlayerView;

namespace Movies.InternetRadio.ViewModels
{
    public class RadioStreamingElementViewModel: NotificationObject
    {
        IMediaPlayerService mediaPlayerService;
        RadioStreamHomePageControl radioStreamingElement;
        private RadioStreamingControl radiocontrol;
        bool Isclosing;
        bool hasOngoingAction;

        IRadioService IRadioService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioService>();
            }
        }

        public RadioStreamingControl RadioControl
        {
            get { return radiocontrol; }
            set { radiocontrol = value; RaisePropertyChanged(() => this.RadioControl); }
        }

        private double streamingtime;

        public double StreamingTime
        {
            get { return streamingtime; }
            set { streamingtime = value; RaisePropertyChanged(() => this.StreamingTime); }
        }

        private NetworkConnectionStatus networkconnectionstatus;

        public NetworkConnectionStatus NetworkConnectionStatus
        {
            get { return networkconnectionstatus; }
            private set { networkconnectionstatus = value; }
        }


        private bool isplaying;

        public bool IsPlaying
        {
            get { return isplaying; }
            set { isplaying = value; RaisePropertyChanged(() => this.IsPlaying); }
        }

        public string PlayText
        {
            get { return playtext; }
            set
            {
                playtext = value; RaisePropertyChanged("PlayText");
            }
        }

        private string movieboard;

        public string MovieBoard
        {
            get { return movieboard; }
            set { movieboard = value; RaisePropertyChanged(() => this.MovieBoard); }
        }


        public Slider VolumeSlider
        {
            get { return VolumeControl.CurrentVolumeSlider; }
        }

        public Slider PlayerSlider
        {
            get { return RadioControl.SliderControl; }
        }

        private RadioStationDetails radiostationdetails;

        public RadioStationDetails RadioStationDetails
        {
            get { return radiostationdetails; }
            set { radiostationdetails = value; RaisePropertyChanged(() => this.RadioStationDetails); }
        }

        private bool HasSubcribed;
        private RadioModel currentradiostation;
        private string playtext;
        private DelegateCommand playbtn;
        private DelegateCommand closemediacontrol;

        public RadioModel CurrentRadioStation
        {
            get { return currentradiostation; }
            set { currentradiostation = value; RaisePropertyChanged(() => this.CurrentRadioStation); }
        }


        public DelegateCommand PauseOrResumeCommand
        {
            get
            {
                if (playbtn == null)
                {
                    playbtn = new DelegateCommand(() =>
                    {
                        PlayAction();
                    });
                }

                return playbtn;
            }
        }

        public DelegateCommand CloseMediaControl
        {
            get
            {
                if (closemediacontrol == null)
                {
                    closemediacontrol = new DelegateCommand(() =>
                    {
                        IRadioService.ShutdownRadio();
                    });
                }

                return closemediacontrol;
            }
        }

        private void PlayAction()
        {
           
            if (mediaPlayerService.State == MovieMediaState.Playing)
            {
                mediaPlayerService.PauseOrResume();
                PlayText = "Play";
                return;
            }

            //if (!NetworkConnectionStatus.CheckForInternetConnection())
            //    return;

            if (mediaPlayerService.State == MovieMediaState.Ended ||
                mediaPlayerService.State == MovieMediaState.Paused ||
                mediaPlayerService.State == MovieMediaState.Stopped)
            {
                if (mediaPlayerService.State == MovieMediaState.Ended ||
                    mediaPlayerService.State == MovieMediaState.Stopped)
                {
                    mediaPlayerService.LoadMedia(new Uri(CurrentRadioStation.StationURL));
                }
                mediaPlayerService.Play();
                PlayText = "Pause";
                // MediaPositionTimer.Start();
            }
        }

        public RadioStreamingElementViewModel()
        {
            RadioControl = new RadioStreamingControl() { DataContext = this};
            RadioStationDetails = new RadioStationDetails() { DataContext = this};
            NetworkConnectionStatus = new NetworkConnectionStatus();
            InitMediaPlayerService();
        }
        

        private void InitMediaPlayerService()
        {
            mediaPlayerService = new MediaPlayerService();
        }
        
        #region MediaPlayerService Events

        private void MediaPlayerService_OnTimeChanged(object sender, EventArgs e)
        {
            PlayerSlider.Value = StreamingTime = mediaPlayerService.CurrentTimer.TotalSeconds;
        }

        private void MediaPlayerService_EndReached(object sender, EventArgs e)
        {
            (RadioControl).Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!hasOngoingAction)
                {
                    hasOngoingAction = true;
                    PlayerSlider.IsEnabled = false;
                    PlayerSlider.Value = 0;
                    CurrentRadioStation.IsActive = false;
                }
                else
                    hasOngoingAction = false;
            }));
        }

        private void MediaPlayerService_EncounteredError(object sender, EventArgs e)
        {
            (RadioControl).Dispatcher.BeginInvoke(new Action(() =>
            {
                PlayerSlider.IsEnabled = false;
                PlayBackAction("Failed to Play", "Stop");
                CloseMediaPlayer();
            }));
        }

        private void MediaPlayerService_OnStateChanged(object sender, EventArgs e)
        {
            var mediaState = mediaPlayerService.State;
            PlayBackAction(mediaState.ToString());
            Console.WriteLine("{0} - checking out", mediaPlayerService.State.ToString());
            //throw new NotImplementedException();

            IsPlaying = mediaState == MovieMediaState.Playing ? true : false;
            
        }

        private void MediaPlayerService_OnMediaOpened(object sender, EventArgs e)
        {
            (RadioControl).Dispatcher.Invoke(new Action(() =>
            {
                //MediaPlayerService.Pause();
               
                CurrentRadioStation.IsActive = true;
               
                CommandManager.InvalidateRequerySuggested();
                PlayerSlider.IsEnabled = true;
                PlayerSlider.Maximum = mediaPlayerService.Duration.TotalSeconds;
                mediaPlayerService.Play();
                PlayBackAction( CommonHelper.SetPlayerTitle("Playing",
                    CurrentRadioStation.StationName));
                
            }), DispatcherPriority.Background);
        }

        private void MediaPlayerService_OnMediaOpening(object sender, EventArgs e)
        {
            PlayBackAction(CommonHelper.SetPlayerTitle("Opening",
                   CurrentRadioStation.StationName + "..."));
        }

        private void MediaPlayerService_Buffering(object sender, MediaBufferingEventArgs e)
        {
            PlayBackAction(CommonHelper.SetPlayerTitle(string.Format("Buffering {0} %", e.NewCache),
                  CurrentRadioStation.StationName));
            if(e.NewCache == 100)
                PlayBackAction(CommonHelper.SetPlayerTitle("Playing",
                    CurrentRadioStation.StationName));
        }
        #endregion

        public void PlayStation(RadioModel radiostation)
        {
            if (CurrentRadioStation != null)
                CurrentRadioStation.IsActive = false;

            if (!mediaPlayerService.HasStopped)
                mediaPlayerService.Stop();

            if (!string.IsNullOrEmpty(radiostation.StationURL))
            {
                CurrentRadioStation = radiostation;
                var url = radiostation.StationURL;
                mediaPlayerService.LoadMedia(new Uri(url));
                //PlayAction();
            }
        }

        public RadioStreamHomePageControl HostViewInHomePage()
        {
            if (radioStreamingElement == null)
            {
                radioStreamingElement = new RadioStreamHomePageControl()
                {
                    DataContext = this,
                };
                radioStreamingElement.MediaControlRegion.Content = mediaPlayerService.VideoPlayer;
                radioStreamingElement.Loaded += (s, e) => Loaded();

                ((IRadioService as RadioService).RadioHomepage as RadioHomepage).
                    MediaControlRegion.Content = radioStreamingElement;
            }
            return radioStreamingElement;
        }

        public void Shutdown()
        {
            if (!Isclosing)
            {
                Isclosing = true;
                DestroyHomePageView();
                this.Dispose();
                Isclosing = false;
            }
        }

        private void DestroyHomePageView()
        {
            ((IRadioService as RadioService).RadioHomepage as RadioHomepage).
                  MediaControlRegion.Content = null;
            radioStreamingElement = null;
        }

        private void Dispose()
        {
            UnsubscribeEvents();
            mediaPlayerService.Dispose();

            CloseMediaPlayer();
        }

        private void UnsubscribeEvents()
        {
            VolumeSlider.MouseDown += VolumeSlider_MouseDown;
            VolumeSlider.PreviewMouseDown += VolumeSlider_MouseDown;
            VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            mediaPlayerService.Buffering -= MediaPlayerService_Buffering;
            mediaPlayerService.OnMediaOpening -= MediaPlayerService_OnMediaOpening;
            mediaPlayerService.OnMediaOpened -= MediaPlayerService_OnMediaOpened;
            mediaPlayerService.OnStateChanged -= MediaPlayerService_OnStateChanged;
            mediaPlayerService.EncounteredError -= MediaPlayerService_EncounteredError;
            mediaPlayerService.EndReached -= MediaPlayerService_EndReached;
        }

        public void Loaded()
        {
            if (!HasSubcribed)
            {
                RegisterSliderEvents();
                HasSubcribed = true;
            }
        }

        private void RegisterSliderEvents()
        {
            VolumeSlider.MouseDown += VolumeSlider_MouseDown;
            VolumeSlider.PreviewMouseDown += VolumeSlider_MouseDown;
            VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;

            mediaPlayerService.Buffering += MediaPlayerService_Buffering;
            mediaPlayerService.OnMediaOpening += MediaPlayerService_OnMediaOpening;
            mediaPlayerService.OnMediaOpened += MediaPlayerService_OnMediaOpened;
            mediaPlayerService.OnStateChanged += MediaPlayerService_OnStateChanged;
            mediaPlayerService.EncounteredError += MediaPlayerService_EncounteredError;
            mediaPlayerService.EndReached += MediaPlayerService_EndReached;
            mediaPlayerService.OnTimeChanged += MediaPlayerService_OnTimeChanged;
        }

        #region Volume Slider
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var vol = e.NewValue;
            SetMediaVolume(vol);
        }

        private void SetMediaVolume(double vol)
        {
            if (mediaPlayerService == null) return;
            mediaPlayerService.Volume = (int)vol;
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

        private void PlayBackAction(string action, string playbtn = null)
        {
            (RadioControl).Dispatcher.Invoke(new Action(() =>
            {
                if (playbtn != null)
                {
                    this.PlayText = playbtn;
                }
                else { this.PlayText = "Play"; }
                MovieBoard = CommonHelper.SetPlayerTitle(action, CurrentRadioStation.StationName);
            }), null);
        }

        public void CloseMediaPlayer(bool wndClose = false)
        {
            MediaPlayStopAction();
            currentradiostation.IsActive = false;
        }

        private void Stop()
        {
            if (!mediaPlayerService.HasStopped && !mediaPlayerService.IsDisposed)
                mediaPlayerService.Stop();
            PlayerSlider.IsEnabled = false;
            PlayerSlider.Value = 0;
        }

        public void MediaPlayStopAction()
        {
            Stop();
        }
    }
}
