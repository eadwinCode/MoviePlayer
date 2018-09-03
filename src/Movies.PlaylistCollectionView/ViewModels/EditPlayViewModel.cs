using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Movies.Models.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Movies.PlaylistCollectionView.ViewModels
{

    public class EditPlayViewModel : NotificationObject
    {
        public Action CloseAction;
        private List<Pathlist> removePlaylist;
        private ObservableCollection<Pathlist> playcollection;
        public ObservableCollection<Pathlist> PlaylistCollection
        {
            get { return playcollection; }
            set { playcollection = value; RaisePropertyChanged(()=>this.PlaylistCollection); }
        }
        private PlaylistModel currentplaylistmodel;
        public PlaylistModel CurrentPlaylist
        {
            get { return currentplaylistmodel; }
            set { currentplaylistmodel = value; RaisePropertyChanged(() => this.PlaylistCollection); }
        }
        
        public DelegateCommand savecommand;
        public DelegateCommand SaveCommand
        {
            get
            {
                if (savecommand == null)
                    savecommand = new DelegateCommand(() =>
                    {
                        SaveAction();
                    });
                return savecommand;
            }
        }

        private void SaveAction()
        {
            foreach (var item in removePlaylist)
            {
                if(currentplaylistmodel.GetEnumerator.Contains(item))
                    currentplaylistmodel.GetEnumerator.Remove(item);
            }
            CloseAction.Invoke();
        }

        public DelegateCommand<Pathlist> removeitemcommand;
        public DelegateCommand<Pathlist> RemoveItemCommand
        {
            get
            {
                if (removeitemcommand == null)
                    removeitemcommand = new DelegateCommand<Pathlist>((d) =>
                    {
                        RemoveAction(d);
                    });
                return removeitemcommand;
            }
        }
        

        public string PlayCollectionCountString
        {
            get { return PlaylistCollection.Count + " Item(s)"; }
        }


        private void RemoveAction(Pathlist pathlist)
        {
            if(PlaylistCollection.Contains(pathlist))
                PlaylistCollection.Remove(pathlist);

            if (!removePlaylist.Contains(pathlist))
                removePlaylist.Add(pathlist);

            RaisePropertyChanged(() => this.PlayCollectionCountString);
        }

        public EditPlayViewModel(PlaylistModel playlistModel)
        {
            PlaylistCollection = new ObservableCollection<Pathlist>(playlistModel.GetEnumerator);
            CurrentPlaylist = playlistModel;
            removePlaylist = new List<Pathlist>();
        }
    }
}
