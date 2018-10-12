//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Windows.Threading;
//using VirtualizingLib.Interfaces;

//namespace VirtualizingLib
//{
//    public class CollectionProvider<T> : ObservableCollection<T>, ICollectionProvider<T>
//    {
//        public int FetchCount()
//        {
//            return this.Count();
//        }

//        public IList<T> FetchRange(int startIndex, int count)
//        {
//            Trace.WriteLine("FetchRange: " + startIndex + "," + count);
//            count = GetPossibleNextItemsRange(startIndex, count);
//            Thread.Sleep(1000);

//            ObservableCollection<T> list = new ObservableCollection<T>();

//            for (int i = startIndex; i < startIndex + count; i++)
//            {
//                list.Add(this[i]);
//            }
//            return list;
//        }

//        private int GetPossibleNextItemsRange(int startIndex, int count)
//        {
//            int nextmove = startIndex + count;
//            int possiblerange = this.Count - nextmove;
//            if (possiblerange < 0)
//            {
//                return this.Count - startIndex;
//            }
//            else
//                return count;
//        }

//        public int GetItemsCount()
//        {
//            return this.Count();
//        }

//        public void DataSource(ObservableCollection<T> source)
//        {
//            //throw new NotImplementedException();
//        }
//    }
//}
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using VirtualizingLib.Interfaces;

namespace VirtualizingLib
{
    public class CollectionProvider<T> : ICollectionProvider<T>
    {
        private ObservableCollection<T> _dataSource;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public int Count { get { return _dataSource.Count; } }

        public bool IsReadOnly { get { return true; } }

        public T this[int index]
        {
            get { return _dataSource[index]; }
            set
            {
                _dataSource[index] = value;
            }
        }

        public CollectionProvider()
        {
            _dataSource = new ObservableCollection<T>();
            HookUpEvents();
        }

        public int FetchCount()
        {
            return this.Count();
        }

        public IList<T> FetchRange(int startIndex, int count)
        {
            Trace.WriteLine("FetchRange: " + startIndex + "," + count);
            count = GetPossibleNextItemsRange(startIndex, count);
            Thread.Sleep(1000);

            ObservableCollection<T> list = new ObservableCollection<T>();

            for (int i = startIndex; i < startIndex + count; i++)
            {
                list.Add(this[i]);
            }
            return list;
        }

        private int GetPossibleNextItemsRange(int startIndex, int count)
        {
            int nextmove = startIndex + count;
            int possiblerange = this.Count - nextmove;
            if (possiblerange < 0)
            {
                return this.Count - startIndex;
            }
            else
                return count;
        }

        public int GetItemsCount()
        {
            return this.Count();
        }

        public void DataSource(ObservableCollection<T> source)
        {
            if (_dataSource != null)
            {
                UnHookEvents();
            }
            _dataSource = source;
            HookUpEvents();
        }

        private void HookUpEvents()
        {
            if (_dataSource != null)
            {
                _dataSource.CollectionChanged += _dataSource_CollectionChanged;
                RaiseChangedEvent(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private void _dataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaiseChangedEvent(sender, e);
        }

        protected void RaiseChangedEvent(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged.Invoke(sender, e);
        }

        private void UnHookEvents()
        {
            _dataSource.CollectionChanged -= _dataSource_CollectionChanged;
        }

        public int IndexOf(T item)
        {
            return _dataSource.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _dataSource.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _dataSource.RemoveAt(index);
        }

        public void Add(T item)
        {
            _dataSource.Add(item);
        }

        public void Clear()
        {
            _dataSource.Clear();
        }

        public bool Contains(T item)
        {
            return _dataSource.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _dataSource.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _dataSource.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dataSource.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
