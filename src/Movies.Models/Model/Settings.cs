
using Delimon.Win32.IO;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Models.Model
{
    public class Settings
    {
        public ObservableCollection<MovieFolderModel> MovieFolders { get; set; }
        private ViewType viewType = ViewType.Small;
        private SortType sorttype = SortType.Date;

        public Settings()
        {
            MovieFolders = new ObservableCollection<MovieFolderModel>();
        }

        public ViewType ViewType { get { return viewType; } set { viewType = value; } }

        public SortType SortType { get { return sorttype; } set { sorttype = value; } }

        public DirectoryInfo LastDirectory { get; set; }
    }
}
