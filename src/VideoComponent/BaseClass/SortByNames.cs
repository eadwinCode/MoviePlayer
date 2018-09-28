
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
    public class SortByNames : IComparer<IItemSort>
    {
        public int Compare(IItemSort x, IItemSort y)
        {
            if (x == null || y == null)
                return -1;
            if (x.FileType == y.FileType)
            {
                return x.FileName.CompareTo(y.FileName);
            }
            else if (x.FileType == GroupCatergory.Grouped)
            {
                return -1;
            }
            return 1;
        }
    }
}
