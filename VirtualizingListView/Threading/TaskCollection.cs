using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualizingListView.Threading
{
    public class TaskCollection
    {
        private IDictionary<string, Task> _collection;

        public TaskCollection()
        {
            _collection = new Dictionary<string, Task>();
        }

        internal void Add(Task task)
        {
            _collection.Add(task.GetSubscription(), task);
        }

        internal ICollection<Task> GetAll()
        {
            return _collection.Values;
        }

        internal void Remove(Task task)
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
