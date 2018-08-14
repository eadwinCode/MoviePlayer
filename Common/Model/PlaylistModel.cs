using Common.Interfaces;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class PlaylistModel :NotificationObject, IPlaylistModel
    {
        private List<string> itemsPath;
        public List<string> ItemsPaths { get { return itemsPath; } }

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
            itemsPath = new List<string>();
            SetIsActive(false);
        }

        public void Add(string filepath)
        {
            if (!itemsPath.Contains(filepath))
            {
                itemsPath.Add(filepath);
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
        
    }
}
