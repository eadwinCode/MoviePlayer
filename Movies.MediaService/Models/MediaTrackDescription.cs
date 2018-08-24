using Meta.Vlc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Movies.MediaService.Models
{
    public class MediaTrackDescription : TrackDescription, INotifyPropertyChanged
    {
        private bool isselected;

        public bool IsSelected
        {
            get
            {
                return isselected;
            }

            set
            {
                isselected =value;
                OnPropertyChanged("IsSelected");
            }
        }

        public MediaTrackDescription(IntPtr pointer) :
            base(pointer)
        {

        }
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
