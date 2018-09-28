using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Movies.InternetRadio.StreamManager
{
    public class RadioDataService : IRadioDataService
    {
        IDictionary<Guid, IMoviesRadio> _radioData;
        IApplicationService ApplicationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationService>();
            }
        }
        
        public RadioDataService()
        {
            _radioData = ApplicationService.SavedRadioCollection.RadioCollection;
        }
        
        public void AddRadio(IRadioGroup radioGroup,IMoviesRadio radio)
        {
            if (radio is IRadioModel)
            {
                var existingItem = _radioData.FirstOrDefault(x =>
                     {
                         if (x.Value is IRadioModel)
                         {
                             return (x.Value as IRadioModel).Url.Equals((radio as IRadioModel).Url);
                         }
                         return false;
                     }
                 );
                if (existingItem.Value != null)
                {
                    radioGroup.AddStation(existingItem.Key);
                }
                else
                {
                    _radioData.Add(radio.Key, radio);
                    radioGroup.AddStation(radio.Key);
                }
                return;
            }

           _radioData.Add(radio.Key, radio);
           radioGroup.AddStation(radio.Key);
        }

        public bool RemoveRadio(IRadioGroup radioGroup,Guid key)
        {
            if (_radioData.ContainsKey(key))
            {
                _radioData.Remove(key);
                radioGroup.RemoveStation(key);
                return true;
            }
            return false;
        }

        public IMoviesRadio GetRadioObjectFromKey(Guid key)
        {
            IMoviesRadio moviesRadio = null;
            _radioData.TryGetValue(key, out moviesRadio);
            return moviesRadio;
        }

        public ObservableCollection<IMoviesRadio> GetRadioObjectsFromKeys(IEnumerable<Guid> keys)
        {
            ObservableCollection<IMoviesRadio> data = new ObservableCollection<IMoviesRadio>();
            foreach (var item in keys)
            {
                var radio = this.GetRadioObjectFromKey(item);
                if (radio != null)
                {
                    data.Add(radio);
                }
            }
            return data;
        }

        public IRadioGroup GetHomeGroup(string homePageKey)
        {
            IRadioGroup data = this.GetRadioObjectFromKey(ApplicationService.SavedRadioCollection.RadioHomePageData[homePageKey]) as IRadioGroup;
            if (data == null)
            {
                string[] delimeter = { "-" };
                data = RadioGroup.GetNewRadioStation();
                data.RadioName = homePageKey.Split(delimeter, StringSplitOptions.RemoveEmptyEntries)[1];
                ApplicationService.SavedRadioCollection.RadioHomePageData[homePageKey] = data.Key;
                _radioData.Add(data.Key, data);
            }
            return data;
        }
    }
}
