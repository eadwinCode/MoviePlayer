using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MovieHub.MediaPlayerElement.Models
{
    public class MediaBufferingEventArgs : RoutedEventArgs
    {
        private Double _newcache;

        internal MediaBufferingEventArgs(RoutedEvent routedEvent, object sender, Double newcache)
            : base(routedEvent, sender)
        {
            _newcache = newcache;
        }

        /// <summary>
        /// The New buffering value.
        /// </summary>
        public double NewCache
        {
            get
            {
                return _newcache;
            }
        }
    }
}
