using Delimon.Win32.IO;
using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoComponent.BaseClass;

namespace VirtualizingListView.Model
{
    public class NavigationModel
    {
        public DirectoryInfo Dir { get; set; }
        public VideoFolder VideoData { get; set; }
    }
}
