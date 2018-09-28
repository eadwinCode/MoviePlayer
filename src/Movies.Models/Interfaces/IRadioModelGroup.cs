using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Movies.Models.Interfaces
{
    public interface IRadioGroup : IMoviesRadio
    {
        ObservableCollection<Guid> RadioStations { get; }
        void AddStation(Guid radioCollection);
        void RemoveStation(Guid radioCollection);
        IMoviesRadio GetNewRadioModel();
    }

    public interface IRadioStations
    {
        ObservableCollection<Guid> RadioStations { set; }
    }
}
