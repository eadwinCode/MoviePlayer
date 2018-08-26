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
    public class MovieDataStore : IDataSource<VideoFolder>
    {
        IApplicationService ApplicationService;
        IFileLoader FileLoader;
        bool isLoadingData;
        private IDictionary<string, VideoFolder> data;
        object _lock = new object();

        private IEventManager IEventManager
        {
            get
            {
                return EventAggregatorService.IEventManager;
            }
        }

        public IList<VideoFolder> Data
        {
            get
            {
                if (DataSource == null)
                    DataSource = FileLoader.GetAllFiles(AllFoldersList);

                return new ObservableCollection<VideoFolder>(DataSource.Values);
            }
        }

        public IDictionary<string, VideoFolder> DataSource
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
        

        private ObservableCollection<VideoFolder> allfolders;
        public ObservableCollection<VideoFolder> AllFoldersList
        {
            get { return allfolders; }
            set { allfolders = value; FolderItemChange(); }
        }
        
        public bool IsLoadingData
        {
            get { return isLoadingData; }
        }

        public MovieDataStore(IApplicationService applicationService, IFileLoader fileLoader)
        {
            this.ApplicationService = applicationService;
            this.FileLoader = fileLoader;
        }

        public void InitFileLoading()
        {
            if (ApplicationService.AppSettings.MovieFolders.Count == 0)
                HasNoFolderAction();
            else
                this.LoadAllFolders();
        }

        public VideoFolder GetExistingCopy(VideoFolder videoFolder)
        {
            if (allfolders == null) return null;
            return allfolders.Where(x => x.Equals(videoFolder)).FirstOrDefault();
        }


        public VideoFolder GetExistingCopy<T>(VideoFolder videoFolder, IList<T> enumerable) where T : VideoFolder
        {
            return enumerable.Where(x => x.Equals(videoFolder)).FirstOrDefault();
        }

        private void StartFileLoading()
        {
            if (AllFoldersList != null && AllFoldersList.Count == 0) return;
            var task = Task.Factory.StartNew(() =>
               {
                   isLoadingData = true;
                   LoaderCompletion.FinishCollectionLoadProcess(AllFoldersList, true);
                   FileLoader.InitGetAllFiles(AllFoldersList);

               }).ContinueWith(t =>
               {
                   isLoadingData = false;
                   CommandManager.InvalidateRequerySuggested();
                   FolderItemChange();
               }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        public void LoadAllFolders(ObservableCollection<MovieFolderModel> removedFolder = null)
        {
            if (AllFoldersList != null && removedFolder != null)
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
                if (DataSource != null)
                    DataSource.TryGetValue(item.FullName, out videoFolder);
                else
                    videoFolder = AllFoldersList.Where(x => x.FilePath.Equals(item.FullName)).FirstOrDefault();
                RemoveVideoItem(videoFolder);
                if (videoFolder != null)
                    videoFolder.Dispose();
            }
        }

        private void MergeChanges(ObservableCollection<VideoFolder> folderlist)
        {
            if (AllFoldersList == null)
                InitializeAllFolderCollection();

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
            AllFoldersList = new ObservableCollection<VideoFolder>();
            AllFoldersList.CollectionChanged += AllFoldersList_CollectionChanged;
        }

        private void AllFoldersList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FolderItemChange();

        }

        private bool AddVideoItem(VideoFolder videoFolder)
        {
            if (AllFoldersList.Contains(videoFolder)) return false;
            AllFoldersList.Add(videoFolder);
            videoFolder.IsLoading = true;
            return true;
        }

        private bool RemoveVideoItem(VideoFolder videoFolder)
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

        private VideoFolder GetRootDirectory(VideoFolder newparent)
        {
            var dir = new System.IO.DirectoryInfo(newparent.FullName);
            if (dir.Parent != null)
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

        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }
    }
}
