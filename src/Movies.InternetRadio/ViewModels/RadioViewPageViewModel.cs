using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Movies.InternetRadio.Interfaces;
using Movies.InternetRadio.StreamManager;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Movies.InternetRadio.ViewModels
{
    public class RadioViewPageViewModel : NotificationObject, IRadioPageService
    {
        private IRadioGroup currentRadioGroup;
        private NavigationService navigationService;
        private  DelegateCommand addstationgroup;
        private  DelegateCommand addradiostation;
        private  DelegateCommand importdatacommand;
        private bool isloading;
        private ObservableCollection<IMoviesRadio> radioModelCollection;
        IRadioDataService IRadioDataService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioDataService>();
            }
        }
        ISortService SortService
        {
            get { return ServiceLocator.Current.GetInstance<ISortService>(); }
        }

        public IRadioGroup CurrentRadioGroup
        {
            get { return currentRadioGroup; }
            set
            {
                currentRadioGroup = value;
                RaisePropertyChanged(() => this.CurrentRadioGroup);
            }
        }

        public bool IsLoading
        {
            get { return isloading; }
            set { isloading = value; RaisePropertyChanged(() => this.IsLoading); }
        }

        public  DelegateCommand AddStationGroup 
        {
            get { return addstationgroup; }
        }

        public  DelegateCommand AddRadioStation
        {
            get { return addradiostation; }
        }

        public DelegateCommand ImportDataCommand
        {
            get { return importdatacommand; }
        }

        public ObservableCollection<IMoviesRadio> RadioModelCollection
        {
            get { return radioModelCollection; }
            set
            {
                radioModelCollection = value;
                RaisePropertyChanged(() => this.RadioModelCollection);
            }
        }


        public RadioViewPageViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
            navigationService.LoadCompleted += NavigationService_LoadCompleted;
            addstationgroup = new DelegateCommand(AddStationAction);
            addradiostation = new DelegateCommand(AddRadioAction);
            importdatacommand = new DelegateCommand(ImportCommandAction);
            RadioModelCollection = new ObservableCollection<IMoviesRadio>();
        }

        private void ImportCommandAction()
        {
            string[] delimeter = { "," };
            string[] files = GetFile();
            if(files != null && files.Length > 0)
            {
                foreach (var item in files)
                {
                    string[] data = item.Split(delimeter, StringSplitOptions.None);
                    if (data.Length < 12)
                        continue;
                    var radioStation = CurrentRadioGroup.GetNewRadioModel() as IRadioModel;

                    radioStation.StationURL = data[0].Trim('\'');
                    radioStation.AudioFormat = data[1].Trim('\'');
                    int bitrate = 0;
                    int.TryParse(data[2].Trim('\''), out bitrate);
                    radioStation.BitRate = bitrate;

                    int.TryParse(data[3].Trim('\''),out bitrate);
                    radioStation.SampleRate = bitrate;

                    radioStation.RadioName = data[7];
                    radioStation.StationLocation = data[9];
                    radioStation.StationBio = data[11];
                    radioStation.Country = data[8];
                    radioStation.Genre = data[10];
                    IRadioDataService.AddRadio(CurrentRadioGroup,radioStation);
                }

                LoadData();
            }
        }

        private string[] GetFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var Dialogresult = openFileDialog.ShowDialog();
            if(Dialogresult == true)
            {
                string filepath = openFileDialog.FileName;
                return File.ReadAllLines(filepath);
            }
            return null;
        }

        private bool CanDeleteStation(IMoviesRadio arg)
        {
            return arg.CanEdit;
        }

        private void DeleteStationAction(IMoviesRadio obj)
        {
            RadioModelCollection.Remove(obj);
        }

        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            IRadioGroup iradiogroup = (IRadioGroup)e.ExtraData;
            this.CurrentRadioGroup = iradiogroup;
            navigationService.LoadCompleted -= NavigationService_LoadCompleted;
            LoadData();
        }

        private void LoadData()
        {
            IsLoading = true;
            Task.Factory.StartNew(() => {
                Thread.Sleep(1000);
                return IRadioDataService.GetRadioObjectsFromKeys(CurrentRadioGroup.RadioStations);
            }).ContinueWith(t =>
            {
                RadioModelCollection = t.Result;
                IsLoading = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void AddRadioAction()
        {
            var radioStation = this.CurrentRadioGroup.GetNewRadioModel() as IRadioModel;
            IRadioDataService.AddRadio(CurrentRadioGroup, radioStation);
            RadioModelCollection.Add(radioStation);
        }

        private void AddStationAction()
        {
            var NewStationgroup = RadioGroup.GetNewRadioStation();
            IRadioDataService.AddRadio(CurrentRadioGroup, NewStationgroup);
            RadioModelCollection.Add(NewStationgroup);

            RadioModelCollection = new ObservableCollection<IMoviesRadio>( SortService.SortList(Enums.SortType.Name, RadioModelCollection));
            //RaisePropertyChanged(()=>RadioModelCollection);
        }

        public void Add(IMoviesRadio moviesRadio)
        {
            if (!RadioModelCollection.Contains(moviesRadio))
            {
                IRadioDataService.AddRadio(CurrentRadioGroup, moviesRadio);
                RadioModelCollection.Add(moviesRadio);
            }
        }

        public void Remove(IMoviesRadio moviesRadio)
        {
            RadioModelCollection.Remove(moviesRadio);
            if (moviesRadio is IRadioGroup)
            {
                var group = (moviesRadio as IRadioGroup);
                var collections = new List<Guid>((moviesRadio as IRadioGroup).RadioStations);
                foreach (var item in collections)
                {
                    IRadioDataService.RemoveRadio(group, item);
                }
            }
            IRadioDataService.RemoveRadio(CurrentRadioGroup, moviesRadio.Key);
        }
    }
}
