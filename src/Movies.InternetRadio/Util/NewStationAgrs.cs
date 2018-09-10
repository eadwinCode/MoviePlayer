using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.InternetRadio.EventHandle
{
    public class NewStationAgrs : EventArgs
    {
       public RadioModel RadioModel { get; set; }
    }
}
