using System.Collections.Generic;
using System.Collections.ObjectModel;
using Movies.Enums;
using Movies.Models.Interfaces;
using Movies.Models.Model;

namespace Movies.MoviesInterfaces
{
    public interface ISortService
    {
        IEnumerable<T> SortList<T>(SortType sorttype, IEnumerable<T> list) where T : IItemSort;
        VideoFolder SortList(SortType sorttype, VideoFolder parent);
    }
}