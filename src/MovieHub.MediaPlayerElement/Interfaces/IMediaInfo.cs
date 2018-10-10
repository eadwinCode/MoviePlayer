using Meta.Vlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieHub.MediaPlayerElement.Interfaces
{
    public interface IMediaInfo
    {
        string Title { get; }
        string Artist { get; }
        string Genre { get; }
        string Copyright { get; }
        string Album { get; }
        string TrackNumber { get; }
        string Description { get; }
        string Rating { get; }
        string Date { get; }
        string Setting { get; }
        string Url { get; }
        string Language { get; }
        string NowPlaying { get; }
        string Publisher { get; }
        string EncodedBy { get; }
        string ArtworkUrl { get; }
        string TrackID { get; }
        string TrackTotal { get; }
        string Director { get; }
        string Season { get; }
        string Episode { get; }
        string ShowName { get; }
        string Actors { get; }
        string AlbumArtist { get; }
        string DiscNumber { get; }
        MediaTrackList MediaTrackList
        {
            get;
        }
    }
}
