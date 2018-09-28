using Movies.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movies.Models.Model
{
    public class DummyPlayableFile : IPlayable
    {
        private string mediatitle = "- No Media title -";
        private Uri mediaUri;
        public bool IsActive { get ; set; }

        public string MediaTitle { get { return mediatitle; } }

        public Uri Url { get { return mediaUri; } }

        private DummyPlayableFile(Uri uri)
        {
            this.mediaUri = uri;
        }

        public static DummyPlayableFile Parse(Uri uri)
        {
            var dummy = new DummyPlayableFile(uri);
            return dummy;
        }
    }


}
