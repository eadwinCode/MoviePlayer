using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualizingListView
{
    public interface ILoadFile
    {
        string MediaPath { get;}
        void CreateMediaFolders();
        bool SerializeData();
       bool DeserializeData();
    }
}
