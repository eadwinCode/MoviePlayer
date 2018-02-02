using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoComponent.BaseClass
{
    public interface IPlayedBefore
    {
        string fileName { get; set; }
        double Duration { get; set; }
    }
}
