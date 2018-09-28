using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MovieHub.MediaPlayerElement.Models
{
    public class MediaPlayerViewTypeRoutedEventArgs : RoutedEventArgs
    {
        private MediaPlayerViewType _mediaplayerviewtype;

        internal MediaPlayerViewTypeRoutedEventArgs(RoutedEvent routedEvent, object sender, MediaPlayerViewType state)
            : base(routedEvent, sender)
        {
            _mediaplayerviewtype = state;
        }

        /// <summary>
        /// The New buffering value.
        /// </summary>
        public MediaPlayerViewType MediaPlayerViewTypeState
        {
            get
            {
                return _mediaplayerviewtype;
            }
        }
    }
}
