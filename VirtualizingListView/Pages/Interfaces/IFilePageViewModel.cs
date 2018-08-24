using Common.Util;
using Movies.Models.Model;
using Movies.Enums;
using VideoComponent.BaseClass;

namespace VirtualizingListView.Pages.ViewModel
{
    public interface IFilePageViewModel
    {
        VideoFolder CurrentVideoFolder { get; set; }
        bool IsLoading { get; set; }
        ViewType ActiveViewType { get; set; }
    }
}