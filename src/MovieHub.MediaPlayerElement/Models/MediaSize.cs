using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MovieHub.MediaPlayerElement.Models
{
   public  class MediaSizeChangedRoutedArgs : RoutedEventArgs
    {
        public double _height;
        public double _width;

        internal MediaSizeChangedRoutedArgs(RoutedEvent routedEvent, object sender, double mediaheight,double mediawidth)
            : base(routedEvent, sender)
        {
            _width = mediawidth;
            this._height = mediaheight;
        }

        /// <summary>
        /// The New buffering value.
        /// </summary>
        public double MediaHeight
        {
            get
            {
                return _height;
            }
        }

        public double MediaWidth
        {
            get
            {
                return _width;
            }
        }
    }
}
