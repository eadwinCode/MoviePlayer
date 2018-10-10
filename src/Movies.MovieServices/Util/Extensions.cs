using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Movies.MovieServices.Util
{
    public static class Extensions
    {

        //public static void AddRange<T>(this ObservableCollection<T> oc, IEnumerable<T> collection)
        //{
        //    if (collection == null) { throw new ArgumentNullException("collection"); }
        //    foreach (var i in collection) { oc.Add(i); }
        //}

        public static void Sort<T>(this ObservableCollection<T> observable) where T : IComparable<T>, IEquatable<T>
        {
            List<T> sorted = observable.OrderBy(x => x).ToList();
            int ptr = 0;

            while (ptr < sorted.Count)
            {
                if (observable[ptr].CompareTo(sorted[ptr]) > 0)
                {
                    T t = observable[ptr];
                    observable.RemoveAt(ptr);
                    observable.Insert(sorted.IndexOf(t), t);
                }
                else
                {
                    ptr++;
                }
            }
        }
    }
}
