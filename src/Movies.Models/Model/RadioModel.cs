using Delimon.Win32.IO;
using Microsoft.Practices.Prism.ViewModel;
using Movies.Enums;
using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Movies.Models.Model
{
    public class RadioModel : NotificationObject, ICloneable, IRadioModel
    {
        private string _stationURL = "http://sc5.radiocaroline.net:8010/;";
        private string _stationLocation;
        private string _stationBio;
        private int _bitRate;
        private int _sampleRate;
        private bool isactive;
        private Channel _channels;
        private string _audioFormat;
        private string radioname;
        private DateTime creationdate;
        private bool canedit = true;
        private bool isfavorite = false;
        private string genre;
        private double frequency;
        private string _country;
        private Guid radioKey;

        public bool IsFavorite
        {
            get { return isfavorite; }
            set { isfavorite = value;RaisePropertyChanged(() => this.IsFavorite); }
        }

        public double Frequency
        {
            get { return frequency; }
            set { frequency = value; }
        }

        public string StationURL
        {
            get
            {
                return _stationURL;
            }
            set { _stationURL = value; }
        }

        public string TooltipMessage
        {
            get
            {
                return string.Format("Radio Station: {0}\nLocation: {1}",RadioName,StationLocation);
            }
        }
        
        public string StationLocation
        {
            get
            {
                return _stationLocation;
            }
            set { _stationLocation = value; }
        }

        public string StationBio
        {
            get
            {
                return _stationBio;
            }
            set { _stationBio = value; }
        }

        public int BitRate
        {
            get
            {
                return _bitRate;
            }
            set { _bitRate = value;
            }
        }

        public int SampleRate
        {
            get
            {
                return _sampleRate;
            }
            set {
                _sampleRate = value ;
            }
        }

        public Channel Channels
        {
            get
            {
                return _channels;
            }
            set { _channels = value; }
        }

        public string AudioFormat
        {
            get
            {
                return _audioFormat;
            }
            set { _audioFormat = value;
                if (!string.IsNullOrEmpty(value))
                    _audioFormat = value.ToUpper();
            }
        }

        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }
        
        public bool IsActive
        {
            get { return isactive; }
            set { isactive = value;
                RaisePropertyChanged(() => this.IsActive); }
        }

        public string MediaTitle { get { return RadioName; } }

        public Uri Url { get { return new Uri(_stationURL); } }

        public string Genre { get { return genre; } set { genre = value; } }
        
        public string RadioName
        {
            get { return radioname; }
            set
            {
                radioname = value;
                RaisePropertyChanged(() => this.RadioName);
            }
        }

        public bool CanEdit { get { return canedit; } }

        #region SortItems

        public string FileExtension { get { return ".radio"; } }

        public string FileName { get { return RadioName; } }

        public GroupCatergory FileType { get { return GroupCatergory.Child; } }

        public DateTime CreationDate { get { return creationdate; } }

        public Guid Key
        {
            get { return radioKey; }
            set
            {
                radioKey = value;
            }
        }

        #endregion

        public RadioModel(Guid guid)
        {
            creationdate = DateTime.Now;
            radioKey = guid;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static RadioModel GetRadioModel()
        {
            return new RadioModel(Guid.NewGuid()) { canedit = true, RadioName = " Edit this"};
        }

        public override string ToString()
        {
            return RadioName;
        }
        
    }
}
