using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoComponent.BaseClass;

namespace VirtualizingListView.Pages.Model
{
    public class SearchModel
    {
        public ICollection<VideoFolder> Results;
        public string SearchQuery;
    }
}
