using Common.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.FileHelper
{
    public class Settings
    {
        public ObservableCollection<string> TreeViewItems 
            = new ObservableCollection<string>();
        private ViewType viewType = ViewType.Small;
        public ViewType ViewType { get { return viewType; } set { viewType = value; } }
        public DirectoryInfo LastDirectory { get; set; }


    }
}
