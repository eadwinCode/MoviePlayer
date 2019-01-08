using Delimon.Win32.IO;
using Movies.Enums;
using Movies.Models.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Movies.Models.Interfaces
{
    public delegate void OnFileNameChangedHandler(string oldname, MediaFolder videoItem);


    public interface IHomeControl
    {
        IMovieControl MovieControl { get; }
    }

    public interface IMovieControl
    {
        void CloseLastSeenBoard();
        void NotifyLastSeen(IMediaPlayabeLastSeen playablelastseen);
        void OnApplyTemplate();
        void SetMediaVolume(double vol);
        void SetMovieTitleBoard(string info);
    }

    public interface ISubtitleFiles
    {
        string Directory { get; set; }
        string FileName { get; set; }
        int Id { get; }
        bool IsSelected { get; set; }
        string Name { get; }
        SubtitleType SubtitleType { get; }

        string ToString();
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

    public interface IPlaylistModel
    {
        string PlaylistName { get; set; }
        void SetIsActive(bool value);
        ICollection<Pathlist> GetEnumerator { get; }
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
        void Add();
        void Save();
    }

    public interface IFolder : IItemSort
    {
        int ChildrenSize { get; }
        int Day { get; }
        DirectoryInfo Directory { get; }
        bool Exists { get; }
        string FilePath { get; }
        string FileSize { get; set; }
        string FolderChildCount { get; }
        string FullName { get; }
        bool HasCompleteLoad { get; set; }
        bool HasFileChanges { get; }
        bool HasSubFolders { get; set; }
        bool HasThumbnail { get; }
        bool IsLoading { get; set; }
        FileInfo FileInfo { get; }
        ObservableCollection<PlayedFiles> LastSeenCollection { get; set; }
        string Name { get; }
        ObservableCollection<MediaFolder> OtherFiles { get; set; }
        IFolder ParentDirectory { get; }
        SortType SortedBy { get; set; }

        event OnFileNameChangedHandler OnFileNameChangedChanged;
        event EventHandler ParentDirectoryChanged;
        event PropertyChangedEventHandler PropertyChanged;

        void Dispose();
        bool Equals(object obj);
        void RefreshFileInfo();
        void RenameFile(string newFilePath);
        void SetParentDirectory(IFolder folder);
        string ToString();
    }

    public interface IVideoData : IMediaPlayabeLastSeen, ILocaFilePlayable , IItemSort
    {
        string Duration { get; set; }
        string FileSize { get; set; }
        uint FrameHeight { get; set; }
        uint FrameWidth { get; set; }
        bool HasCompleteLoad { get; }
        bool HasLastSeen { get; }
        bool HasProgress { get; }
        bool HasSearchSubtitleFile { get; set; }
        bool HasThumbnail { get; }
        bool IsActive { get; set; }
        
        int? MaxiProgress { get; set; }
        string MediaTitle { get; set; }
        Visibility PlayedVisible { get; }
        ObservableCollection<PlaylistModel> PlayListItems { get; }
        string ProgressAsString { get; set; }
        string Resolution { get; set; }
        ObservableCollection<string> SubPath { get; set; }
        Visibility SubVisible { get; }
        ImageSource Thumbnail { get; set; }
        string TooltipMessage { get; }

        event OnFileNameChangedHandler OnFileNameChangedChanged;
        string ToString();
        void RefreshFileInfo();
    }

    public interface IMediaPlayabeLastSeen
    {
        double Progress { get; set; }
        ILastSeen LastPlayedPoisition { get; set; }
        void SetProgress();
        void PlayCompletely();
    }

    public interface ILocaFilePlayable
    {
        string FilePath { get; }
    }

    public interface IItemSort
    {
        DateTime CreationDate { get; }
        string FileExtension { get; }
        string FileName { get; }
        GroupCatergory FileType { get; }
    }
}
