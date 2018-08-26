using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Models.Model
{
    public class SavedPlaylistCollection
    {
        public ObservableCollection<PlaylistModel> MoviePlayList { get; set; }
        public SavedPlaylistCollection()
        {
            MoviePlayList =
            new ObservableCollection<PlaylistModel>();
        }
    }
}
