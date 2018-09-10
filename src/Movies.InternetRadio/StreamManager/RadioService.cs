using Movies.InternetRadio.ViewModels;
using Movies.InternetRadio.Views;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoPlayerView;

namespace Movies.InternetRadio.StreamManager
{
    public class RadioService : IRadioService
    {
        RadioStreamingElementViewModel radioStreamingElement;
        private bool isradioon = false;
        IPlayFile fileplayermanager;
        IMainPage radiohomepage;

        public bool IsRadioOn { get { return isradioon; } }
        public IPlayFile FileplayerManager { get { return fileplayermanager; } }
        public IMainPage RadioHomepage { get { return radiohomepage; } }
        
        public RadioService(IPlayFile fileplayermanager)
        {
            this.fileplayermanager = fileplayermanager;
            radiohomepage = new RadioHomepage();
        }

        public void PlayRadio(RadioModel radioModel)
        {
            if (FileplayerManager.IsPlayingMedia)
                FileplayerManager.ShutDownMediaPlayer();

            InitRadioPlayer();
            radioStreamingElement.PlayStation(radioModel);
            isradioon = true;
        }

        private void InitRadioPlayer()
        {
            if(radioStreamingElement == null)
            {
                radioStreamingElement = new RadioStreamingElementViewModel();
                radioStreamingElement.HostViewInHomePage();
            }
        }

        public void ShutdownRadio()
        {
            if (radioStreamingElement != null)
                radioStreamingElement.Shutdown();
            radioStreamingElement = null;
            isradioon = false;
        }
    }
}
