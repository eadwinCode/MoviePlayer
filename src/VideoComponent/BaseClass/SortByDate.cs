
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Movies.Models.Interfaces;

namespace VideoComponent.BaseClass
{
    public class SortByDate : IComparer<IItemSort>
    {
        public int Compare(IItemSort x, IItemSort y)
        {
            if (x == null || y == null)
                return 0;
            if (x.FileType == y.FileType)
            {
                var vv = x.CreationDate.CompareTo(y.CreationDate);
                return vv * -1;
            }
            else if (x.FileType == GroupCatergory.Grouped)
            {
                return -1;
            }
            return 1;
        }
    }
}
