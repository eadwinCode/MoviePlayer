using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Movies.MovieServices.Services
{
    public delegate void FileLoaderHandler();
    public class MovieDataStore : IDataSource<MediaFolder>
    {
        IApplicationService ApplicationService;
        IFileLoader FileLoader;
        readonly IBackgroundService BackgroundService;
        bool isLoadingData;
        private IDictionary<string, MediaFolder> data;
        private ObservableCollection<MediaFolder> allfolders;
        object _lock = new object();

        private IEventManager IEventManager
        {
            get
            {
                return EventAggregatorService.IEventManager;
            }
        }

        public IList<MediaFolder> Data
        {
            get
            {
                if (DataSource == null)
                    DataSource = FileLoader.GetAllFiles(AllFoldersList);

                return new ObservableCollection<MediaFolder>(DataSource.Values);
            }
        }

        public IDictionary<string, MediaFolder> DataSource
        {
            get
            {
                return data;
            }
            set { data = value; }
        }

        public bool HasDataSource
        {
            get {
                if (isLoadingData)
                    return false;
                return data != null && data.Count > 0; }
        }
        
        public ObservableCollection<MediaFolder> AllFoldersList
        {
            get { return allfolders; }
            set { allfolders = value; FolderItemChange(); }
        }
        
        public bool IsLoadingData
        {
            get { return isLoadingData; }
        }

        public MovieDataStore(IApplicationService applicationService, 
            IFileLoader fileLoader, IBackgroundService backgroundService)
        {
            this.ApplicationService = applicationService;
            this.FileLoader = fileLoader;
            this.BackgroundService = backgroundService;
            data = new Dictionary<string, MediaFolder>();
        }

        public void InitFileLoading()
        {
            if (ApplicationService.AppSettings.MovieFolders.Count == 0)
                HasNoFolderAction();
            else
                this.LoadAllFolders();
        }

        public MediaFolder GetExistingCopy(MediaFolder videoFolder)
        {
            if (allfolders == null) return null;
            return allfolders.Where(x => x.Equals(videoFolder)).FirstOrDefault();
        }
        
        public MediaFolder GetExistingCopy<T>(MediaFolder videoFolder, IList<T> enumerable) where T : MediaFolder
        {
            return enumerable.Where(x => x.Equals(videoFolder)).FirstOrDefault();
        }

        private void StartFileLoading()
        {
            if (AllFoldersList != null && AllFoldersList.Count == 0) return;
            isLoadingData = true;

            BackgroundService.Execute(() =>
            {
                LoaderCompletion.FinishCollectionLoadProcess(AllFoldersList, true);
            }, "", () => {
                foreach (var item in AllFoldersList)
                {
                    BackgroundService.Execute(() =>
                    {
                        var movies = FileLoader.GetAllFiles(item);
                        foreach (var movie in movies)
                        {
                            FileLoader.DispatcherService.InvokeDispatchAction(() => {
                                lock (_lock)
                                {
                                    if (!this.data.ContainsKey(movie.Key))
                                        this.data.Add(movie);
                                }
                            });
                        }
                    });
                }
                BackgroundService.OnTasksEnded += BackgroundService_OnTasksEnded;

            });

        }

        private void BackgroundService_OnTasksEnded(object sender, EventArgs e)
        {
            isLoadingData = false;
            CommandManager.InvalidateRequerySuggested();
            FolderItemChange();
            ResetAllFolderListIsloadingFlag();
            BackgroundService.OnTasksEnded -= BackgroundService_OnTasksEnded;
        }

        private void ResetAllFolderListIsloadingFlag()
        {
            foreach (MediaFolder videofolder in AllFoldersList)
            {
                if (videofolder.IsLoading)
                {
                    videofolder.IsLoading = false;
                }
            }
        }

        public void LoadAllFolders(ObservableCollection<MovieFolderModel> removedFolder = null)
        {
            if (AllFoldersList != null && removedFolder != null)
            {
                RemoveFolders(ref removedFolder);
            }
            var moviefolders = ApplicationService.AppSettings.MovieFolders;
            var folderlist = new ObservableCollection<MediaFolder>();

            for (int i = 0; i < moviefolders.Count; i++)
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(moviefolders[i].FullName);
                if (directoryInfo.Exists)
                {
                    var vf = new MediaFolder(moviefolders[i].FullName);
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
                MediaFolder videoFolder = null;
                if (DataSource != null)
                    DataSource.TryGetValue(item.FullName, out videoFolder);

                if(videoFolder == null)
                    videoFolder = AllFoldersList.Where(x => x.FilePath.Equals(item.FullName)).FirstOrDefault();
                RemoveVideoItem(videoFolder);
                if (videoFolder != null)
                    videoFolder.Dispose();
            }
        }

        private void MergeChanges(ObservableCollection<MediaFolder> folderlist)
        {
            if (AllFoldersList == null)
                InitializeAllFolderCollection();

            foreach (var item in folderlist)
            {
                MediaFolder videoFolder = item;
                var dir = new System.IO.DirectoryInfo(item.FullName);

                if (dir.Parent != null)
                {
                    var newparent = new MediaFolder(dir.Parent.FullName.ToString());
                    var ExistingParent = GetExistingCopy<MediaFolder>(newparent, folderlist);
                    if (ExistingParent != null)
                        videoFolder = new MediaFolder(ExistingParent, item.FullName);
                }
                if (!AddVideoItem(videoFolder)) continue;
            }
            if (AllFoldersList.Count == 0)
            {
                HasNoFolderAction();
                return;
            }

            StartFileLoading();
        }

        private void InitializeAllFolderCollection()
        {
            AllFoldersList = new ObservableCollection<MediaFolder>();
            AllFoldersList.CollectionChanged += AllFoldersList_CollectionChanged;
        }

        private void AllFoldersList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FolderItemChange();
        }

        private bool AddVideoItem(MediaFolder videoFolder)
        {
            if (AllFoldersList.Contains(videoFolder)) return false;
            AllFoldersList.Add(videoFolder);
            videoFolder.IsLoading = true;
            return true;
        }

        private bool RemoveVideoItem(MediaFolder videoFolder)
        {
            if (!AllFoldersList.Contains(videoFolder)) return false;
            AllFoldersList.Remove(videoFolder);
            return true;
        }

        private void HasNoFolderAction()
        {
            IEventManager.GetEvent<NoFolderNoticeEventToken>().Publish(true);
        }

        private void FolderItemChange()
        {
            IEventManager.GetEvent<FolderItemChangeEventToken>().Publish(true);
        }

        private MediaFolder GetRootDirectory(MediaFolder newparent)
        {
            var dir = new System.IO.DirectoryInfo(newparent.FullName);
            if (dir.Parent != null)
            {
                var root = new MediaFolder(dir.Parent.FullName.ToString());
                var rootparent = GetRootDirectory(root);
                if (rootparent != null)
                {
                    root.SetParentDirectory(rootparent);
                }

                newparent.SetParentDirectory(root);
            }
            return newparent;
        }

        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }
    }
}
