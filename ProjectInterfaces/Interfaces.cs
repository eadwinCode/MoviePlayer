using Meta.Vlc.Wpf;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace Movies.MoviesInterfaces
{
    public interface IAddFolderDialog
    {
        event EventHandler OnFinished;
        void ShowDialog();
    }

    public interface IStatusMessageManager
    {
        IStatusMessage CreateMessage(string message);
        IStatusMessage DefaultStatusMessage { get; }
        IStatusMessage CreateMessage(string message,long destroyTimeInMilliseconds);
        event EventHandler OnMessageCollectionChanged;
        IDictionary<Guid,IStatusMessage> MessageCollection { get; }
        void DestroyMessage(IStatusMessage statusMessage);
        void DestroyMessage(IStatusMessage statusMessage,DestroyTime UsePredefinedTime);
        void AutomateMessageDestroy(IStatusMessage statusMessage, long miilisecond);
    }

    public interface IStatusMessage
    {
        Guid Id { get; }
        string Message { get; set; }
        IStatusMessageManager StatusMessageManager { get; }
        void AutomateMessageDestroy(long millisecond);
        void AutomateMessageDestroy(DestroyTime UsePredefinedTime);
    }
    public interface IDispatcherService
    {
        void InvokeDispatchAction(Action action);
        void BeginInvokeDispatchAction(Action action);
        void ExecuteTimerAction(Action callback, long millisecond);
    }
    
    public interface ISearchControl
    {
        string CustomPropertyName { get; set; }
        bool HasText { get; }
        bool IsSearchButtonEnabled { get; set; }
        event TextChangedEventHandler TextChangeEvent;
        string Text { get; set; }
        void Clear();
        void QuickSearch(string query);
        void UseExtendedSearchTextbox(object searchTextBox);
    }

    public interface IDataSource<T>
    {
        IDictionary<string, VideoFolder> DataSource { get; set; }
        bool HasDataSource { get; }
        ObservableCollection<T> AllFoldersList { get; set; }
        IList<T> Data { get; }
        bool IsLoadingData { get; }

        VideoFolder GetExistingCopy(VideoFolder videoFolder);
        VideoFolder GetExistingCopy<T>(VideoFolder videoFolder, IList<T> enumerable) where T : VideoFolder;
        void LoadAllFolders(ObservableCollection<MovieFolderModel> removedFolder = null);
        void InitFileLoading();
    }

    public interface INavigatorService
    {
        NavigationService NavigationService { get; }
        Frame Host { get; }
        ContentControl DockControl { get; }
    }
   

    public interface ITreeViewer
    {
        UserControl MoviesFolder { get; }
        UserControl MoviesPLaylist { get; }
    }
    
    public interface IShell
    {

    }

    public interface IPageNavigatorHost
    {
        INavigatorService PageNavigator { get; }
        ContentControl DockControl { get; }
    }
    public interface IHasChanges
    {
        bool HasChanges { get; set; }
    }

    

    public interface IFileExplorer
    {
        ListView FileExplorerListView { get; }
        Object ContextMenuObject { get; }
    }
    

    
    public interface IPlaylistViewMediaPlayerView
    {
        event EventHandler OnPlaylistClose;
        void OnPlaylistCloseExecute(object sender);
    }
    
    public interface IApplicationService : IApplicationModelService
    {
        IDictionary<string, string> Formats { get; }
        string[] SubtitleFormats { get; }

        void CreateFolder();
        string FileExistOrCreate(string path_to_file, bool returnpath = false);
        void LoadFiles();
        void LoadLastSeenFile();
        bool LoadPlaylistFile();
        bool LoadTreeViewFile();
        void SaveFiles();
        bool SaveLastSeenFile();
        bool SavePlaylistFiles();
        bool SaveMovieFolders();
    }

   
}
