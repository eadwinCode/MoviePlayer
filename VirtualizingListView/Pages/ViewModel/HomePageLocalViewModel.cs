using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using SearchComponent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using VideoComponent.BaseClass;
using VirtualizingListView.Pages.Model;
using VirtualizingListView.Pages.Util;
using VirtualizingListView.Util;
using VirtualizingListView.Util.AddFolderDialogWindow;

namespace VirtualizingListView.Pages.ViewModel
{
    public delegate void FileLoaderHandler();

    internal class HomePageLocalViewModel:NotificationObject,IDataSource<VideoFolder>
    {
        private NavigationService navigationService;
        private static ObservableCollection<VideoFolder> allfolders;
        private HomePageLocal pageOwner;
        private static bool isLoadingData;
        //private static VideoFolder HomePageFolderHost;
        public static FileLoaderHandler FileLoaderDelegate;
        public ObservableCollection<VideoFolder> AllFoldersList
        {
            get { return allfolders; }
            set { allfolders = value; RaisePropertyChanged(() => this.AllFoldersList); }
        }

        public static ObservableCollection<VideoFolder> HomePageCollection
        {
            get { return allfolders; }
        }

        public IList<VideoFolder> Data
        {
            get {
                if (FileLoader.FileLoaderInstance.DataSource == null)
                    FileLoader.FileLoaderInstance.DataSource = FileLoader.FileLoaderInstance.GetAllFiles(AllFoldersList);

                return new ObservableCollection<VideoFolder>(FileLoader.FileLoaderInstance.DataSource.Values);
            }
        }

        public static bool HasSearchData()
        {
            if(ApplicationService.AppSettings.MovieFolders.Count == 0)
            {
                return true;
            }
            return isLoadingData;
        }

        public bool IsLoadingData
        {
            get { return isLoadingData; }
        }

        public bool IsAllDataLoaded
        {
            get { return isLoadingData; }
            set
            {
                isLoadingData = value;
                RaisePropertyChanged(() => this.IsAllDataLoaded);
            }
        }

        public DelegateCommand AddFolderCommand { get; private set; }
        public DelegateCommand<object> OpenFolderCommand { get; private set; }

        public HomePageLocalViewModel(HomePageLocal owner)
        {
            pageOwner = owner;
            AddFolderCommand = new DelegateCommand(AddFolderCommandAction);
            OpenFolderCommand = new DelegateCommand<object>(OpenFolderCommandAction);
            FileLoaderDelegate += StartFileLoading;

            if (ApplicationService.AppSettings.MovieFolders.Count == 0)
                HasNoFolderAction();
            else
                this.LoadAllFolders();
        }

        internal void SetNavigationService(NavigationService navigationService)
        {
            this.navigationService = navigationService;
            (IShell.PageNavigatorHost.GetSearchControl as ISearchControl<VideoFolder>).DataSource = this;
        }
        
        private void OpenFolderCommandAction(object obj)
        {
            this.navigationService.Navigate(new FilePageView(this.navigationService), obj);
        }

        private void AddFolderCommandAction()
        {
            AddFolderDialog addFolderDialog = new AddFolderDialog();
            addFolderDialog.OnFinished += AddFolderDialog_OnFinished;
            addFolderDialog.ShowDialog();
        }

        private void AddFolderDialog_OnFinished(object sender, EventArgs e)
        {
            var addfiledialogVM = ((sender as AddFolderDialog).DataContext as AddFolderDialogViewModel);
            if (addfiledialogVM.FileHasChange)
            {
                LoadAllFolders(addfiledialogVM.RemovedFolders);
            }
            HasNoFolderAction();
        }

        private void HasNoFolderAction()
        {
            var moviefolderlist = ApplicationService.AppSettings.MovieFolders;
            if (moviefolderlist != null && moviefolderlist.Count == 0)
            {
                HomePageLocalNoFolder homePageLocalNoFolder = new HomePageLocalNoFolder(new NewFolderModel(), pageOwner.HomePageDock);
                homePageLocalNoFolder.OnFinished += HomePageLocalNoFolder_OnFinished;
                homePageLocalNoFolder.Show();
            }
        }

        private void HomePageLocalNoFolder_OnFinished(object sender, RoutedEventArgs e)
        {
            if (e == null) return;
            NewFolderModel newFolderModel = e.Source as NewFolderModel;
            if (newFolderModel.NewFolderCollection == null) return;
            LoadAllFolders();
        }

        public static VideoFolder GetExistingCopy(VideoFolder videoFolder)
        {
            if (allfolders == null) return null;
            return allfolders.Where(x => x.Equals(videoFolder)).FirstOrDefault();
        }


        public static VideoFolder GetExistingCopy<T>(VideoFolder videoFolder, IList<T> enumerable) where T : VideoFolder
        {
            return enumerable.Where(x => x.Equals(videoFolder)).FirstOrDefault();
        }

        private void StartFileLoading()
        {
            if (AllFoldersList != null && AllFoldersList.Count == 0) return;

            FileLoaderCompletion fileCompletionLoader = new FileLoaderCompletion();
            Threading.Executor executor = new Threading.ExecutorImpl();
            var task = executor.CreateTask(() =>
            {

                IsAllDataLoaded = true;
                fileCompletionLoader.FinishCollectionLoadProcess(AllFoldersList, true);
                FileLoader.FileLoaderInstance.InitGetAllFiles(AllFoldersList);

            }, "Loading your movie folders!.", () =>
            {
                IsAllDataLoaded = false;
                CommandManager.InvalidateRequerySuggested();
                RaisePropertyChanged(() => this.AllFoldersList);
            });

            executor.Execute(task);
        }

        public void LoadAllFolders(ObservableCollection<MovieFolderModel> removedFolder = null)
        {
            if(AllFoldersList != null && removedFolder != null)
            {
                RemoveFolders(ref removedFolder);
            }
            var moviefolders = ApplicationService.AppSettings.MovieFolders;
            var folderlist = new ObservableCollection<VideoFolder>();

            for (int i = 0; i < moviefolders.Count; i++)
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(moviefolders[i].FullName);
                if (directoryInfo.Exists)
                {
                    var vf = new VideoFolder(moviefolders[i].FullName);
                    folderlist.Add(vf);
                }
                else
                {
                    ApplicationService.AppSettings.MovieFolders.Remove(moviefolders[i]);
                    i--;
                }
            }
           
            MergeChanges(folderlist);
        }

        private void RemoveFolders(ref ObservableCollection<MovieFolderModel> removedFolder)
        {
            if (removedFolder != null && removedFolder.Count == 0) return;
            foreach (var item in removedFolder)
            {
                VideoFolder videoFolder = null;
                if (FileLoader.FileLoaderInstance.DataSource != null)
                    FileLoader.FileLoaderInstance.DataSource.TryGetValue(item.FullName, out videoFolder);
                else
                    videoFolder = AllFoldersList.Where(x => x.FilePath.Equals(item.FullName)).FirstOrDefault();
                AllFoldersList.Remove(videoFolder);
                if(videoFolder != null)
                    videoFolder.Dispose();
            }
        }

        private void MergeChanges(ObservableCollection<VideoFolder> folderlist)
        {
            if (AllFoldersList == null)
                AllFoldersList = new ObservableCollection<VideoFolder>();

            foreach (var item in folderlist)
            {
                VideoFolder videoFolder = item;
                var dir = new System.IO.DirectoryInfo(item.FullName);

                if (dir.Parent != null)
                {
                    var newparent = new VideoFolder(dir.Parent.FullName.ToString());
                    var ExistingParent = GetExistingCopy<VideoFolder>(newparent, folderlist);
                    if (ExistingParent != null)
                        videoFolder = new VideoFolder(ExistingParent, item.FullName);
                }
                if (AllFoldersList.Contains(videoFolder)) continue;
                AllFoldersList.Add(videoFolder);
                videoFolder.IsLoading = true;
            }
            if(AllFoldersList.Count == 0){
                HasNoFolderAction();
                return;
            }

            StartFileLoading();
        }

        private VideoFolder GetRootDirectory(VideoFolder newparent)
        {
            var dir = new System.IO.DirectoryInfo(newparent.FullName);
            if(dir.Parent != null)
            {
                var root = new VideoFolder(dir.Parent.FullName.ToString());
                var rootparent = GetRootDirectory(root);
                if (rootparent != null)
                {
                    root.SetParentDirectory(rootparent);
                }

                newparent.SetParentDirectory(root);
            }
            return newparent;
        }
        
        private IShell IShell
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShell>();
            }
        }
        
    }
}