using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Models.Interfaces
{
    public interface IApplicationModelService
    {
        SavedPlaylistCollection AppPlaylist { get; }
        SavedLastSeenCollection SavedLastSeenCollection { get; }
        SavedRadioCollection SavedRadioCollection { get; }
        Settings AppSettings { get; }

        bool SaveLastSeenFile();
    }
}
