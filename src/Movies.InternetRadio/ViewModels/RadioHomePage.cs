using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.InternetRadio.Interfaces;
using Movies.InternetRadio.StreamManager;
using Movies.InternetRadio.Views;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MovieServices.Services;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Movies.InternetRadio.ViewModels
{
    public class RadioHomePageService : NotificationObject, IRadioPageService
    {
        IRadioService IRadioService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IRadioService>();
            }
        }

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

        static RadioHomePageService _currentInstance;
        private static RoutedCommand openfoldercommand;
        private static RoutedCommand editoraditradiostation;
        private static RoutedCommand showradiostationdetails;
        private static RoutedCommand checkfavoriteitemcommand;
        private bool isloading; 

        private ObservableCollection<IMoviesRadio> radioModelCollection;
        private  DelegateCommand addstationgroup; 
        private  DelegateCommand<IRadioGroup> openhomepagecommand; 
        private  DelegateCommand<IRadioGroup> openfavoritepagecommand; 
        private static RoutedCommand deletestationgroup;

        private INavigatorService NavigatorService
        {
            get { return ServiceLocator.Current.GetInstance<INavigatorService>(); }
        }

        private IRadioGroup currentRadioGroup;

        public IRadioGroup CurrentRadioGroup
        {
            get { return currentRadioGroup; }
            set
            {
                currentRadioGroup = value;
                RaisePropertyChanged(() => this.CurrentRadioGroup);
            }
        }

        public IRadioGroup FavoriteRadioGroup
        {
            get; private set;
        }

        public IRadioGroup StyleRadioGroup
        {
            get;
            private set;
        }

        public IRadioGroup CountryRadioGroup
        {
            get;
            private set;
        }

        public bool IsLoading
        {
            get { return isloading; }
            set { isloading = value; RaisePropertyChanged(() => this.IsLoading); }
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

        public static RoutedCommand OpenFolderCommand
        {
            get { return openfoldercommand; }
        }

        public static RoutedCommand CheckFavoriteItemCommand
        {
            get { return checkfavoriteitemcommand; }
        }

        public static RoutedCommand EditorAditRadioStation
        {
            get { return editoraditradiostation; }
        }

        public static RoutedCommand ShowRadioStationDetails
        {
            get { return showradiostationdetails; }
        }

        public DelegateCommand AddStationGroup
        {
            get { return addstationgroup; }
        }

        public DelegateCommand<IRadioGroup> OpenHomePageCommand
        {
            get { return openhomepagecommand; }
        }

        public DelegateCommand<IRadioGroup> OpenFavoritePageCommand
        {
            get { return openfavoritepagecommand; }
        }
        
        public static RoutedCommand DeleteStationGroup
        {
            get { return deletestationgroup; }
        }

        static RadioHomePageService()
        {
            InitializeCommands();
            RegisterCommands();
        }

        private static void InitializeCommands()
        {
            openfoldercommand = new RoutedCommand();
            editoraditradiostation = new RoutedCommand();
            showradiostationdetails = new RoutedCommand();
            deletestationgroup = new RoutedCommand();
            checkfavoriteitemcommand = new RoutedCommand();
        }

        private static void RegisterCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(Page),(new CommandBinding(openfoldercommand, OpenFile_Executed)));
            CommandManager.RegisterClassCommandBinding(typeof(Page),(new CommandBinding(CheckFavoriteItemCommand, CheckFavoriteItemCommand_Executed)));
            CommandManager.RegisterClassCommandBinding(typeof(Page),(new CommandBinding(editoraditradiostation, EditFile_Executed, DeleteFile_Enabled)));
            CommandManager.RegisterClassCommandBinding(typeof(Page), (new CommandBinding(showradiostationdetails, ShowDetails_Executed)));
            CommandManager.RegisterClassCommandBinding(typeof(Page), (new CommandBinding(deletestationgroup, DeleteFile_Executed,DeleteFile_Enabled)));
        }

        private static void CheckFavoriteItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if(sender is RadioFavoritePage)
            {
                IRadioPageService radioPageService = ((sender as Page).DataContext as IRadioPageService);
                if (radioPageService != null)
                    radioPageService.Remove(e.Parameter as IMoviesRadio);
                return;
            }
            _currentInstance.ToggleFavoriteRadioGroup((e.Parameter as IMoviesRadio));
        }
        
        private static void OpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if((e.Parameter as IMoviesRadio) != null)
                _currentInstance.OpenRadioStationHandler(e.Parameter as IMoviesRadio);
        }

        private static void EditFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IRadioPageService radioPageService = ((sender as Page).DataContext as IRadioPageService);
            if (radioPageService != null)
                _currentInstance.EditorAditRadioStationHandler(radioPageService,e.Parameter as IMoviesRadio);
        }
        
        private static void ShowDetails_Executed(object sender, ExecutedRoutedEventArgs e)
        {
           
        }

        private static void DeleteFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IRadioPageService radioPageService = ((sender as Page).DataContext as IRadioPageService);
            if(radioPageService != null)
                radioPageService.Remove(e.Parameter as IMoviesRadio);
        }

        private static void DeleteFile_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((e.Parameter as IMoviesRadio) != null)
                e.CanExecute = (e.Parameter as IMoviesRadio).CanEdit;
            else
                e.CanExecute = false;
        }
        
        private static void ShowRadioStationAction(IMoviesRadio sender)
        {
            _currentInstance.ShowRadioStationDetailsHandler(sender);
        }

        public RadioHomePageService()
        {
            addstationgroup = new DelegateCommand(AddStationAction);
            openhomepagecommand = new DelegateCommand<IRadioGroup>(OpenHomePageAction);
            openfavoritepagecommand = new DelegateCommand<IRadioGroup>(OpenFavoritePageAction);
            RadioModelCollection = new ObservableCollection<IMoviesRadio>();
            _currentInstance = this;

            ReadyRadioData();
        }

        private void ReadyRadioData()
        {
            var radioDataService = IRadioDataService as RadioDataService;

            if (IRadioDataService.Count() < 5)
                radioDataService.LoadDefaultRadioData();
            
            CurrentRadioGroup = radioDataService.GetHomeGroup("Home-Radio Station");
            CountryRadioGroup = radioDataService.GetHomeGroup("RadioStation-Country");
            StyleRadioGroup = radioDataService.GetHomeGroup("RadioStation-Styles");
            FavoriteRadioGroup = radioDataService.GetHomeGroup("RadioStation-Favorites");
        }

        private void OpenFavoritePageAction(IRadioGroup obj)
        {
            NavigatorService.NavigatePage(new RadioFavoritePage(NavigatorService.NavigationService), obj);
        }

        private void LoadPageData()
        {
            IsLoading = true;
            Task.Factory.StartNew(() => {
                return IRadioDataService.GetRadioObjectsFromKeys(CurrentRadioGroup.RadioStations);
            }).ContinueWith(t=> 
            {
                RadioModelCollection = t.Result;
                IsLoading = false;

            }, TaskScheduler.FromCurrentSynchronizationContext());
            RadioModelCollection = new ObservableCollection<IMoviesRadio>(SortService.SortList(Enums.SortType.Name, RadioModelCollection));
        }

        private void ToggleFavoriteRadioGroup(IMoviesRadio imoviesradio)
        {
            var iradioModel = imoviesradio as IRadioModel;
            if (iradioModel != null)
            {
                if (!iradioModel.IsFavorite)
                    FavoriteRadioGroup.AddStation(imoviesradio.Key);
                else
                    FavoriteRadioGroup.RemoveStation(imoviesradio.Key);

                iradioModel.IsFavorite = !iradioModel.IsFavorite;
            }
        }

        private void EditorAditRadioStationHandler(IRadioPageService radioPageService, IMoviesRadio moviesRadio)
        {
            IEditStation IEditStation;
            if (moviesRadio.FileType == Enums.GroupCatergory.Grouped)
                IEditStation = new EditStationGroup();
            else
                IEditStation = new EditStation();

            IEditStation.CurrentRadioStation = (IMoviesRadio)(moviesRadio as ICloneable);
            (IEditStation as UserControl).Measure(new Size(double.MaxValue,double.MaxValue));
            ComponentDocker componentDocker = new ComponentDocker()
            {
                Content = IEditStation,
                DialogHeight = (IEditStation as UserControl).DesiredSize.Height + 80,
                DialogWidth = (IEditStation as UserControl).DesiredSize.Width + 200,
                DialogTitle = string.Format("Edit - {0}", IEditStation.CurrentRadioStation.RadioName)
            };
            componentDocker.OnFinished += (s, e) =>
            {
                if (s != null)
                {
                    var indexMovie = radioPageService.RadioModelCollection.IndexOf(moviesRadio);
                    radioPageService.RadioModelCollection.RemoveAt(indexMovie);
                    radioPageService.RadioModelCollection.Insert(indexMovie, (s as IEditStation).CurrentRadioStation);
                }
            };
            componentDocker.ShowDialog();
        }

        internal void Onloaded(object sender)
        {
            Page page = sender as Page;
            if(radioModelCollection.Count == 0)
                LoadPageData();
        }

        private void ShowRadioStationDetailsHandler(IMoviesRadio obj)
        {
            
        }

        private void OpenRadioStationHandler(IMoviesRadio obj)
        {
            if (obj.FileType == Enums.GroupCatergory.Grouped)
            {
                NavigatorService.NavigatePage(new RadioViewPage(NavigatorService.NavigationService), obj);
                return;
            }
            IRadioService.PlayRadio(obj as RadioModel);
        }

        private void OpenHomePageAction(IRadioGroup obj)
        {
            NavigatorService.NavigatePage(new RadioViewPage(NavigatorService.NavigationService), obj);
        }

        private void AddStationAction()
        {
            RadioGroup radioGroup = RadioGroup.GetNewRadioStation();
            this.Add(radioGroup);
        }

        private void Add(IMoviesRadio radioGroup)
        {
            if (!RadioModelCollection.Contains(radioGroup))
            {
                this.RadioModelCollection.Add(radioGroup);
                IRadioDataService.AddRadio(CurrentRadioGroup,radioGroup);
            }
        }

        void IRadioPageService.Add(IMoviesRadio moviesRadio)
        {
            //do nothing
        }

        public void Remove(IMoviesRadio moviesRadio)
        {
            if (RadioModelCollection.Contains(moviesRadio))
            {
                this.RadioModelCollection.Remove(moviesRadio);
                if(moviesRadio is IRadioGroup)
                {
                    var group = (moviesRadio as IRadioGroup);
                    var collections = new List<Guid>((moviesRadio as IRadioGroup).RadioStations);
                    foreach (var item in collections)
                    {
                        IRadioDataService.RemoveRadio(group, item);
                    }
                }
                IRadioDataService.RemoveRadio(CurrentRadioGroup,moviesRadio.Key);
            }
        }
    }
}
