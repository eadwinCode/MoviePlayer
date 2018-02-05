using Common.ApplicationCommands;
using Common.Interfaces;
using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VideoComponent.BaseClass;
using VideoComponent.Command;
using VirtualizingListView.Model;
using VirtualizingListView.OtherFiles;
using VirtualizingListView.Util;
using Common.FileHelper;

namespace VirtualizingListView.ViewModel
{
    public class CollectionViewModel : NotificationObject, ICollectionViewModel
    {
        #region Fields

        private DataTemplateSelector mytemplatechange = new MoreTemplateSelector();
        private VideoItemViewCollection<VideoFolder> videoitemviewcollection;
        private int activeindex = 0;
        private List<NavigationModel> Navigation;
        private List<string> comboxstring;
        private string comboxSelectedItem;
        //readonly IEventAggregator _aggregator;
        private string currentdir;
        private VideoFolder videodataaccess;
        private double loadinprogress;
        //private bool IsLoaded = false;
       // private bool SortonLoad;
        //private DirectoryInfo directorypos;
        //private bool next;
       // private bool previous;
        private bool _isloading;
        private Style listviewstyle;
        public static LastSeenHelper LastSeen;
        public CustomItemProvider ItemProvider;
        public event EventHandler CloseFileExporerEvent;
        private DelegateCommand fileaccess;
        private IFileExplorer ifileExplorer;
        //private ICollectionView myVideoDataFilter;

        internal void GetFileExplorerInstance(IFileExplorer ifileExplorer){
            this.ifileExplorer = ifileExplorer;
            (ifileExplorer as UserControl).Loaded += CollectionViewModel_Loaded;
        }

        

        #endregion

        #region Properties

        public delegate void ExecuteCommand(object sender);

        public ExecuteCommand ViewChanged;

        public DelegateCommand Next { get; private set; }

        public DelegateCommand Refresh { get; private set; }

        public DelegateCommand Previous { get; private set; }

       

        private static CollectionViewModel instance = new CollectionViewModel();

        public static CollectionViewModel Instance { get { return instance; } }

        public VideoItemViewCollection<VideoFolder> VideoItemViewCollection
        {
            get { return videoitemviewcollection; }
            set { videoitemviewcollection = value;
               
                RaisePropertyChanged(() => this.VideoItemViewCollection); }
        }    

        private SortType SortType { get; set; }

        public ICommand PlayMovie { get; private set; }

        public ICommand TemplateToggle
        {
            get;
            private set;
        }

        public bool IsLoading
        {
            get { return _isloading; }
            set { _isloading = value; RaisePropertyChanged(() => this.IsLoading); }
        }

        public double LoadingProgress
        {
            get { return loadinprogress; }
            set { loadinprogress = value; RaisePropertyChanged(() => this.LoadingProgress); }
        }

        public Style ListViewStyle
        {
            get { return listviewstyle; }
            set { listviewstyle = value; RaisePropertyChanged(() => this.ListViewStyle); }
        }

        public VideoFolder VideoDataAccess
        {
            get { return videodataaccess; }
            set
            {
                videodataaccess = value;
                if (value.OtherFiles != null)
                    UpdateViewCollection();
                //RaisePropertyChangedEvent("VideoItem");
            }
        }

        public DirectoryInfo DirectoryPosition
        {
            get { return ApplicationService.AppSettings.LastDirectory; }
            set
            {
                ApplicationService.AppSettings.LastDirectory = value; 
                CurrentDir = value.Name + " - " 
                    + value.FullName;
            }
        }

        public List<string> ComboxString
        {
            get { return comboxstring; }
            set { comboxstring = value; RaisePropertyChanged(() => this.ComboxString); }
        }

        public string ComboxSelectedItem
        {
            get { return comboxSelectedItem; }
            set
            {
                comboxSelectedItem = value;
                if (value != null)
                {
                    if (value == "Date")
                    {
                        SortType = SortType.Date;

                    }
                    else if (value == "Ext")
                    {
                        SortType = SortType.Extension;
                    }
                    else
                    {
                        SortType = SortType.Name;
                    }

                    CheckSort();

                }
                RaisePropertyChanged(() => this.ComboxString);
            }
        }

        public string CurrentDir
        {
            get { return currentdir; }
            set { currentdir = value; RaisePropertyChanged(() => this.CurrentDir); }
        }

        public ViewType ViewType
        {
            get
            {
                return ApplicationService.AppSettings.ViewType;
            }
            set
            {
                ApplicationService.AppSettings.ViewType = value;
                UpdateView(value);
                //if (ViewChanged != null)
                //{
                //    ViewChanged(value);
                //}

            }
        }

        public DelegateCommand CloseFileExplorer
        {
            get
            {
                if (fileaccess == null)
                {
                    fileaccess = new DelegateCommand(() =>
                    {
                        CloseFileExplorerAction();
                    });
                }
                return fileaccess;
            }
        }
        private DelegateCommand cancelsearchbtn;
        public DelegateCommand CancelSearchBtn {
            get
            {
                if (cancelsearchbtn == null)
                {
                    cancelsearchbtn = new DelegateCommand(() =>
                    {
                       CancelSearchAction();
                    }, CanCancelSearch);
                }
                return cancelsearchbtn;
            }
        }

        public DataTemplateSelector MyTemplateChange
        {
            get { return mytemplatechange; }
            set
            {
                mytemplatechange = value;
                this.RaisePropertyChanged(() => this.MyTemplateChange);
            }
        }
        #endregion

        public CollectionViewModel()
        {
            // _aggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            // _aggregator.GetEvent<LoadViewExecuteCommandEvent>().Subscribe(LoadViewUpdate); 
            // _aggregator.GetEvent<LoadExecuteCommandEvent>().Subscribe(OnVideoItemSelected);
            LastSeen = new LastSeenHelper();
            InitCombox();
            //IsLoaded = false;
            PlayMovie = new RelayCommand(PlayMovieAction);
            TemplateToggle = new RelayCommand(() => TemplateToggleAction());
            Next = new DelegateCommand(() => Next_Action(), CanNextExecute);
            Previous = new DelegateCommand(() => Previous_Action(), CanPreviousExecute);
            Refresh = new DelegateCommand(() => Refresh_Action(), CanRefresh);
            UpdateView(this.ViewType);
            //  VideoItemPanel
            //  var sds = Application.Current;
        }

        private Visibility issearchbtnvisible= Visibility.Collapsed;

        public Visibility IsSearchBtnVisible
        {
            get { return issearchbtnvisible; }
            set { issearchbtnvisible = value; RaisePropertyChanged(() => this.IsSearchBtnVisible); }
        }


        private bool CanCancelSearch()
        {
            return Search != null;
        }

        private void CancelSearchAction()
        {
            Search = null;
            CloseSearch = true;
            ResetSource();
            IsSearchInit = false;
            if (VideoDataAccess != null)
            {
                InitSearchItems();
            }
            //ListVideos = Visibility.Visible;
            //NoSearchfound = Visibility.Collapsed;
        }
        
        private bool CanRefresh()
        {
            return this.CurrentDir != null;
        }

        private void Refresh_Action()
        {
            if (Search != null)
            {
                CancelSearchAction();
            }
            this.VideoDataAccess = new VideoFolder(DirectoryPosition.FullName);
            this.VideoDataAccess = FileLoader.LoadParentFiles(VideoDataAccess, SortType, this);
            var Navigate = Navigation.First(x => x.Dir == DirectoryPosition);
            if (Navigate != null)
            {
                Navigate.VideoData = VideoDataAccess;
            }
            //Navigation.Add(new NavigationModel { Dir = DirectoryPosition, VideoData = s });
        }

        public void CloseFileExplorerAction(object sender = null)
        {
            CloseFileExporerEvent?.Invoke(sender, new EventArgs());
        }

        private void UpdateViewCollection()
        {
            //if (ItemProvider == null)
            ItemProvider = new CustomItemProvider(this);

            VideoItemViewCollection = null;
            VideoItemViewCollection = new VideoItemViewCollection<VideoFolder>(ItemProvider, 0, 1000);
            videoitemviewcollection.PageIndexChangedEvent += PageChangeEvent_Execute;
            //VideoItemViewCollection
            //_aggregator.GetEvent<LoadViewExecuteCommandEvent>().Publish(ItemProvider);
        }

        private void PageChangeEvent_Execute(Object s)
        {
            if (videodataaccess.HasCompleteLoad)
            {

            }
            ItemProvider.CompleteLoad(s);
        }
        
        public void TreeViewUpdate(string obj)
        {
            if (Search != null)
            {
                CancelSearchAction();
            }
            Task.Factory.StartNew(() => NewMethod(obj))
                .ContinueWith(t => this.VideoDataAccess = t.Result,
                TaskScheduler.FromCurrentSynchronizationContext()).Wait(200);
            //StartLoadingProcedure(obj);
        }
        

        private VideoFolder NewMethod(string obj)
        {
            return StartLoadingProcedure(obj);
        }
        
        private void CheckSort()
        {
            if (VideoDataAccess == null || VideoDataAccess.SortedBy == SortType) return;

            VideoDataAccess = FileLoader.SortList(SortType, VideoDataAccess);
            RaisePropertyChanged(() => this.VideoItemViewCollection);
        }
        
        private void LoadViewUpdate(CustomItemProvider obj)
        {
            VideoItemViewCollection = new VideoItemViewCollection<VideoFolder>(obj, 0, 30 * 1000);
        }

        private void PlayMovieAction()
        {
            //throw new NotImplementedException();
        }

        private void DoubleCommandAction(object parameter)
        {

        }

        private void TemplateToggleAction()
        {
            if (VideoItemViewCollection != null)
            {
                DataTemplateSelector sed = MyTemplateChange;
                if (sed.GetType() == new itemListSelector().GetType())
                {
                    ViewType = ViewType.Large;
                }
                else
                {
                    ViewType = ViewType.Small;
                }

                this.UpdateViewCollection();
            }
        }

        private void UpdateView(ViewType value)
        {
            if (value == ViewType.Large)
            {
                MyTemplateChange = new MoreTemplateSelector();
                ListViewStyle = (Style)Application.Current.FindResource("lvStyle1");
            }
            else
            {
                MyTemplateChange = new itemListSelector();
                ListViewStyle = (Style)Application.Current.FindResource("listViewControl");
                //ListViewStyle = (Style)Application.Current.FindResource("lvStyle1");
            }
        }

        private void InitCombox()
        {
            ComboxString = new List<string>();
            ComboxString.Add("Date");
            ComboxString.Add("Name");
            ComboxString.Add("Ext");

            ComboxSelectedItem = ComboxString[0];
        }

        private VideoFolder StartLoadingProcedure(string path = null)
        {
            //Dispatcher.Invoke(new Action(delegate
            //{
            Reset();
            DirectoryPosition = new DirectoryInfo(path);
            VideoFolder s = FileLoader.LoadParentFiles(new VideoFolder(DirectoryPosition.FullName), SortType, this);
            Navigation.Add(new NavigationModel { Dir = DirectoryPosition, VideoData = s });
            CheckCanExecut();

            //}));
            return s;
        }

        private void Reset()
        {
            activeindex = 0;
            Navigation = new List<NavigationModel>();
            ItemProvider = null;
        }

        public void VideoComponentViewModel_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.IsLoaded = true;
            if (ApplicationService.AppSettings.LastDirectory != null)
            {
                TreeViewUpdate(ApplicationService.AppSettings.LastDirectory.FullName);
            }
        }

        private void CheckCanExecut()
        {
            //CommandManager.InvalidateRequerySuggested();

            //Next = CanNextExecute();
            //Previous = CanPreviousExecute();
            Next.RaiseCanExecuteChanged();
            Previous.RaiseCanExecuteChanged();
            Refresh.RaiseCanExecuteChanged();
        }

        public void Previous_Action()
        {
            if (Search != null)
            {
                CancelSearchAction();
            }
            int prev = activeindex - 1;
            DirectoryPosition = Navigation[prev].Dir;
            VideoDataAccess = Navigation[prev].VideoData;
            // this.VideoDataAccess = VideoDataAccessor.LoadParent(DirectoryPosition, SortType);
            activeindex--;
            CheckCanExecut();
            RefreshLink();
            CheckSort();
        }

        public void Next_Action()
        {
            if (Search != null)
            {
                CancelSearchAction();
            }
            int next = activeindex + 1;
            DirectoryPosition = Navigation[next].Dir;
            VideoDataAccess = Navigation[next].VideoData;
            // this.VideoDataAccess = VideoDataAccessor.LoadParent(DirectoryPosition, SortType);
            activeindex++;
            CheckCanExecut();
            RefreshLink();
            CheckSort();
        }

        public bool CanNextExecute()
        {
            if (Navigation == null)
            {
                return false;
            }
            if (activeindex == Navigation.Count - 1)
            {
                return false;
            }
            return true;
        }

        public bool CanPreviousExecute()
        {
            if (activeindex - 1 < 0)//|| activeindex > Navigation.Count - 1
            {
                return false;
            }

            return true;
        }

        public void OnVideoItemSelected(VideoFolder obj)
        {
            //if (obj.HasCompleteLoad)
                AddToNav(obj);
        }

        private void AddToNav(VideoFolder obj)
        {
            if (Search != null)
            {
                CancelSearchAction();
            }
            TrimNav();
            activeindex++;
            this.DirectoryPosition = obj.Directory;
            this.VideoDataAccess = obj;
            Navigation.Add(new NavigationModel { Dir = DirectoryPosition, VideoData = VideoDataAccess });
            CheckCanExecut();
            CheckSort();
            
        }

        private void RefreshLink()
        {
            //Dispatcher.Invoke(new Action(delegate
            //{
            //   VideoDataAccessor.RefreshPath(VideoDataAccess);
            //}));

        }

        //public void RefreshAction(VideoData obj)
        //{
        //  if (obj.intChildCount < VideoDataAccess.ChildFiles.Count)
        //  {
        //      VideoDataAccess = obj;
        //      VideoDataAccess.ChildFiles = VideoDataAccessor.SortList(SortType, VideoDataAccess);
        //      return;
        //  }

        //  VideoDataAccess.ChildFiles.AddRange(obj.ChildFiles);
        ////  VideoDataAccess.ChildCount = obj.ChildCount;
        //  VideoDataAccess.ChildFiles = VideoDataAccessor.SortList(SortType, VideoDataAccess);
        //  RaisePropertyChangedEvent("VideoItem");
        // }
        private void TrimNav()
        {
            if (activeindex == Navigation.Count - 1) return;

            for (int i = Navigation.Count - 1; i > activeindex; i--)
            {
                Navigation.RemoveAt(i);
            }
        }

        public object GetCollectionVM
        {
            get { return this; }
        }

        private string _search;
        private bool CloseSearch;
        private bool IsSearchInit = false;
        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                if(videoitemviewcollection != null && value != null)
                    DoASearch(value);

                IsSearchBtnVisible = value == null ? Visibility.Collapsed : Visibility.Visible;
                RaisePropertyChanged(() => this.Search);
                CancelSearchBtn.RaiseCanExecuteChanged();
            }
        }

        private void DoASearch(string value)
        {
            if (value != "")
            {

                if (!IsSearchInit)
                {
                    CheckCurrentView();
                    CloseSearch = false;
                    IsSearchInit = true;
                }
                Search_TextChanged();
            }
            else
            {
                if (!CloseSearch)
                {
                    Search_TextChanged();
                }

            }
        }

        public void Search_TextChanged()
        {
            //Window1.returneditems = 0;
            CollectionViewSource.GetDefaultView(IFileExplorer.FileExplorerListView.ItemsSource).Refresh();

            //if (Window1.returneditems == 0)
            //{
            //    if (URL.Search != "")
            //    {
            //        URL.View.Text = "No Movie found";
            //    }
            //    else
            //    {
            //        URL.View.Text = "Search for a Movie";
            //    }
            //    URL.View.confirmZ();
            //    URL.View.ShowCount = System.Windows.Visibility.Collapsed;
            //}
            //else
            //{
            //    URL.View.confirm();
            //    URL.View.Textfoundcount = Window1.returneditems;
            //    URL.View.ShowCount = System.Windows.Visibility.Visible;
            //}
        }

        public void CheckCurrentView()
        {
            //myVideoDataFilter = (CollectionView)CollectionViewSource.GetDefaultView(GetAllFiles(IFileExplorer.FileExplorerListView.ItemsSource));
            //myVideoDataFilter.Filter = new Predicate<object>(myVideofilter);
            InitSearchItems(true);
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(IFileExplorer.FileExplorerListView.ItemsSource);
            view.Filter = MyVideofilter;

            //IFileExplorer.FileExplorerListView.ItemsSource = myVideoDataFilter;
        }

        private void InitSearchItems(bool IsSearch = false)
        {
            
            if (IsSearch)
            {
                this.IsLoading = true;
                VideoDataAccess.Tag = VideoDataAccess.OtherFiles;
                VideoDataAccess.OtherFiles = GetAllFiles(VideoDataAccess.OtherFiles);
                this.IsLoading = false;
                UpdateViewCollection();
                this.ItemProvider.CompleteLoad(VideoDataAccess.OtherFiles);
            }
            else
            {
                if (VideoDataAccess.Tag == null)
                {
                    return;
                }
                this.VideoDataAccess.OtherFiles = VideoDataAccess.Tag as ObservableCollection<VideoFolder>;
                VideoDataAccess.Tag = null;
                //var dir = VideoDataAccess.Tag as DirectoryInfo;
                //Task.Factory.StartNew(() => VideoDataAccessor.LoadParent(VideoDataAccess, SortType, this))
                //    .ContinueWith(t => VideoDataAccess = t.Result,
                //    TaskScheduler.FromCurrentSynchronizationContext()).Wait(200);
                UpdateViewCollection();
            }

            Thread.SpinWait(VideoDataAccess.OtherFiles.Count);
        }

        private ObservableCollection<VideoFolder> GetAllFiles(IEnumerable itemsSource)
        {
            ObservableCollection<VideoFolder> allfile = new ObservableCollection<VideoFolder>();
            if (itemsSource != null)
            {

                foreach (VideoFolder item in itemsSource)
                {
                    if (item.FileType == FileType.Folder)
                    {
                        allfile.AddRange((ObservableCollection<VideoFolder>)GetAllFiles((IEnumerable)item.OtherFiles));
                    }
                    allfile.Add(item);
                }
            }

            return allfile;
        }
        

        private bool MyVideofilter(object item)
        {
            if (item == null) return false;
            if (String.IsNullOrEmpty(Search))
            {
                return true;
            }

            else
            {
                var vfc = ((item as VideoFolder).FileName.IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0);
                //if (vfc)
                //{
                //    (this.VideoItemViewCollection.ItemsProvider as CustomItemProvider).CompleteLoad(item);
                //}
                return vfc;
                //var search = (VideoData)item;
                //return (search.FileName.StartsWith(Search.Text, StringComparison.OrdinalIgnoreCase)
                //|| search.LastName.StartsWith(Search.Text, StringComparison.OrdinalIgnoreCase));
            }

        }

        public void ResetSource()
        {
            CollectionViewSource myvideoData = new CollectionViewSource();

            myvideoData = (CollectionViewSource)(IFileExplorer as UserControl).FindResource("myVideoData");
            Binding binding = new Binding
            {
                Source = myvideoData
            };
            BindingOperations.SetBinding( IFileExplorer.FileExplorerListView, ListBox.ItemsSourceProperty, binding);
        }

        public IFileExplorer IFileExplorer
        {
            get
            {
                return ifileExplorer;
            }
        }

        private void CollectionViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            //myVideoDataFilter = (CollectionView)CollectionViewSource.GetDefaultView(VideoItemViewCollection);
            //myVideoDataFilter.Filter = new Predicate<object>(myVideofilter);
        }

       

    }
}
