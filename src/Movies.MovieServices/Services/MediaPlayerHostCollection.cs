using Movies.MoviesInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.MovieServices.Services
{
    internal class MediaPlayerHostCollection : IMediaPlayerHostCollection
    {
        IList<IMediaPlayerHost> _collection;

        public MediaPlayerHostCollection()
        {
            _collection =new List<IMediaPlayerHost>();
        }

        public void Add(IMediaPlayerHost mediaPlayerHost)
        {
            if (!_collection.Contains(mediaPlayerHost))
                _collection.Add(mediaPlayerHost);
        }

        public IEnumerator<IMediaPlayerHost> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public void Remove(IMediaPlayerHost mediaPlayerHost)
        {
            if (_collection.Contains(mediaPlayerHost))
                _collection.Remove(mediaPlayerHost);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
    }
}
