using Common.Util;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Windows.Forms;
using System.Windows.Threading;

namespace MediaControl
{
    public partial class WindowsMediaPlayControl : Form
    {
        public double resumeposition;
       // private int currentpos;
        private int playstate;
        // private int timerstate;

        
        private IApplicationService IApplicationService 
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationService>();
            }
        }

        public VideoFolderChild CurrentVideoItem;
        private DispatcherTimer MediaPositionTimer;

        public WindowsMediaPlayControl()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.uiMode = "full";
            axWindowsMediaPlayer1.stretchToFit = true;
            axWindowsMediaPlayer1.PlayStateChange +=
                new AxWMPLib.
                _WMPOCXEvents_PlayStateChangeEventHandler(mediaElement1_PlayStateChange);
            
            axWindowsMediaPlayer1.MediaError += AxWindowsMediaPlayer1_MediaError;
            MediaPositionTimer = new DispatcherTimer();
            MediaPositionTimer.Tick += MediaPositionTimer_Tick;
            MediaPositionTimer.Interval = TimeSpan.FromMilliseconds(200);
            
            this.FormClosing += Wmp_test_FormClosing;
        }

        private void MediaPositionTimer_Tick(object sender, EventArgs e)
        {
            CurrentVideoItem.Progress = Math.Round((double.Parse(axWindowsMediaPlayer1.Ctlcontrols.currentPosition.ToString()) / double.Parse(axWindowsMediaPlayer1.currentMedia.duration.ToString()) * 100), 2) ;
        }

        private void AxWindowsMediaPlayer1_MediaError(object sender, AxWMPLib._WMPOCXEvents_MediaErrorEvent e)
        {

        }

        private void AxWindowsMediaPlayer1_PositionChange(object sender, AxWMPLib._WMPOCXEvents_PositionChangeEvent e)
        {
            
        }

        void mediaElement1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsPlaying)
            {
                if (resumeposition != 0)
                {
                    axWindowsMediaPlayer1.Ctlcontrols.currentPosition = (resumeposition/100)* axWindowsMediaPlayer1.currentMedia.duration;
                    resumeposition = 0;
                }
                if (playstate != 1)
                {
                    MediaOpen();
                }
                MediaPositionTimer.Start();
                this.Text = CommonHelper.SetPlayerTitle("Playing", CurrentVideoItem.MediaTitle);
            }
            else if (e.newState == (int)WMPLib.WMPPlayState.wmppsStopped)
            {
                //Played.RLastPostion(Helper.FileName(URL.uri));
                CurrentVideoItem.LastPlayedPoisition.ProgressLastSeen = (double)CurrentVideoItem.Progress;
                if (!CurrentVideoItem.HasLastSeen && CurrentVideoItem.Progress > 0)
                {
                    IApplicationService.SavedLastSeenCollection.Add((PlayedFiles)CurrentVideoItem.LastPlayedPoisition);
                }
                MediaPositionTimer.Stop();
                resumeposition = CurrentVideoItem.Progress;
                this.Text = CommonHelper.SetPlayerTitle("Stopped", CurrentVideoItem.MediaTitle);
            }
            else if(e.newState == (int)WMPLib.WMPPlayState.wmppsPaused)
            {
                MediaPositionTimer.Stop();
                this.Text = CommonHelper.SetPlayerTitle("Paused", CurrentVideoItem.MediaTitle);
            }

        }

        internal void OpenFile(VideoFolderChild obj)
        {
            if (CurrentVideoItem == null)
            {
                CurrentVideoItem = obj;
            }
            else if (CurrentVideoItem == obj)
            {
                return;
            }

            axWindowsMediaPlayer1.URL = obj.Directory.FullName;
            if (obj.HasLastSeen)
            {
                resumeposition = obj.LastPlayedPoisition.ProgressLastSeen;
            }
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void MediaOpen()
        {
            InitWndSize();
            //this.Text = CommonHelper.SetPlayerTitle("Playing", CurrentVideoItem.Directory.FullName);
            //PlayListManager.UpdateNowPlaying(CurrentVideoItem,false);
            playstate = 1;
        }

        private void InitWndSize()
        {
            this.Height = Math.Min(720, axWindowsMediaPlayer1.currentMedia.imageSourceHeight + 32);
            this.Width = Math.Min(1280, axWindowsMediaPlayer1.currentMedia.imageSourceWidth);
        }

        private void Previous()
        {
            SavePlayed();
            //if (PlayListManager.CanPrevious)
            //{
            //    CurrentVideoItem = PlayListManager.GetPreviousItem();
            //    axWindowsMediaPlayer1.URL = (CurrentVideoItem.Directory.FullName);
            //}
           
        }

        private void Next()
        {
            SavePlayed();
            //if (PlayListManager.CanNext)
            //{
            //    CurrentVideoItem = PlayListManager.GetNextItem();
            //    axWindowsMediaPlayer1.URL = (CurrentVideoItem.Directory.FullName);
            //}
        }

        private void SavePlayed()
        {
            CurrentVideoItem.LastPlayedPoisition.ProgressLastSeen = (double)axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            if (!CurrentVideoItem.HasLastSeen && CurrentVideoItem.Progress > 0)
            {
                IApplicationService.SavedLastSeenCollection.Add((PlayedFiles)CurrentVideoItem.LastPlayedPoisition);
            }
            IApplicationService.SaveLastSeenFile();
        }
        

        internal void CloseMedia()
        {
            //throw new NotImplementedException();
        }

        private void Wmp_test_FormClosing(object sender, FormClosingEventArgs e)
        {
            SavePlayed();
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            axWindowsMediaPlayer1.close();
            axWindowsMediaPlayer1 = null;
           
        }

        private void axWindowsMediaPlayer1_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            this.ActiveControl = this.panel1;
        }
    }
}
