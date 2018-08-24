using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Movies.Models.Interfaces;
using Movies.Models.Model;

namespace Movies.MoviesInterfaces
{
    public interface IPlaylistManagerViewModel
    {
        bool CanNext { get; }
        bool CanPrevious { get; }
        DelegateCommand ClearPlaylist { get; }
        PlaylistModel CurrentPlaylist { get; set; }
        DelegateCommand EnableSaveDialog { get; }
        bool HasChanges { get; set; }
        bool IsSaveDialogEnable { get; set; }
        int NowPlayingIndex { get; }
        ObservableCollection<VideoFolder> PlayListCollection { get; set; }
        string PlaylistName { get; }
        string TempPlaylistName { get; set; }

        void Add(VideoFolder vfc);
        VideoFolderChild GetNextItem();
        VideoFolderChild GetPreviousItem();
        PlaylistModel NewCreatePlaylist();
        void PlayFromAList(PlaylistModel plm);
        void PlaylistViewLoaded();
        void Remove(VideoFolder vfc);
        void SavePlaylistAction();
        void UpdateList();
        void UpdateNowPlaying(object obj, bool frompl);
        string WhatsNextItem();
        string WhatsPreviousItem();

        object GetPlaylistView(IMediaControllerViewModel mediaControllerViewModel);
        void PlayFromTemperalList(IVideoData playFile, IEnumerable<VideoFolderChild> TemperalList);
    }
}