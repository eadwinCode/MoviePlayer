using Common.Model;
using Common.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.FileHelper
{
    public class SavedPlaylistCollection
    {
        public ObservableCollection<PlaylistModel> MoviePlayList =
            new ObservableCollection<PlaylistModel>();

       
    }
}
