using Common.Interfaces;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VideoComponent.BaseClass
{
    public interface IVideoData 
    {
        BitmapSource Thumbnail { get; set; }
        Visibility PlayedVisible { get; }
        string FileExtension { get;}
        uint FrameWidth { get; set; }
        uint FrameHeight { get; set; }
        string Duration { get; set; }
        Visibility SubVisible { get; }
        bool HasProgress { get; }
        string FileSize { get; set; }
        double Progress { get; }
        bool HasLastSeen { get; }
        string FilePath { get; }
        bool IsActive { get; set; }
        double? MaxiProgress { get; set; }
        ILastSeen LastPlayedPoisition { get; set; }
        // PlayedBefore Checked(VideoData file);
    }

    public interface IParentData
    {
        VideoFolder GetParentDirectory { get; }
    }
}
