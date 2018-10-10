using Microsoft.Practices.ServiceLocation;
using Movies.InternetRadio.ViewModels;
using Movies.InternetRadio.Views;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Movies.InternetRadio.StreamManager
{
    internal class RadioService : IRadioService
    {
        RadioPlayer radioplayer;
        private bool isradioon = false;
        IPlayFile fileplayermanager;
        IMainPage radiohomepage;
        internal IDispatcherService _idispatcherService;

        public bool IsRadioOn { get { return isradioon; } }
        public IPlayFile FileplayerManager { get { return fileplayermanager; } }
        public IMainPage RadioHomepage { get { return radiohomepage; } internal set { radiohomepage = value; } }

        private IShellWindow IShell
        {
            get { return ServiceLocator.Current.GetInstance<IShellWindow>(); }
        }

        public RadioService(IPlayFile fileplayermanager,IDispatcherService dispatcherService)
        {
            this.fileplayermanager = fileplayermanager;
            _idispatcherService = dispatcherService;
            RadioHomepage = new RadioHomepage();
        }
        

        public void PlayRadio(RadioModel radioModel)
        {
            if (FileplayerManager.IsPlayingMedia)
                FileplayerManager.ShutDown();

            InitRadioPlayer();
            radioplayer.PlayStation(radioModel);
            isradioon = true;
        }

        private void InitRadioPlayer()
        {
            if(radioplayer == null)
            {
                radioplayer = new RadioPlayer();
                radioplayer.HostViewInHomePage();
            }
        }
        
        public void ShutDown()
        {
            if (radioplayer != null)
                radioplayer.Shutdown();
            radioplayer = null;
            isradioon = false;
        }
    }
}
