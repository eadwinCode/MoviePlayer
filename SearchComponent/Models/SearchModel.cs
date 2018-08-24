using Movies.Models.Interfaces;
using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchComponent.Model
{
    public class SearchModel : ISearchModel
    {
        public ICollection<VideoFolder> Results { get; set; }
        public string SearchQuery { get; set; }

        public SearchModel()
        {
        }
    }
    
}
