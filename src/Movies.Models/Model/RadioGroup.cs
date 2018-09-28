using Delimon.Win32.IO;
using Microsoft.Practices.Prism.ViewModel;
using Movies.Enums;
using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Movies.Models.Model
{
    public class RadioGroup : NotificationObject, IRadioGroup,ICloneable, IRadioStations
    {
        private string radioname;
        private ObservableCollection<Guid> radiostations;
        private bool canedit = false;
        private DateTime creationdate;
        private Guid radiokey;

        public string RadioName
        {
            get { return radioname; }
            set { radioname = value; RaisePropertyChanged(() => this.RadioName); }
        }

        public ObservableCollection<Guid> RadioStations
        {
            get { return radiostations; }
            set { radiostations = value; RaisePropertyChanged(() => this.RadioStations); }
        }
        

        public bool CanEdit
        {
            get { return canedit; }
        }

        #region SortItems

        public string FileExtension { get { return ".group"; } }

        public string FileName
        {
            get { return RadioName; }
        }

        public GroupCatergory FileType { get { return GroupCatergory.Grouped; } }

        public DateTime CreationDate { get { return creationdate; } }

        public Guid Key { get { return radiokey; } set { radiokey = value; } }

        #endregion

        public RadioGroup()
        {
            RadioStations = new ObservableCollection<Guid>();
            creationdate = DateTime.Now;
            canedit = false;
            radiokey = Guid.NewGuid();
        }

        public void AddStation(Guid imoviesradio)
        {
            if (!RadioStations.Contains(imoviesradio))
                this.RadioStations.Add(imoviesradio);
        }

        public void RemoveStation(Guid imoviesradio)
        {
            this.RadioStations.Remove(imoviesradio);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        
        public static RadioGroup GetNewRadioStation()
        {
            return new RadioGroup() { canedit = true, RadioName = " Edit this" };
        }

        public IMoviesRadio GetNewRadioModel()
        {
            return RadioModel.GetRadioModel();
        }
    }
}
