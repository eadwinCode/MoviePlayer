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
using MahApps.Metro.Controls;

namespace Movies.MoviesInterfaces
{
    public interface IAddFolderDialog
    {
        event EventHandler OnFinished;
        void ShowDialog();
    }

    public interface IHomePlaylist
    {
        ObservableCollection<PlaylistModel> PlayListCollection { get; }
        object GetHomePlaylistView();
        void AddMoviePlaylistItem(PlaylistModel plm);
        void AddToPlayList(IPlaylistModel ipl, bool addtomovieplaylist = true);
        void CreateNewPlayList(PlaylistModel playlistModel);
        void CreateNewPlayList(string ItemPath);
        void InitializeComponent();
        void RemoveMoviePlaylistItem(PlaylistModel plm);
    }

    public interface IStatusMessageManager : IEnumerable<IStatusMessage>
    {
        IStatusMessage CreateMessage(string message);
        IStatusMessage DefaultStatusMessage { get; }
        IStatusMessage CreateMessage(string message,long destroyTimeInMilliseconds);
        event EventHandler OnMessageCollectionChanged;
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
        void InvokeDispatchAction(Dispatcher customDispatcher, Action action);
        void BeginInvokeDispatchAction(Action action);
        void BeginInvokeDispatchAction(Dispatcher customDispatcher, Action action);
        void ExecuteTimerAction(Action callback, long millisecond);
    }

    public interface INavigatorService
    {
        NavigationService NavigationService { get; }
        Frame Host { get; }
        void NavigatePage(object page,object pageData = null);
        void NavigateMainPage(IMainPage mainPage, object pageData = null);
    }

    public interface IMainPage
    {
        IMenuFlyout FlyoutMenu { get; set; }
        int GetHashCode();
        NavigationService NavigationService { get; }
        ContentControl Docker { get; }
    }

    public interface IWindowsCommandButton
    {
        void SetActive(bool setactive, bool loadPage);
    }

    public interface IPageNavigatorHost : IMultiViewHost
    {
        INavigatorService PageNavigator { get; }
    }

    public interface IMultiViewHost
    {
        object GetView(string ViewName);
        void AddView(object view, string uniqueName);
        void RemoveView(string ViewName);
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
    

    public interface ITreeViewer
    {
        UserControl MoviesFolder { get; }
        UserControl MoviesPLaylist { get; }
    }
    
    public interface IShellWindow
    {
        CommandBindingCollection CommandBindings { get; }
    }

    public interface IShellWindowService : IMultiViewHost
    {
        MetroWindow ShellWindow { get; }
        void OnWindowsLoaded();
        void RegisterMenu(object flyoutSubMenuItem);
        void RegisterMenuAt(object flyoutSubMenuItem,int Position);
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
        bool SavePlaylistFiles();
        bool SaveMovieFolders();
    }

   
}
