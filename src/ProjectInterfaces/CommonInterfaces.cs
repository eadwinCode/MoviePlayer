using Delimon.Win32.IO;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Movies.MoviesInterfaces
{
    public interface IBackgroundService
    {
        void Shutdown();
        void Shutdown(ITask task);
        void Execute();
        void Execute(ITask task);
        ITask Execute(Action action, string message = null, Action callback = null);
        //Task CreateTask<T> (Action<T> action, string message, Action callback);
    }
    public interface ITask
    {
        bool IsBusy { get; }
        bool IsCancelled { get; }
        IStatusMessage ProcessStatusNotice { get;}
        string GetMessage();
        string GetSubscription();
        void Run();
        void Stop();
    }
        public interface ISettings
    {
        DirectoryInfo LastDirectory { get; set; }
        ViewType ViewType { get; set; }
    }
    
    public interface IPlayFile
    {
        bool IsPlayingMedia { get; }
        void ShutDownMediaPlayer();
        void PlayFileInit(IVideoData obj);
        void AddFiletoPlayList(IVideoData obj);
        void WMPPlayFileInit(IVideoData vfc);
        void PlayFileInit(IFolder obj);
        void AddFiletoPlayList(IFolder obj);
        void WMPPlayFileInit(IFolder vfc);
        void PlayFileFromPlayList(PlaylistModel playlistModel);
        void PlayFileInit(IPlayable playFile, IEnumerable<IPlayable> TemperalList);
        void PrepareVideoElement();
    }

    public interface ICollectionViewModel
    {
        string CurrentDir { get; set; }
        DirectoryInfo DirectoryPosition { get; set; }
        Object GetCollectionVM { get; }
        IFileExplorer IFileExplorer { get; }
        bool IsLoading { get; set; }
        double LoadingProgress { get; set; }
        DataTemplateSelector MyTemplateChange { get; set; }
        ViewType ActiveViewType { get; set; }

        void TreeViewUpdate(string obj);
    }

    public interface IFileLoader
    {
        bool HasDataSource { get; }
        IDictionary<string, VideoFolder> GetAllFiles(ObservableCollection<VideoFolder> itemsSource);
        VideoFolder GetExistingVideoFolderIfAny(VideoFolder videoFolder);
        VideoFolder GetFolderItems(VideoFolder item);
        void GetRootDetails(SortType sorttype, ref VideoFolder ParentDir);
        void InitGetAllFiles(ObservableCollection<VideoFolder> itemsSource);
        VideoFolderChild LoadChildrenFiles(DirectoryInfo directoryInfo, bool newpath = false);
        ObservableCollection<VideoFolder> LoadChildrenFiles(VideoFolder Parentdir, bool newpath = false);
        ObservableCollection<VideoFolder> LoadChildrenFiles(VideoFolder Parentdir, IList<FileInfo> files, bool newpath = false);
        VideoFolder LoadParentFiles(VideoFolder Parentdir, IList<DirectoryInfo> SubDirectory, IList<FileInfo> SubFiles, SortType sorttype);
        VideoFolder LoadParentFiles(VideoFolder Parentdir, IList<DirectoryInfo> SubDirectory, SortType sorttype);
        VideoFolder LoadParentFiles(VideoFolder Parentdir, IList<FileInfo> SubFiles, SortType sorttype);
        VideoFolder LoadParentFiles(VideoFolder ParentDir, SortType sorttype);
        void RemoveFromDataSource(VideoFolder existingVideoFolder);
        VideoFolder SortList(SortType sorttype, VideoFolder parent);
        VideoFolderChild CreateVideoFolderChild(IFolder Parentdir, FileInfo fileInfo);
        void SetLastSeen(VideoFolderChild videoFolderChild);
    }

    public interface IFileLoaderCompletion
    {
        void FinishCollectionLoadProcess(IList<VideoFolder> itemsSource, Dispatcher dispatcherUnit);
        void FinishCollectionLoadProcess(ObservableCollection<VideoFolder> itemsSource, bool IsMovieFolder);
    }


    public interface IFileExplorerCommonHelper
    {
        bool CheckForWantedDir(DirectoryInfo dir, IDictionary<string, string> formats);
        string FileSizeConverter(double filelength);
        T GetElement<T>(DependencyObject element);
        List<FileInfo> GetFilesByExtensions(DirectoryInfo dir, IDictionary<string, string> extensions);
        List<DirectoryInfo> GetParentSubDirectory(DirectoryInfo DirectoryPosition, IDictionary<string, string> formats);
        List<DirectoryInfo> GetParentSubDirectoryWithoutCheck(DirectoryInfo DirectoryPosition, IDictionary<string, string> formats);
        IEnumerable<FileInfo> GetSubtitleFiles(DirectoryInfo dir);
        bool Match(string srtfile, string file);
        ObservableCollection<string> MatchSubToMedia(string p, IEnumerable<FileInfo> subpath);
        List<FileInfo> RemoveDirectory(List<FileInfo> files, FileInfo fileInfo);
    }

    public interface ISavedPlaylistCollection
    {
        ObservableCollection<PlaylistModel> MoviePlayList { get; set; }
    }

    public interface ISavedLastSeenCollection
    {
        IDictionary<string, PlayedFiles> LastSeenCollection { get; }

        void Add(PlayedFiles playedFiles);
        PlayedFiles GetLastSeen(string fileName);
        bool HasItem(PlayedFiles playedFiles);
        void Remove(PlayedFiles playedFiles);
    }
}
