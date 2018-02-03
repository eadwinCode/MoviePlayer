using System.Windows;
using System.Windows.Controls;

namespace VideoPlayer
{
    internal interface IMediaController
    {
        MovieTitle_Tab MovieTitle_Tab { get; }
        Panel GroupedControls { get; }
        VolumeControl VolumeControl { get; }
    }
}