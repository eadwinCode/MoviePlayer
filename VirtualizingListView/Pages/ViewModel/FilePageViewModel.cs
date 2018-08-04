using Common.FileHelper;
using Common.Util;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using VideoComponent.BaseClass;
using VirtualizingListView.Pages.Util;
using VirtualizingListView.Util;

namespace VirtualizingListView.Pages.ViewModel

{
    public class FilePageViewModel : NotificationObject, IFilePageViewModel
    {
        private NavigationService navigationService;
        private VideoFolder currentvideofolder;
        private MovieItemProvider ItemProvider;
        private ObservableCollection<VideoFolder> videoitemviewcollection;
        private bool isloading;
        private DataTemplateSelector mytemplatechange;
        private Style listviewstyle;
        private HamburgerMenuIconItem hamburgermenuicon;

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

        private SortType activesorttype = SortType.Extension;
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

        private ViewType activeType = ApplicationService.AppSettings.ViewType;

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
                activeType = ApplicationService.AppSettings.ViewType = value;
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

        public FilePageViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.navigationService.LoadCompleted += NavigationService_LoadCompleted;
            OpenFolderCommand = new DelegateCommand<object>(OpenFolderCommandAction);
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

            FileLoaderCompletion fileCompletionLoader = new FileLoaderCompletion();
            var task = FileLoaderCompletion.CurrentTaskExecutor.CreateTask(() => {
                this.IsLoading = true;
                GetVideoFolder(videoFolder);
            }, "Getting Folder Files", () =>
            {
                this.IsLoading = false;
                OnDataLoaded(videoFolder);
            });

            FileLoaderCompletion.CurrentTaskExecutor.Execute(task);
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
            ActiveSortType = CurrentVideoFolder.SortedBy;
        }

        private VideoFolder GetVideoFolder(VideoFolder obj)
        {
            if (obj.HasCompleteLoad) return obj;
            return FileLoader.FileLoaderInstance.LoadParentFiles(obj, ActiveSortType);
        }

        private void UpdateViewCollection()
        {
            if (VideoItemViewCollection == null) return;
            FileLoaderCompletion fileCompletionLoader = new FileLoaderCompletion();
            var task = FileLoaderCompletion.CurrentTaskExecutor.CreateTask(() =>
            {
                lock (this)
                {
                    this.IsLoading = true;
                    fileCompletionLoader.FinishCollectionLoadProcess(VideoItemViewCollection);
                }
            }, "Rounding Up File loading.", () =>
            {
                this.IsLoading = false;
            });

            FileLoaderCompletion.CurrentTaskExecutor.Execute(task);
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

            CurrentVideoFolder = FileLoader.FileLoaderInstance.SortList(ActiveSortType, CurrentVideoFolder);
            RaisePropertyChanged(() => this.VideoItemViewCollection);
        }

        private void OpenFolderCommandAction(object obj)
        {
            VideoFolder videoFolder = (VideoFolder)obj;
            if (videoFolder.FileType == FileType.Folder)
            {
                this.navigationService.Navigate(new FilePageView(this.navigationService), obj);
            }
            else
                FileOpenCall.Open(videoFolder);
        }
    }
}