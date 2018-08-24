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
    public delegate void OnFileNameChangedHandler(string oldname, VideoFolder videoItem);

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
    }

    public interface IFolder
    {
        int ChildrenSize { get; }
        string CreationDate { get; }
        int Day { get; }
        DirectoryInfo Directory { get; }
        bool Exists { get; }
        string FileExtension { get; }
        FileInfo FileInfo { get; }
        string FileName { get; }
        string FilePath { get; }
        string FileSize { get; set; }
        FileType FileType { get; set; }
        string FolderChildCount { get; }
        string FullName { get; }
        bool HasCompleteLoad { get; set; }
        bool HasFileChanges { get; }
        bool HasSubFolders { get; set; }
        bool HasThumbnail { get; }
        bool IsLoading { get; set; }
        ObservableCollection<PlayedFiles> LastSeenCollection { get; set; }
        string Name { get; }
        ObservableCollection<VideoFolder> OtherFiles { get; set; }
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

    public interface IVideoData
    {
        string Duration { get; set; }
        string FilePath { get; }
        string FileSize { get; set; }
        uint FrameHeight { get; set; }
        uint FrameWidth { get; set; }
        bool HasCompleteLoad { get; }
        bool HasLastSeen { get; }
        bool HasProgress { get; }
        bool HasSearchSubtitleFile { get; set; }
        bool HasThumbnail { get; }
        bool IsActive { get; set; }
        ILastSeen LastPlayedPoisition { get; set; }
        int? MaxiProgress { get; set; }
        string MediaTitle { get; set; }
        Visibility PlayedVisible { get; }
        ObservableCollection<PlaylistModel> PlayListItems { get; }
        double Progress { get; set; }
        string ProgressAsString { get; set; }
        string Resolution { get; set; }
        ObservableCollection<SubtitleFilesModel> SubPath { get; set; }
        Visibility SubVisible { get; }
        ImageSource Thumbnail { get; set; }
        string TooltipMessage { get; }

        event OnFileNameChangedHandler OnFileNameChangedChanged;

        void PlayCompletely();
        string ToString();
        void UpdateProperties();
    }
}
