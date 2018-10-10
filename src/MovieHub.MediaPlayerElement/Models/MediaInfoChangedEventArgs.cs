using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MovieHub.MediaPlayerElement.Models
{
    public class MediaInfoChangedEventArgs : RoutedEventArgs
    {
        private MediaInformation _mediainfo;

        internal MediaInfoChangedEventArgs(RoutedEvent routedEvent, object sender, MediaInformation mediainfo)
            : base(routedEvent, sender)
        {
            _mediainfo = mediainfo;
        }

        /// <summary>
        /// The New buffering value.
        /// </summary>
        public MediaInformation MediaInformation
        {
            get
            {
                return _mediainfo;
            }
        }
    }
}
