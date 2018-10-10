using Common.Util;
using Movies.Models.Model;
using Movies.Enums;

namespace VirtualizingListView.Pages.ViewModel
{
    public interface IFilePageViewModel
    {
        VideoFolder CurrentVideoFolder { get; set; }
        bool IsLoading { get; set; }
        ViewType ActiveViewType { get; set; }
    }
}