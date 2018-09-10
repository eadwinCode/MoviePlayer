using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.MediaService.Models
{
    public class MediaBufferingEventArgs : EventArgs
    {
        public Double NewCache { get; set; }
    }
}
