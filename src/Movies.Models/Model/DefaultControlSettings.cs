using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Models.Model
{
    public class DefaultControlSettings:NotificationObject
    {
        private bool _nextEnabled;

        public bool NextEnabled
        {
            get { return _nextEnabled; }
            set { _nextEnabled = value;RaisePropertyChanged(() => this.NextEnabled); }
        }

        private bool _previousEnabled;

        public bool PreviousEnabled
        {
            get { return _previousEnabled; }
            set { _previousEnabled = value; RaisePropertyChanged(() => this.PreviousEnabled); }
        }

        private bool _repeatEnabled;

        public bool RepeatToggleEnabled
        {
            get { return _repeatEnabled; }
            set { _repeatEnabled = value; RaisePropertyChanged(() => this.RepeatToggleEnabled); }
        }

        private bool _playlistEnabled;

        public bool PlaylistToggleEnabled
        {
            get { return _playlistEnabled; }
            set { _playlistEnabled = value; RaisePropertyChanged(() => this.PlaylistToggleEnabled); }
        }

        private bool _sliderEnabled;

        public bool SliderEnabled
        {
            get { return _sliderEnabled; }
            set { _sliderEnabled = value; RaisePropertyChanged(() => this.SliderEnabled); }
        }

        private bool _mediaOptionEnabled;

        public bool MediaOptionEnabled
        {
            get { return _mediaOptionEnabled; }
            set { _mediaOptionEnabled = value; RaisePropertyChanged(() => this.MediaOptionEnabled); }
        }

        private bool _fullscreenEnabled;

        public bool FullscreenEnabled
        {
            get { return _fullscreenEnabled; }
            set { _fullscreenEnabled = value; RaisePropertyChanged(() => this.FullscreenEnabled); }
        }
    }
}
