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
    public class SortByExtension : IComparer<IItemSort>
    {
        public int Compare(IItemSort x, IItemSort y)
        {
            if (x == null || y == null)
                return 0;
            if (x.FileType == GroupCatergory.Grouped) 
            {
                return 0;
            }
            else if (x.FileType == GroupCatergory.Child && y.FileType == GroupCatergory.Child)
            {
                return x.FileExtension.CompareTo(y.FileExtension);
            }
            return 1;
        }
    }
}
