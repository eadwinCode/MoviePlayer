using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using VirtualizingListView.Pages.Util;

namespace VirtualizingListView.Pages.ViewModel

{
    public class FilePageViewModel : NotificationObject, IFilePageViewModel
    {
        private readonly Dispatcher FilePageDispatcher;
        private VideoFolder currentvideofolder;
        private MovieItemProvider ItemProvider;
        private bool isloading;
        private DataTemplateSelector mytemplatechange;
        private Style listviewstyle;
        private HamburgerMenuIconItem hamburgermenuicon;
        private object Padlock = new object();

        protected INavigatorService NavigatorService
        {
            get { return ServiceLocator.Current.GetInstance<INavigatorService>(); }
        }

        protected IBackgroundService BackgroundService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IBackgroundService>();
            }
        }

        protected IApplicationService ApplicationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationService>();
            }
        }

        protected IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }

        protected IFileLoader FileLoader
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoader>();
            }
        }

        protected IOpenFileCaller OpenFileCaller
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IOpenFileCaller>();
            }
        }

        public HamburgerMenuIconItem HamburgerMenuIcon
        {
            get { return hamburgermenuicon; }
            set { hamburgermenuicon = value; RaisePropertyChanged(() => this.HamburgerMenuIcon); }
        }

        public Style ListViewStyle
        {
            get { return listviewstyle; }
            set { listviewstyle = value; RaisePropertyChanged(() => this.ListViewStyle); }
        }

        public VideoFolder CurrentVideoFolder
        {
            get { return currentvideofolder; }
            set
            {
                currentvideofolder = value;
                RaisePropertyChanged(() => this.VideoItemViewCollection);
                RaisePropertyChanged(() => this.CurrentVideoFolder);
                if (value.OtherFiles != null)
                    UpdateViewCollection();
            }
        }
        public ObservableCollection<VideoFolder> VideoItemViewCollection
        {
            get { return CurrentVideoFolder == null ? null : CurrentVideoFolder.OtherFiles; }
        }
        public DelegateCommand<object> OpenFolderCommand { get; private set; }

        private SortType activesorttype = SortType.Date;
        public SortType ActiveSortType
        {
            get { return activesorttype; }
            set
            {
                activesorttype = value;
                RaisePropertyChanged(() => this.ActiveSortType);
                CheckSort();
            }
        }

        private ViewType activeType;
        public ViewType ActiveViewType
        {
            get
            {
                return activeType;
            }
            set
            {
                if (TemplateToggleAction())
                {
                    UpdateView(value);
                }
                activeType  = ApplicationService.AppSettings.ViewType = value;
                this.RaisePropertyChanged(() => this.ActiveViewType);
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

        private void UpdateView(ViewType value)
        {
            if (value == ViewType.Large)
            {
                MyTemplateChange = new MoreTemplateSelector();
                ListViewStyle = (Style)Application.Current.FindResource("lvStyle1");
            }
            else
            {
                MyTemplateChange = new ItemListSelector();
                ListViewStyle = (Style)Application.Current.FindResource("listViewControl");
            }
           // UpdateViewCollection();
        }

        public bool IsLoading
        {
            get { return isloading; }
            set { isloading = value; RaisePropertyChanged(() => this.IsLoading); }
        }

        public FilePageViewModel(NavigationService navigationService, Dispatcher dispatcher)
        {
            this.FilePageDispatcher = dispatcher;
            navigationService.LoadCompleted += NavigationService_LoadCompleted;
            Init();
        }

        public FilePageViewModel(Dispatcher dispatcher)
        {
            this.FilePageDispatcher = dispatcher;
            Init();
        }

        private void Init()
        {
            OpenFolderCommand = new DelegateCommand<object>(OpenFolderCommandAction);
            activeType = ApplicationService.AppSettings.ViewType;
            UpdateView(this.ActiveViewType);
        }

        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            VideoFolder videoFolder = (VideoFolder)e.ExtraData;
            FileWatceherSubscription(videoFolder);
            HamburgerMenuIcon = new HamburgerMenuIconItem()
            {
                Label = videoFolder.Name,
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.FolderOpen }
            };
            this.IsLoading = true;

            AsynLoadData(videoFolder, String.Format("Updating files in {0}", videoFolder.Name));

            NavigatorService.NavigationService.LoadCompleted -= NavigationService_LoadCompleted;
        }

        protected void FileWatceherSubscription(VideoFolder videoFolder)
        {
            videoFolder.OnFileWatcherUpdate -= VideoFolder_OnFileWatcherUpdate;
            videoFolder.OnFileWatcherUpdate += VideoFolder_OnFileWatcherUpdate;
        }

        private void VideoFolder_OnFileWatcherUpdate(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(() => this.VideoItemViewCollection);
            UpdateViewCollection();
        }

        protected void AsynLoadData(VideoFolder videoFolder,string message,Action callback = null)
        {
            BackgroundService.Execute(() =>
            {
                Thread.Sleep(1000);
                GetVideoFolder(videoFolder);
            }, message, () =>
            {
                this.IsLoading = false;
                OnDataLoaded(videoFolder);
                if (callback != null)
                    callback();
            });
        }

        private bool TemplateToggleAction()
        {
            if (VideoItemViewCollection != null)
            {
                DataTemplateSelector sed = MyTemplateChange;
                if (sed.GetType() == new ItemListSelector().GetType())
                {
                    ApplicationService.AppSettings.ViewType = ViewType.Large;
                }
                else
                {
                    ApplicationService.AppSettings.ViewType = ViewType.Small;
                }

                this.UpdateViewCollection();
                return true;
            }
            return false;
        }


        private void OnDataLoaded(VideoFolder result)
        {
            this.CurrentVideoFolder = result;
        }

        protected VideoFolder GetVideoFolder(VideoFolder obj)
        {
            if (obj.HasCompleteLoad) return obj;
            return FileLoader.InitGetAllFiles(obj);
        }

        private void UpdateViewCollection()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(5000)
            };
            dispatcherTimer.Tick += (s, e) =>
            {
                dispatcherTimer.Stop();
                if (VideoItemViewCollection == null) return;
                this.IsLoading = true;
                BackgroundService.Execute(() =>
                {
                    lock (Padlock)
                    {
                        LoaderCompletion.FinishCollectionLoadProcess(VideoItemViewCollection, FilePageDispatcher);
                    }
                }, String.Format("Checking and updating files info in {0}", CurrentVideoFolder.Name), () => {
                    this.IsLoading = false;
                });
            };
            dispatcherTimer.Start();
        }

        private void PageChangeEvent_Execute(Object s)
        {
            if (currentvideofolder.HasCompleteLoad)
            {

            }
            if (ItemProvider != null)
                ItemProvider.CompleteLoad(s);
        }

        private void CheckSort()
        {
            if (CurrentVideoFolder == null || CurrentVideoFolder.SortedBy == ActiveSortType) return;

            CurrentVideoFolder = FileLoader.SortList(ActiveSortType, CurrentVideoFolder);
            RaisePropertyChanged(() => this.VideoItemViewCollection);
        }

        private void OpenFolderCommandAction(object obj)
        {
            VideoFolder videoFolder = (VideoFolder)obj;
            if (videoFolder.Exists)
            {
                if (videoFolder.FileType == GroupCatergory.Grouped)
                {
                    this.NavigatorService.NavigatePage(new FilePageView(this.NavigatorService.NavigationService), obj);
                }
                else
                    OpenFileCaller.Open(videoFolder as IPlayable, CurrentVideoFolder.OtherFiles.OfType<IPlayable>());
            }
            else
            {
                while (this.NavigatorService.NavigationService.CanGoBack)
                {
                    if (NavigatorService.NavigationService.Content is IMainPage) break;
                    this.NavigatorService.NavigationService.GoBack();
                }
            }
        }
    }
}