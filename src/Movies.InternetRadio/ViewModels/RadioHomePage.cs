using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Movies.InternetRadio.ViewModels
{
    public class RadioHomePageViewModel : NotificationObject
    {
        private ObservableCollection<RadioModel> radioModelCollection;

        public ObservableCollection<RadioModel> RadioModelCollection
        {
            get { return radioModelCollection; }
            set { radioModelCollection = value; RaisePropertyChanged(()=>this.RadioModelCollection); }
        }

        public DelegateCommand<RadioModel> OpenFolderCommand { get; private set; }
        public DelegateCommand<RadioModel> EditorAditRadioStation { get; private set; }
        public DelegateCommand<RadioModel> ShowRadioStationDetails { get; private set; }

        public RadioHomePageViewModel()
        {
            RadioModelCollection = new ObservableCollection<RadioModel>() {
                new RadioModel {
                StationName = "USA 1000hit",
                StationBio = "USA Radio Hip Hop Songs 24/7",
                StationLocation = "USA",
                StationURL = "http://gotradio-edge1.cdnstream.com/1159_128"
                }
                ,
                 new RadioModel {
                StationName = "234 Radio",
                StationBio = "Nigeria Radio Hip Hop Songs 24/7",
                StationLocation = "Nigeria",
                StationURL = "https://streaming.radio.co/s7f3695a64/listen" }
                ,
                 new RadioModel {
                StationName = "1.FM - Jamz",
                StationBio = "Hip HOP Jammin' Hip Hop",
                StationLocation = "Sweden",
                StationURL = "http://sc1c-sjc.1.fm:8052" }
            };
            OpenFolderCommand = new DelegateCommand<RadioModel>(PlayRadioStationHandler);
            ShowRadioStationDetails = new DelegateCommand<RadioModel>(ShowRadioStationDetailsHandler);
            EditorAditRadioStation = new DelegateCommand<RadioModel>(EditorAditRadioStationHandler);
        }

        private void EditorAditRadioStationHandler(RadioModel obj)
        {
            EditOrAddRadioStream editOrAddRadioStream = new EditOrAddRadioStream(obj);
            editOrAddRadioStream.OnFinished += (s, e) =>
            {
                var radiomodel = e.RadioModel;
                if (radiomodel != null)
                {
                    if (obj != null)
                    {
                        obj = e.RadioModel;
                        return;
                    }
                    RadioModelCollection.Add(radiomodel);
                }
            };
            editOrAddRadioStream.ShowDialog();
        }

        private void ShowRadioStationDetailsHandler(RadioModel obj)
        {
            
        }

        private void PlayRadioStationHandler(RadioModel obj)
        {
            IRadioService.PlayRadio(obj);
        }

        IRadioService IRadioService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioService>();
            }
        }
    }
}
