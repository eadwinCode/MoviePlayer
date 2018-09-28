using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.InternetRadio.Interfaces;
using Movies.InternetRadio.StreamManager;
using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Movies.InternetRadio.ViewModels
{
    public class RadioFavoritePageViewModel : NotificationObject, IRadioPageService
    {
        private IRadioGroup currentRadioGroup;
        private bool isloading;
        private ObservableCollection<IMoviesRadio> radioModelCollection;

        IRadioDataService IRadioDataService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioDataService>();
            }
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

        public ObservableCollection<IMoviesRadio> RadioModelCollection
        {
            get { return radioModelCollection; }
            private set
            {
                radioModelCollection = value;
                RaisePropertyChanged(() => this.RadioModelCollection);
            }
        }

        public DelegateCommand ClearAllCommand { get; private set; }

        public RadioFavoritePageViewModel(NavigationService navigationService)
        {
            navigationService.LoadCompleted += NavigationService_LoadCompleted;
            RadioModelCollection = new ObservableCollection<IMoviesRadio>();
            ClearAllCommand = new DelegateCommand(ClearAllHandler);

        }

        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            IRadioGroup iradiogroup = (IRadioGroup)e.ExtraData;
            this.CurrentRadioGroup = iradiogroup;
            (sender as Frame).NavigationService.LoadCompleted -= NavigationService_LoadCompleted;
            LoadData();
        }

        private void ClearAllHandler()
        {
            this.CurrentRadioGroup.RadioStations.Clear();
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

        public void Add(IMoviesRadio moviesRadio)
        {
            if (!RadioModelCollection.Contains(moviesRadio))
            {
                IRadioDataService.AddRadio(CurrentRadioGroup, moviesRadio);
                CurrentRadioGroup.AddStation(moviesRadio.Key);
            }
        }

        public void Remove(IMoviesRadio moviesRadio)
        {
            RadioModelCollection.Remove(moviesRadio);
            CurrentRadioGroup.RemoveStation(moviesRadio.Key);
        }
    }
}
