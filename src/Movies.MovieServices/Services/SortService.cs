﻿using Microsoft.Practices.Prism;
using Movies.Enums;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using VideoComponent.BaseClass;

namespace Movies.MovieServices.Services
{
    internal class SortService : ISortService
    {
        private object _lock = new object();
        public VideoFolder SortList(SortType sorttype, VideoFolder parent)
        {
            lock (_lock)
            {
                if (parent.OtherFiles == null) return parent;

                ObservableCollection<VideoFolder> asd = new ObservableCollection<VideoFolder>();
                if (sorttype == SortType.Date)
                {
                    IEnumerable<VideoFolder> de = (parent.OtherFiles).OrderBy(x => x, new SortByDate());
                    asd.AddRange(de);
                }
                else if (sorttype == SortType.Extension)
                {
                    IEnumerable<VideoFolder> de = parent.OtherFiles.OrderBy(x => x, new SortByExtension());
                    asd.AddRange(de);
                }
                else
                {
                    IEnumerable<VideoFolder> de = parent.OtherFiles.OrderBy(x => x, new SortByNames());
                    asd.AddRange(de);
                }
                parent.OtherFiles.Clear();
                parent.OtherFiles = asd;
                //parent.SortedBy = sorttype;
                return parent;
            }

        }

        public ObservableCollection<VideoFolder> SortList(SortType sorttype, ObservableCollection<VideoFolder> list)
        {
            lock (this)
            {
                if (list == null) return list;

                ObservableCollection<VideoFolder> asd = new ObservableCollection<VideoFolder>();
                if (sorttype == SortType.Date)
                {
                    IEnumerable<VideoFolder> de = (list).OrderBy(x => x, new SortByDate());
                    asd.AddRange(de);
                }
                else if (sorttype == SortType.Extension)
                {
                    IEnumerable<VideoFolder> de = list.OrderBy(x => x, new SortByExtension());
                    asd.AddRange(de);
                }
                else
                {
                    IEnumerable<VideoFolder> de = list.OrderBy(x => x, new SortByNames());
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
