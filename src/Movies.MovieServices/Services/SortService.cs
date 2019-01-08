using Microsoft.Practices.Prism;
using Movies.Enums;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MovieServices.Util;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Movies.MovieServices.Services
{
    internal class SortService : ISortService
    {
        private object _lock = new object();
        public MediaFolder SortList(SortType sorttype, MediaFolder parent)
        {
            lock (_lock)
            {
                if (parent.OtherFiles == null) return parent;

                ObservableCollection<MediaFolder> asd = new ObservableCollection<MediaFolder>();
                if (sorttype == SortType.Date)
                {
                    IEnumerable<MediaFolder> de = (parent.OtherFiles).OrderBy(x => x, new SortByDate());
                    asd.AddRange(de);
                }
                else if (sorttype == SortType.Extension)
                {
                    IEnumerable<MediaFolder> de = parent.OtherFiles.OrderBy(x => x, new SortByExtension());
                    asd.AddRange(de);
                }
                else
                {
                    IEnumerable<MediaFolder> de = parent.OtherFiles.OrderBy(x => x, new SortByNames());
                    asd.AddRange(de);
                }
                parent.OtherFiles.Clear();
                parent.OtherFiles = asd;
                parent.SortedBy = sorttype;
                return parent;
            }

        }

        public ObservableCollection<MediaFolder> SortList(SortType sorttype, ObservableCollection<MediaFolder> list)
        {
            lock (this)
            {
                if (list == null) return list;

                ObservableCollection<MediaFolder> asd = new ObservableCollection<MediaFolder>();
                if (sorttype == SortType.Date)
                {
                    IEnumerable<MediaFolder> de = (list).OrderBy(x => x, new SortByDate());
                    asd.AddRange(de);
                }
                else if (sorttype == SortType.Extension)
                {
                    IEnumerable<MediaFolder> de = list.OrderBy(x => x, new SortByExtension());
                    asd.AddRange(de);
                }
                else
                {
                    IEnumerable<MediaFolder> de = list.OrderBy(x => x, new SortByNames());
                    asd.AddRange(de);
                }
                return asd;
            }
           
        }

        public IEnumerable<T> SortList<T>(SortType sorttype, IEnumerable<T> list) where T : IItemSort
        {
            lock (this)
            {
                if (list == null) return list;

                ObservableCollection<T> asd = new ObservableCollection<T>();
                if (sorttype == SortType.Date)
                {
                    IEnumerable<T> de = (list).OrderBy(x => x, new SortByDate());
                    asd.AddRange(de);
                }
                else if (sorttype == SortType.Extension)
                {
                    IEnumerable<T> de = list.OrderBy(x => x, new SortByExtension());
                    asd.AddRange(de);
                }
                else
                {
                    IEnumerable<T> de = list.OrderBy(x => x, new SortByNames());
                    asd.AddRange(de);
                }
                return asd;
            }
        }
    }
}
