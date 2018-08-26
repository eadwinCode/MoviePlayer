using Movies.Models.Interfaces;
using Movies.Models.Model;
using System.Collections.Generic;

namespace Movies.MoviesInterfaces
{
    public interface IOpenFileCaller
    {
        void Open(IVideoData videoFolder);
        void Open(IVideoData videoFolder,IEnumerable<VideoFolderChild> movielist);
    }
}