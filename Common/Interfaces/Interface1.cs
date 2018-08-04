using Common.Model;
using Common.Util;
using Delimon.Win32.IO;
using Meta.Vlc.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Common.Interfaces
{
    public interface IMainPages
    {
        void SetController(IWindowsCommandButton controller);
        bool HasController { get; }
    }

    public interface IWindowsCommandButton
    {
        void SetActive(bool setactive,bool loadPage);
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
        IList<T> Data { get; }
        bool IsLoadingData { get; }
    }

    public interface INavigatorService
    {
        NavigationService NavigationService { get; }
        Frame Host { get; }
        ContentControl DockControl { get; }

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

    public interface ITreeViewer {
        UserControl MoviesFolder { get; }
        UserControl MoviesPLaylist { get; }
    }


    public interface IShell
    {
        IPageNavigatorHost PageNavigatorHost { get; }
    }

    public interface IPageNavigatorHost
    {
        INavigatorService PageNavigator { get; }
        ISearchControl GetSearchControl { get; set; }
        ContentControl DockControl { get; }
    }
    public interface IHasChanges
    {
        bool HasChanges { get; set; }
    }
    public interface IPlaylistModel
    {
        string PlaylistName { get; set; }
        ObservableCollection<string> ItemsPaths { get; }
        void SetIsActive(bool value);
    }
    public interface ILastSeen
    {
        string FileName { get; }
        double ProgressLastSeen
        {
            get; set;
        }

        string Percentage { get; set; }

        bool Exist
        {
            get;
        }
        void RemoveLastSeen();
    }

    public interface IFolder
    {
        FileType FileType { get; set; }
        int ChildrenSize { get; }
        string FilePath { get; }
        SortType SortedBy { get; set; }
        FileInfo FileInfo { get; }
        string FolderChildCount { get; }
        DirectoryInfo Directory { get; }
        IFolder ParentDirectory { get; }
        bool HasFileChanges { get; }
        bool HasCompleteLoad { get; }
        string FileExtension { get; }
        string FileName { get; }
        string CreationDate { get; }
        event EventHandler ParentDirectoryChanged;
    }

    public interface IFileExplorer
    {
        ListView FileExplorerListView { get; }
        Object ContextMenuObject { get; }
    }
    public interface IPlayFile
    {
        IVideoElement VideoElement { get; }
        void PlayFileInit(object obj);
        void AddFiletoPlayList(object obj);
        void WMPPlayFileInit(object vfc);
        void PlayFileFromPlayList(PlaylistModel playlistModel);
    }

    public interface IVideoElement
    {
        CommandBindingCollection CommandBindings { get; }
        IPlayListClose PlayListView { get; }
        IMediaController IVideoPlayer { get; }
        //UIElement WindowsTab { get; }
        //UIElement WindowsTabDock { get; }
        VlcPlayer MediaPlayer { get; }
        UIElement ParentGrid { get; }
        string Title { get; set; }
        void SetTopMost();
    }

    public interface IPlayListClose
    {
        event EventHandler OnPlaylistClose;
    }

    public interface ISubtitleFiles
    {
        string Directory { get; set; }
        string FileName { get; set; }
        bool IsSelected { get; set; }
    }

    public interface ISubtitle
    {
        void LoadSub(ISubtitleFiles subtitlefiles);
        void AdjustFontSize(double fontsize, double thickness);
        bool HasSub { get; }
        void SetText(double position);
        void Clear();
        bool IsDisabled { get; set; }
    }

    public interface IMediaController
    {
       
       // MediaUriElement MediaPlayer { get; }
        //Canvas CanvasEnvironment { get; }
        //ISubtitle Subtitle { get; }
        UserControl MediaController { get; }
        event EventHandler ScreenSettingsChanged;
    }
}
