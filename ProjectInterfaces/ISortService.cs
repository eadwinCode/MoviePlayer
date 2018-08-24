using System.Collections.ObjectModel;
using Movies.Enums;
using Movies.Models.Model;

namespace Movies.MoviesInterfaces
{
    public interface ISortService
    {
        ObservableCollection<VideoFolder> SortList(SortType sorttype, ObservableCollection<VideoFolder> list);
        VideoFolder SortList(SortType sorttype, VideoFolder parent);
    }
}