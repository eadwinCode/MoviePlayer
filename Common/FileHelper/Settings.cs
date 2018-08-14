using Common.Model;
using Common.Util;
using Delimon.Win32.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.FileHelper
{
    public class Settings
    {
        public ObservableCollection<MovieFolderModel> MovieFolders 
            = new ObservableCollection<MovieFolderModel>();
        private ViewType viewType = ViewType.Small;
        public ViewType ViewType { get { return viewType; } set { viewType = value; } }

        public DirectoryInfo LastDirectory { get; set; }
    }
}
