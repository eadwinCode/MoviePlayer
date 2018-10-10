using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Movies.InternetRadio.StreamManager
{
    public interface IRadioDataService: IEnumerable<KeyValuePair<Guid, IMoviesRadio>>
    {
        void AddRadio(IRadioGroup radioGroup,IMoviesRadio radio);
        IMoviesRadio GetRadioObjectFromKey(Guid key);
        ObservableCollection<IMoviesRadio> GetRadioObjectsFromKeys(IEnumerable<Guid> keys);
        bool RemoveRadio(IRadioGroup radioGroup,Guid key);
    }
}