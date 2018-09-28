using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Movies.Models.Interfaces;
using Movies.Models.Model;

namespace Movies.MoviesInterfaces
{
    public interface IPlaylistManager
    {
        bool CanNext { get; }
        bool CanPrevious { get; }
        IPlaylistModel CurrentPlaylist { get;  }
        ObservableCollection<IPlayable> PlayListCollection { get; }
        string PlayListName { get; }
        
        IPlayable GetNextItem();
        IPlayable GetPreviousItem();
        IPlaylistModel NewCreatePlaylist(string playlistName);
        
        void Remove(IPlayable vfc);
        void SavePlaylistAction(string playlistName = null);

        string WhatsNextItem();
        string WhatsPreviousItem();
        
    }
}