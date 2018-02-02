using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoComponent.BaseClass
{
    public class SortByDate : IComparer<VideoFolder>
    {
        public int Compare(VideoFolder x, VideoFolder y)
        {
            if (x.FileType == y.FileType)
            {
                var vv = x.CreationDate.CompareTo(y.CreationDate);
                return vv;
            }
            else if (x.FileType == FileType.Folder)
            {
                return -1;
            }
            return 1;
        }
    }
}
