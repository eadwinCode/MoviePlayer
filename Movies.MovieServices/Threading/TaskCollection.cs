using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.MovieServices.Threading
{
    public class TaskCollection
    {
        private IDictionary<string, MovieTask> _collection;

        public TaskCollection()
        {
            _collection = new Dictionary<string, MovieTask>();
        }

        internal void Add(MovieTask task)
        {
            _collection.Add(task.GetSubscription(), task);
        }

        internal ICollection<MovieTask> GetAll()
        {
            return _collection.Values;
        }

        internal void Remove(MovieTask task)
        {
            if (_collection.ContainsKey(task.GetSubscription()))
            {
                _collection.Remove(task.GetSubscription());
            }
        }

        internal int Count()
        {
            return _collection.Count();
        }
    }
}
