using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Models.Interfaces
{
    public interface IPlayable
    {
        bool IsActive { get; set; }
        string MediaTitle { get; }
        Uri Url { get; }
    }
}
