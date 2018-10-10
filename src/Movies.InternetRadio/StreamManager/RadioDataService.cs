using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MovieServices.Services;
using Movies.MoviesInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

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
                var item = _radioData[key];
                if (item is IRadioGroup && item.CanEdit == true)
                    RemoveRadioGroupItems(item as IRadioGroup);

                if(item.CanEdit)
                    _radioData.Remove(key);

                radioGroup.RemoveStation(key);
                return true;
            }
            return false;
        }

        private void RemoveRadioGroupItems(IRadioGroup radiogroup)
        {
            foreach (var item in radiogroup.RadioStations)
            {
                RemoveRadio(radiogroup, item);
            }
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
                else
                {

                }
            }
            return data;
        }

        public IEnumerator<KeyValuePair<Guid, IMoviesRadio>> GetEnumerator()
        {
            return _radioData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        internal IRadioGroup GetHomeGroup(string homePageKey)
        {
            IRadioGroup data = this.GetRadioObjectFromKey(ApplicationService.SavedRadioCollection.RadioHomePageData[homePageKey]) as IRadioGroup;
            if (data == null)
            {
                string[] delimeter = { "-" };
                data = new DefaultRadioGroup();
                data.RadioName = homePageKey.Split(delimeter, StringSplitOptions.RemoveEmptyEntries)[1];
                ApplicationService.SavedRadioCollection.RadioHomePageData[homePageKey] = data.Key;
                _radioData.Add(data.Key, data);
            }
            return data;
        }

        internal void LoadDefaultRadioData()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var rssdsd = assembly.GetManifestResourceStream("Movies.InternetRadio.StreamManager.DefaultRadioData.xds");
            var defaultRadios = (SavedRadioCollection)(ApplicationService as ApplicationService).LoadRadioFiles(rssdsd);

            foreach (var item in defaultRadios.RadioHomePageData)
            {
                IRadioGroup radioGroup = GetHomeGroup(item.Key);
                IRadioGroup itemRadiogroup = defaultRadios.RadioCollection[item.Value] as IRadioGroup;
                foreach (var stationKey in itemRadiogroup.RadioStations)
                {
                    radioGroup.AddStation(stationKey);
                }
            }

            foreach (var item in defaultRadios.RadioCollection)
            {
                if (defaultRadios.RadioHomePageData.ContainsValue(item.Key))
                    continue;

                _radioData.Add(item.Key, item.Value);
            }
        }

    }
}
