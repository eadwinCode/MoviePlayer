using Common.Util;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using VideoComponent.BaseClass;
using VirtualizingListView.Pages.Util;
using Microsoft.Practices.ServiceLocation;
using PresentationExtension.InterFaces;
using System.Windows.Threading;

namespace VirtualizingListView.Pages.ViewModel

{
    public class FilePageViewModel : NotificationObject, IFilePageViewModel
    {
        private readonly Dispatcher FilePageDispatcher;
        private NavigationService navigationService;
        private VideoFolder currentvideofolder;
        private MovieItemProvider ItemProvider;
        private bool isloading;
        private DataTemplateSelector mytemplatechange;
        private Style listviewstyle;
        private HamburgerMenuIconItem hamburgermenuicon;
        private object Padlock = new object();

        IBackgroundService BackgroundService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IBackgroundService>();
            }
        }

        IApplicationService ApplicationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationService>();
            }
        }
        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }
        IFileLoader fileLoader
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoader>();
            }
        }
        IOpenFileCaller openFileCaller
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
            this.navigationService = navigationService;
            this.navigationService.LoadCompleted += NavigationService_LoadCompleted;
            OpenFolderCommand = new DelegateCommand<object>(OpenFolderCommandAction);
            activeType = ApplicationService.AppSettings.ViewType;
            UpdateView(this.ActiveViewType);
        }

        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            VideoFolder videoFolder = (VideoFolder)e.ExtraData;
            HamburgerMenuIcon = new HamburgerMenuIconItem()
            {
                Label = videoFolder.Name,
                Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.FolderOpen }
            };

            BackgroundService.Execute(() =>
            {
                this.IsLoading = true;
                GetVideoFolder(videoFolder);
            }, String.Format("Updating files in {0}", videoFolder.Name), () => {
                this.IsLoading = false;
                OnDataLoaded(videoFolder);
            });
            
            navigationService.LoadCompleted -= NavigationService_LoadCompleted;
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

        private VideoFolder GetVideoFolder(VideoFolder obj)
        {
            if (obj.HasCompleteLoad) return obj;
            return fileLoader.LoadParentFiles(obj, ActiveSortType);
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
                BackgroundService.Execute(() =>
                {
                    lock (Padlock)
                    {
                        this.IsLoading = true;
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

            CurrentVideoFolder = fileLoader.SortList(ActiveSortType, CurrentVideoFolder);
            RaisePropertyChanged(() => this.VideoItemViewCollection);
        }

        private void OpenFolderCommandAction(object obj)
        {
            VideoFolder videoFolder = (VideoFolder)obj;
            if (videoFolder.Exists)
            {
                if (videoFolder.FileType == FileType.Folder)
                {
                    this.navigationService.Navigate(new FilePageView(this.navigationService), obj);
                }
                else
                    openFileCaller.Open(videoFolder as IVideoData);
            }
            else
            {
                while (this.navigationService.CanGoBack)
                {
                    if (navigationService.Content is IMainPage) break;
                    this.navigationService.GoBack();
                }
            }
        }
    }
}