using Microsoft.Practices.Prism.ViewModel;
using Movies.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Models.Model
{
    public class PlaylistModel :NotificationObject, IPlaylistModel
    {
        private List<Pathlist> itemsPath;

        private string playlistName;
        public string PlaylistName
        {
            get { return playlistName; }
            set { playlistName = value; RaisePropertyChanged(() => this.PlaylistName); }
        }

        private bool isactive;
        public bool IsActive
        {
            get { return isactive; }
        }

        public PlaylistModel()
        {
            itemsPath = new List<Pathlist>();
            SetIsActive(false);
        }

        public void Add(string filepath)
        {
            Pathlist pathlist = new Pathlist(filepath);
            if (!itemsPath.Contains(pathlist))
            {
                itemsPath.Add(pathlist);
            }
        }

        public void Add(Pathlist Pathlist)
        {
            if (!itemsPath.Contains(Pathlist))
            {
                itemsPath.Add(Pathlist);
            }
        }

        public override string ToString()
        {
            return this.PlaylistName;
        }

        public void SetIsActive(bool value)
        {
            isactive = value;
            RaisePropertyChanged(() => this.IsActive);
        }

        public ICollection<Pathlist> GetEnumerator
        {
            get { return itemsPath; }
        }

        public void Clear()
        {
            itemsPath.Clear();
        }
    }

    public sealed class Pathlist
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Uri UriPath { get; set; }
        public Pathlist(string filepath)
        {
            FilePath = filepath;
            FileName = new FileInfo(filepath).Name;
        }

        public Pathlist()
        {

        }

        public static Pathlist Parse(IPlayable playable)
        {
            var pathlist = new Pathlist() { UriPath = playable.Url, FileName = playable.MediaTitle };
            ILocaFilePlayable locaFilePlayable = playable as ILocaFilePlayable;
            if (locaFilePlayable != null)
                pathlist.FilePath = locaFilePlayable.FilePath;

            return pathlist;
        }
        
        public override bool Equals(object obj)
        {
            if (obj != null && (obj is Pathlist))
            {
                return ((Pathlist)obj).FilePath == this.FilePath;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
