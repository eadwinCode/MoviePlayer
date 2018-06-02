using Common.Model;
using Common.Util;
using Meta.Vlc.Wpf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Common.Interfaces
{

    public interface ICollectionViewModel
    {
        string CurrentDir { get; set; }
        DirectoryInfo DirectoryPosition { get; set; }
        Object GetCollectionVM { get; }
        IFileExplorer IFileExplorer { get; }
        bool IsLoading { get; set; }
        double LoadingProgress { get; set; }
        DataTemplateSelector MyTemplateChange { get; set; }
        ViewType ViewType { get; set; }

        void TreeViewUpdate(string obj);
    }

    public interface ITreeViewer {
        UserControl MoviesFolder { get; }
        UserControl MoviesPLaylist { get; }
    }


    public interface IShell
    {
        IFileViewer FileView { get; }
    }

    public interface IFileViewer
    {
        UIElement FileExplorer { get; }
        UIElement TreeViewer { get; }
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

    }

    public interface IFolder
    {
        FileType FileType { get; set; }
        int ChildrenSize { get; }
        string FilePath { get; }
        SortType SortedBy { get; set; }
        FileInfo FileInfo { get; }
        ObservableCollection<PlayedFiles> LastSeenCollection { get; set; }
        string FolderChildCount { get; }
        DirectoryInfo Directory { get; }
        IFolder ParentDirectory { get; }
        bool HasFileChanges { get; }
        bool HasCompleteLoad { get; }
        string FileExtension { get; }
        string FileName { get; }
        string CreationDate { get; }
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
        ISubtitleMediaController IVideoPlayer { get; }
        UIElement WindowsTab { get; }
        UIElement WindowsTabDock { get; }
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

    public interface ISubtitleMediaController
    {
       
       // MediaUriElement MediaPlayer { get; }
        //Canvas CanvasEnvironment { get; }
        //ISubtitle Subtitle { get; }
        UserControl MediaController { get; }
        event EventHandler ScreenSettingsChanged;
    }
}
