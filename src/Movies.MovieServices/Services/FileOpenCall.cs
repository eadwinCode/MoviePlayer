using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoComponent.BaseClass;

namespace Movies.MovieServices.Services
{
    public class OpenFileCaller : IOpenFileCaller
    {
        private IPlayFile iplayFilecaller;

        public OpenFileCaller(IPlayFile playFilecaller)
        {
            this.iplayFilecaller = playFilecaller;
        }

        public void Open(IVideoData videoFolder)
        {
            iplayFilecaller.PlayFileInit(videoFolder);
        }

        public void Open(IVideoData videoFolder, IEnumerable<VideoFolderChild> movielist)
        {
            iplayFilecaller.PlayFileInit(videoFolder,movielist);
        }
    }
}
