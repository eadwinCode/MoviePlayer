﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VirtualizingLib.Interfaces;

namespace VirtualizingLib
{
    public class VirtualizingCollection<T> : IList<T>, IList
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageTimeout">The page timeout.</param>
        public VirtualizingCollection(int pageSize, int pageTimeout):this()
        {
            var no_of_items = _itemsProvider.GetItemsCount();
            if (no_of_items > 30)
            {
                _pageSize = 30;
            }
            else
                _pageSize = no_of_items;

            _pageTimeout = pageTimeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
        public VirtualizingCollection(int pageSize):this()
        {
            _pageSize = pageSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        public VirtualizingCollection()
        {
            _itemsProvider = new CollectionProvider<T>();
            (_itemsProvider as CollectionProvider<T>).CollectionChanged += VirtualizingCollection_CollectionChanged;
        }

        protected virtual void VirtualizingCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LoadCount();
        }

        #endregion

        #region ItemsProvider

        private readonly ICollectionProvider<T> _itemsProvider;

        /// <summary>
        /// Gets the items provider.
        /// </summary>
        /// <value>The items provider.</value>
        public ICollectionProvider<T> ItemsProvider
        {
            get { return _itemsProvider; }
        }

        #endregion

        private int _pageindex = -1;

        public int Pageindex
        {
            get { return _pageindex; }
            set
            {
                if (value != _pageindex)
                {

                    if (!(_pages.Count > 0))
                    {
                        Trace.WriteLine("PageIndex waiting for values");
                        _pageindex = -1;
                        return;
                    }

                    if (value > _pages.Count - 1)
                    {
                        _pageindex = value;
                        return;
                    }

                    if (PageIndexChangedEvent != null && _pages[value] != null)
                    {
                        _pageindex = value;
                        PageIndexChangedEvent(_pages[_pageindex]); Trace.WriteLine("PageChange Event Fired");
                    }
                }

            }
        }

        #region PageSize

        private readonly int _pageSize = 100;

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize
        {
            get { return _pageSize; }
        }

        #endregion

        #region PageTimeout

        private readonly long _pageTimeout = 10000;

        /// <summary>
        /// Gets the page timeout.
        /// </summary>
        /// <value>The page timeout.</value>
        public long PageTimeout
        {
            get { return _pageTimeout; }
        }

        #endregion

        #region IList<T>, IList

        #region Count

        private int _count = -1;
        private int currentPageIndex = -1;
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// The first time this property is accessed, it will fetch the count from the IItemsProvider.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public virtual int Count
        {
            get
            {
                //if (_count == -1)
                //{
                //    LoadCount();
                //}
                // return _count;
                return _itemsProvider.Count();
            }
            //protected set
            //{
            //    _count = value;
            //}
        }

        #endregion

        #region Indexer

        /// <summary>
        /// Gets the item at the specified index. This property will fetch
        /// the corresponding page from the IItemsProvider if required.
        /// </summary>
        /// <value></value>
        public T this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set { throw new NotSupportedException(); }
        }

        private T GetItem(int index)
        {
            // determine which page and offset within page
            currentPageIndex = index / PageSize;
            int pageOffset = index % PageSize;
            SetPageindex(index, pageOffset);

            // request primary page
            RequestPage(currentPageIndex);

            if (HasMorePages)
            { 
                // if accessing upper 50% then request next page
                if (pageOffset > PageSize / 2 && currentPageIndex < Count / PageSize)
                    RequestPage(currentPageIndex + 1);
                
                // if accessing lower 50% then request prev page
                if (pageOffset < PageSize / 2 && currentPageIndex > 0)
                    RequestPage(currentPageIndex - 1);

                //if (this.HasDeletePage)
                //{
                //    if (pageIndex > 0 && pageIndex < 4)
                //    {
                //        int page_to_reload = _removeKeys.LastOrDefault();
                //        if (page_to_reload > 0)
                //        {
                //            GetDeletedPage(page_to_reload);
                //        }
                //    }

                //}
                // remove stale pages
                CleanUpPages();
            }
            // defensive check in case of async load
            if (_pages[currentPageIndex] == null)
            {
                return default(T);
            }

            if (pageOffset > _pages[currentPageIndex].Count - 1)
            {
                ReloadPage(currentPageIndex);
            }

            // return requested item
            return _pages[currentPageIndex][pageOffset];
        }

        private void GetDeletedPage(int page_to_reload)
        {
            _removeKeys.Remove(page_to_reload);
            RequestPage(page_to_reload);
            Trace.WriteLine("Reload Page: " + page_to_reload);
        }

        private void ReloadPage(int pageIndex)
        {
            _pages[pageIndex] =  FetchPage(pageIndex);
        }

        private void SetPageindex(int pageIndex, int pageOffset)
        {
            if (Pageindex < 0)
            {
                Pageindex = (pageIndex / PageSize);
                return;
            }

            if (pageOffset < PageSize / 2 && HasMorePages)
            {
                Pageindex = (pageIndex / PageSize);
            }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region IEnumerator<T>, IEnumerator

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <remarks>
        /// This method should be avoided on large collections due to poor performance.
        /// </remarks>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Add

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Contains

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// Always false.
        /// </returns>
        public bool Contains(T item)
        {
            return false;
        }

        #endregion

        #region Clear

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IndexOf

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// Always -1.
        /// </returns>
        public int IndexOf(T item)
        {
            return -1;
        }

        #endregion

        #region Insert

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region CopyTo

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// -or-
        /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Misc

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>Always false.
        /// </returns>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>Always true.
        /// </returns>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>Always false.
        /// </returns>
        public bool IsFixedSize
        {
            get { return false; }
        }

        #endregion

        #endregion

        #region Paging

        private readonly Dictionary<int, IList<T>> _pages = new Dictionary<int, IList<T>>();
        private readonly Dictionary<int, DateTime> _pageTouchTimes = new Dictionary<int, DateTime>();
        private readonly IList<int> _removeKeys = new List<int>();

        /// <summary>
        /// Cleans up any stale pages that have not been accessed in the period dictated by PageTimeout.
        /// </summary>
        internal void CleanUpPages()
        {
            List<int> keys = new List<int>(_pageTouchTimes.Keys);
            foreach (int key in keys)
            {
                // page 0 is a special case, since WPF ItemsControl access the first item frequently
                if (key != 0 && (DateTime.Now - _pageTouchTimes[key]).TotalMilliseconds > PageTimeout)
                {
                    _pages.Remove(key);
                    _pageTouchTimes.Remove(key);
                    Trace.WriteLine("Removed Page: " + key);
                }
            }

        }

        /// <summary>
        /// Populates the page within the dictionary.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="page">The page.</param>
        protected virtual void PopulatePage(int pageIndex, IList<T> page)
        {
            Trace.WriteLine("Page populated: " + pageIndex);
            if (_pages.ContainsKey(pageIndex))
            {
                _pages[pageIndex] = page;
            }
        }

        /// <summary>
        /// Makes a request for the specified page, creating the necessary slots in the dictionary,
        /// and updating the page touch time.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void RequestPage(int pageIndex)
        {
            if (!_pages.ContainsKey(pageIndex))
            {
                _pages.Add(pageIndex, null);
                _pageTouchTimes.Add(pageIndex, DateTime.Now);
                Trace.WriteLine("Added page: " + pageIndex);

                LoadPage(pageIndex);
            }
            else
            {
                _pageTouchTimes[pageIndex] = DateTime.Now;
            }
        }

        #endregion

        #region Load methods

        /// <summary>
        /// Loads the count of items.
        /// </summary>
        protected virtual void LoadCount()
        {
           // Count = FetchCount();
        }

        /// <summary>
        /// Loads the page of items.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void LoadPage(int pageIndex)
        {
            PopulatePage(pageIndex, FetchPage(pageIndex));
        }

        #endregion

        #region Fetch methods

        /// <summary>
        /// Fetches the requested page from the IItemsProvider.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns></returns>
        protected IList<T> FetchPage(int pageIndex)
        {
            return ItemsProvider.FetchRange(pageIndex * PageSize, PageSize);
        }

        /// <summary>
        /// Fetches the count of itmes from the IItemsProvider.
        /// </summary>
        /// <returns></returns>
        protected int FetchCount()
        {
            return ItemsProvider.FetchCount();
        }

        #endregion

        public delegate void ExecuteCommand(object sender);
        public ExecuteCommand PageIndexChangedEvent;
        private bool HasDeletePage { get { return _removeKeys.Count > 0; } }

        public bool HasMorePages { get { return Count - PageSize > 0; } }
    }
}
