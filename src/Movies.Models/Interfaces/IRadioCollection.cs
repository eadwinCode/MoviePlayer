using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Models.Interfaces
{
    public interface IMoviesRadio : IItemSort
    {
        string RadioName { get; set; }
        bool CanEdit { get; }
        Guid Key { get; set; }
    }
}
