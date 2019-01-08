using MovieHub.MediaPlayerElement;
using Movies.Models.Interfaces;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoPlayerView.View;

namespace VideoPlayerView.interfaces
{
    interface IPlayFileExtended:IPlayFile
    {
        DefaultPlayerControl DefaultPlayerControl { get; }
    }

}
