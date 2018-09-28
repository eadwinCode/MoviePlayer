using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MovieHub.MediaPlayerElement.Models
{
    public class WindowFullScreenRoutedEventArgs : RoutedEventArgs
    {
        private WindowFullScreenState _fullscreenstate;

        internal WindowFullScreenRoutedEventArgs(RoutedEvent routedEvent, object sender, WindowFullScreenState state)
            : base(routedEvent, sender)
        {
            _fullscreenstate = state;
        }

        /// <summary>
        /// The New buffering value.
        /// </summary>
        public WindowFullScreenState FullScreenState
        {
            get
            {
                return _fullscreenstate;
            }
        }
    }
}
