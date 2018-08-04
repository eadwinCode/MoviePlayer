using Common.Interfaces;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoComponent.BaseClass;

namespace VirtualizingListView.Util
{
    public static class FileOpenCall
    {
        public static void Open(VideoFolder videoFolder)
        {
            Iplayfile.PlayFileInit(videoFolder);
        }
        private static IPlayFile Iplayfile
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }
    }
}
