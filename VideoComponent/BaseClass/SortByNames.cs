using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoComponent.BaseClass
{
    public class SortByNames : IComparer<VideoFolder>
    {
        public int Compare(VideoFolder x, VideoFolder y)
        {
            if (x.FileType == y.FileType)
            {
                return x.FileName.CompareTo(y.FileName);
            }
            else if (x.FileType == FileType.Folder)
            {
                return -1;
            }
            return 1;
        }
    }
}
