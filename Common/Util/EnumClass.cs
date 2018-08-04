using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Util
{
    public enum FileType
    {
        Folder,
        File
    };

    public enum VolumeState
    {
        Muted,
        Active
    };

    public enum SortType
    {
        Date,
        Name,
        Extension
    };

    public enum ViewType
    {
        Small,
        Large
    };

    public enum RepeatMode
    {
        Repeat,
        RepeatOnce,
        NoRepeat
    };
}
