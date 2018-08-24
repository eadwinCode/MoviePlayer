using Movies.MoviesInterfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace SearchComponent
{
    public interface ISearchControl<T>: ISearchControl
    {
        SearchMode SearchPattern { get; set; }
        IDataSource<T> DataSource { get; set; }
        event SearchControl<T>.OnSearchAsynStart OnSearchStarted;
        event PropertyChangedEventHandler PropertyChanged;
        event SearchControl<T>.UpdateHandler UpdateEvent;
    }
}