using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace VirtualizingLib.Interfaces
{
    /// <summary>
    /// Represents a provider of collection details.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface ICollectionProvider<T> : IList<T>, IEnumerable<T>
    {
        /// <summary>
        /// Fetches the total number of items available.
        /// </summary>
        /// <returns></returns>
        int FetchCount();

        /// <summary>
        /// Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
        IList<T> FetchRange(int startIndex, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetItemsCount();
        /// <summary>
        /// Set DataSource to pick data
        /// </summary>
        /// <param name="source"></param>
        void DataSource(ObservableCollection<T> source);
    }
}
