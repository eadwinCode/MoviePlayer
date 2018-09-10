using Microsoft.Practices.Prism.ViewModel;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Movies.Models.Model
{
    public class RadioModel :NotificationObject, ICloneable
    {
        private RadioSort _radioSort;
        private string _stationName;
        private string _stationURL;
        private byte[] _stationLogo;
        private string _stationLocation;
        private string _stationBio;
        private string _bitRate;
        private string _sampleRate;
        private bool isactive;
        private string _channels;
        private string _audioFormat;

        public string StationName
        {
            get
            {
                return _stationName;
            }
            set { _stationName = value; }
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
                return string.Format("Radio Station: {0}\nLocation: {1}",StationName,StationLocation);
            }
        }

        public byte[] StationUidLogo
        {
            get
            {
                return _stationLogo;
            }
            set { _stationLogo = value; RaisePropertyChanged(() => this.StationUidLogo); }
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

        public string BitRate
        {
            get
            {
                return _bitRate;
            }
            set { _bitRate = value; }
        }
        public string SampleRate
        {
            get
            {
                return _sampleRate;
            }
            set { _sampleRate = value; }
        }
        public string Channels
        {
            get
            {
                return _channels;
            }
            set { _channels = value; }
        }
        public string AudioFormat {
            get
            {
                return _audioFormat;
            }
            set { _audioFormat = value; }
        }
        public RadioSort RadioSort
        {
            get
            {
                return _radioSort;
            }
            set { _radioSort = value; }
        }

        public bool IsActive
        {
            get { return isactive; }
            set { isactive = value;
                RaisePropertyChanged(() => this.IsActive); }
        }

        public object Clone()
        {
            return this;
        }

        public override string ToString()
        {
            return StationName;
        }
    }
}
