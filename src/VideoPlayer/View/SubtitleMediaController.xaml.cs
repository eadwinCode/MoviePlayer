using Microsoft.Practices.ServiceLocation;
using Movies.MoviesInterfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using VideoPlayerControl.ViewModel;

namespace VideoPlayerControl
{
    /// <summary>
    /// Interaction logic for VideoPlayerView.xaml
    /// </summary>
    public partial class SubtitleMediaController : UserControl, IMediaController
    {
        IControllerView mediacontroller;
        public IControllerView MediaController
        {
            get
            {
                if (mediacontroller == null)
                    mediacontroller = new MediaController(FilePlayerManager.MediaControllerViewModel);
                return this.mediacontroller;
            }
        }
        
        public SubtitleMediaController()
        {
            InitializeComponent();
            var videoplayerViewModel = new VideoPlayerVM(this);
            this.DataContext = videoplayerViewModel;
            this.Loaded += (s,e)=> {
                videoplayerViewModel.Loaded();
            };

            this.controlRegion.Content = this.MediaController;
        }

        public event EventHandler ScreenSettingsChanged;
        
        IPlayFile FilePlayerManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }

        internal void OnScreenSettingsChanged(object sender)
        {
            if (ScreenSettingsChanged != null)
            {
                ScreenSettingsChanged(sender, EventArgs.Empty);
            }
        }

     

    }

    public enum SCREENSETTINGS
    {
        Normal,
        Fullscreen
    };
}
