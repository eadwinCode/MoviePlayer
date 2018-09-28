using System.Collections.ObjectModel;
using Movies.Models.Interfaces;

namespace Movies.InternetRadio.Interfaces
{
    internal interface IRadioPageService
    {
        ObservableCollection<IMoviesRadio> RadioModelCollection { get; }
        void Add(IMoviesRadio moviesRadio);
        void Remove(IMoviesRadio moviesRadio);
    }
}