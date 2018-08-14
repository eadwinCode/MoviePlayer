using Common.Interfaces;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoComponent.BaseClass
{
   

    public interface IParentData
    {
        VideoFolder GetParentDirectory { get; }
    }
}
