using Meta.Vlc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.MediaService.Models
{
    public class MediaTrackDescriptionList : IDisposable, IEnumerable<MediaTrackDescription>, IEnumerable
    {
        private List<MediaTrackDescription> _list;
        private IntPtr _pointer;
        public IntPtr Pointer { get { return _pointer; } }
        public event EventHandler OnPropertyChanged;
        /// <summary>
        ///     Create a readonly list by a pointer of <see cref="Interop.MediaPlayer.TrackDescription" />.
        /// </summary>
        /// <param name="pointer"></param>
        public MediaTrackDescriptionList(IntPtr pointer)
        {
            _list = new List<MediaTrackDescription>();
            _pointer = pointer;

            while (pointer != IntPtr.Zero)
            {
                var trackDescription = new MediaTrackDescription(pointer);
                trackDescription.PropertyChanged += TrackDescription_PropertyChanged;
                _list.Add(trackDescription);

                pointer = trackDescription.Struct.Next;
            }
        }

        private void TrackDescription_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OnPropertyChanged != null)
                OnPropertyChanged.Invoke(sender, EventArgs.Empty);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public MediaTrackDescription this[int index]
        {
            get { return _list[index]; }
        }

        public void Dispose()
        {
            if (_pointer == IntPtr.Zero) return;

            LibVlcManager.ReleaseTrackDescriptionList(_pointer);
            _pointer = IntPtr.Zero;
            _list.Clear();
        }

        public IEnumerator<MediaTrackDescription> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
