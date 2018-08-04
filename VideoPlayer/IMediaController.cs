using System.Windows;
using System.Windows.Controls;

namespace VideoPlayerControl
{
    internal interface IController
    {
        MovieTitle_Tab MovieTitle_Tab { get; }
        Panel GroupedControls { get; }
        VolumeControl VolumeControl { get; }
    }
}