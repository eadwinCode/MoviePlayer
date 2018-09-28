using Movies.Models.Interfaces;
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
            MoviePlayList = new ObservableCollection<PlaylistModel>();
        }
    }

    public class SavedRadioCollection
    {
        public IDictionary<Guid,IMoviesRadio> RadioCollection { get; set; }
        public Dictionary<string,Guid> RadioHomePageData { get; set; }

        public SavedRadioCollection()
        {
            RadioHomePageData = new Dictionary<string, Guid>()
            {
                { "Home-Radio Station", Guid.Empty },
                { "RadioStation-Country", Guid.Empty },
                { "RadioStation-Styles", Guid.Empty },
                { "RadioStation-Favorites", Guid.Empty },
            };
            RadioCollection = new Dictionary<Guid, IMoviesRadio>();
        }
    }
}
