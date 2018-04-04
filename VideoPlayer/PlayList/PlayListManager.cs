using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using VideoComponent.BaseClass;
using VideoPlayer.ViewModel;
using VirtualizingListView.Util;
using VirtualizingListView.View;

namespace VideoPlayer.PlayList
{
    public class PlayListManager:NotificationObject,IHasChanges
    {
        //private static PlayListManager instance = new PlayListManager();

        //public static PlayListManager Current
        //{
        //    get { return instance; }
        //}
        private bool hasSubcribed = false;
        private PlaylistModel currentplaylist;
        public PlaylistModel CurrentPlaylist
        {
            get { return currentplaylist; }
            set { 
                if (currentplaylist != null)
                {
                    currentplaylist.SetIsActive(false);
                    currentplaylist.PropertyChanged -= Currentplaylist_PropertyChanged;
                    hasSubcribed = false;
                    if (HasChanges && MessageBox.Show("Do you wish to save changes in " +
                    currentplaylist.PlaylistName + " ?",
                    currentplaylist.PlaylistName, MessageBoxButton.OKCancel)
                    == MessageBoxResult.OK)
                    {
                        this.UpdateList();
                    }
                }
                currentplaylist = value;
                if (!hasSubcribed && currentplaylist != null)
                {
                    currentplaylist.PropertyChanged += Currentplaylist_PropertyChanged;
                    haschanges = true;
                }
                RaisePropertyChanged(() => this.PlaylistName);
                HasChanges = false;
            }
        }

        //private void Currentplaylist_PropertyChanged(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        private void Currentplaylist_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(() => this.PlaylistName);
        }

        private bool issavedialogenable = false;

        public bool IsSaveDialogEnable
        {
            get { return issavedialogenable; }
            set { issavedialogenable = value; RaisePropertyChanged(() => this.IsSaveDialogEnable); }
        }
        
        ObservableCollection<VideoFolder> _playlist
            = new ObservableCollection<VideoFolder>();
        public ObservableCollection<VideoFolder> PlayList
        {
            get { return _playlist; }
            set {
                _playlist = value;
                if (value != null && CurrentPlaylist != null)
                {
                    MediaControllerVM.Current.
                        GetVideoItem((VideoFolderChild)value.First(), true);
                }
                this.RaisePropertyChanged(() => this.PlayList);
            }
        }

        private bool _canNext;
        private bool _canPrevious;
        private VideoFolderChild NowPlaying; 
        private DelegateCommand clearplaylist;
        public DelegateCommand ClearPlaylist
        {
            get
            {
                if (clearplaylist == null)
                {
                    clearplaylist = new DelegateCommand(() =>
                    {
                        Clear();
                    });
                }
               return clearplaylist;
            }
        }

        private void Clear()
        {
            if (CurrentPlaylist != null)
            {
                if (HasChanges)
                {
                    if (MessageBox.Show("Do you wish to save changes in " +
                    CurrentPlaylist.PlaylistName + " ?",
                    CurrentPlaylist.PlaylistName, MessageBoxButton.OKCancel)
                    == MessageBoxResult.OK)
                    {
                        this.UpdateList();
                    }
                }

            }
            CurrentPlaylist = null;
            foreach (var item in PlayList)
            {
                if (item == MediaControllerVM.Current.CurrentVideoItem) continue;
                PlayList.Remove(item);
            }
        }

        private DelegateCommand enablesavedialog;
        public DelegateCommand EnableSaveDialog
        {
            get
            {
                if (enablesavedialog == null)
                {
                    enablesavedialog = new DelegateCommand(() =>
                    {
                        if (CurrentPlaylist != null && HasChanges)
                        {
                            this.UpdateList();
                        }
                        else if (CurrentPlaylist == null && PlayList.Count > 0)
                        {
                            IsSaveDialogEnable = true;
                        }
                    });
                }
                return enablesavedialog;
            }
        }

        //private DelegateCommand saveplaylist;
        //public DelegateCommand SavePlaylist
        //{
        //    get
        //    {
        //        if (saveplaylist == null)
        //        {
        //            saveplaylist = new DelegateCommand(() =>
        //            {
        //                SavePlaylistAction();
                        
        //                IsSaveDialogEnable = false;
        //            },CanSavelist);
        //        }
        //        return saveplaylist;
        //    }
        //}

        //private bool CanSavelist()
        //{
        //    return TempPlaylistName != null || TempPlaylistName != string.Empty;
        //}

        public void PlayFromAList(PlaylistModel plm)
        {
            Clear();
            CurrentPlaylist = plm;
            currentplaylist.SetIsActive(true);
            Task.Factory.StartNew(() => GetObservableCollection(plm))
                           .ContinueWith(t => this.PlayList = t.Result, 
                           TaskScheduler.FromCurrentSynchronizationContext());
        }

        private ObservableCollection<VideoFolder> GetObservableCollection(PlaylistModel plm)
        {
            ObservableCollection<VideoFolder> list = new ObservableCollection<VideoFolder>();
            foreach (var item in plm.ItemsPaths)
            {
                //Dispatcher.Invoke(new Action(delegate
                //{
                DirectoryInfo directoryInfo = new DirectoryInfo(item);
                VideoFolder vfc = FileLoader.LoadChildrenFiles(directoryInfo);
                list.Add(vfc);
                //}));
            }
            return list;
        }

        //private DelegateCommand disablelistdialog;
        //public DelegateCommand DisableListDialog
        //{
        //    get
        //    {
        //        if (disablelistdialog == null)
        //        {
        //            disablelistdialog = new DelegateCommand(() =>
        //            {
        //                IsSaveDialogEnable = false;
        //            });
        //        }
        //        return disablelistdialog;
        //    }
        //}

        public PlayListManager()
        {
            if (MediaControllerVM.Current != null)
            {
                MediaControllerVM.Current.CurrentVideoItemChangedEvent += UpdateNowPlaying;
            }
        }

        public bool CanNext
        {
            get
            {
                if (MediaControllerVM.Current.RepeatMode == RepeatMode.Repeat)
                {
                    _canNext = true;
                    return _canNext;
                }
                _canNext = _playlist.Count > 1 && NowPlayingIndex + 1 != _playlist.Count ? true: false;
                
                return _canNext;
            }
        }

        public int NowPlayingIndex
        {
            get
            {
                return _playlist.IndexOf(NowPlaying);
            }
        }

        public bool CanPrevious
        {
            get
            {
                if (MediaControllerVM.Current.RepeatMode == RepeatMode.Repeat)
                {
                    _canPrevious = true;
                    return _canPrevious;
                }
                _canPrevious = (_playlist.Count > 1 && NowPlayingIndex - 1 >= 0) ? true : false;
                
                return _canPrevious;
            }
        }
        private bool haschanges = false;
        public bool HasChanges {
            get { return haschanges; }
            set { haschanges = value; RaisePropertyChanged(() => this.HasChanges); }
        }

        public VideoFolderChild GetNextItem()
        {
            int curreentpos = NowPlayingIndex;
            if (MediaControllerVM.Current.RepeatMode == RepeatMode.RepeatOnce)
            {
                return NowPlaying;
            }
            if (CanNext)
            {
                int finalpos = curreentpos + 1;
                if (MediaControllerVM.Current.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos > PlayList.Count - 1)
                    {
                        finalpos = 0;
                    }
                }
                var NowPlaying = (VideoFolderChild)PlayList[finalpos];
                return NowPlaying;
            }
            return null;
        }

        public VideoFolderChild GetPreviousItem()
        {
            int curreentpos = NowPlayingIndex;
            if (CanPrevious)
            {
                int finalpos = curreentpos - 1;
                if (MediaControllerVM.Current.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos < 0)
                    {
                        finalpos = PlayList.Count - 1;
                    }
                }
                var NowPlaying = (VideoFolderChild)PlayList[finalpos];
                return NowPlaying;
            }
            return null;
        }

        public string WhatsPreviousItem()
        {
            if (CanPrevious)
            {
                int finalpos = NowPlayingIndex - 1;
                if (MediaControllerVM.Current.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos < 0)
                    {
                        finalpos = PlayList.Count - 1;
                    }
                }
                return PlayList[finalpos].Name;
            }
            return null;
        }

        public string WhatsNextItem()
        {
            if (CanNext)
            {
                int finalpos = NowPlayingIndex + 1;
                if (MediaControllerVM.Current.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos > PlayList.Count - 1)
                    {
                        finalpos = 0;
                    }
                }
                return PlayList[finalpos].Name;
            }
            return null;
        }


        //public void PlayIndexChanged(ref VideoFolderChild obj)
        //{
        //    this.NowPlaying = obj;
        //}

        public void UpdateNowPlaying(object obj,bool frompl)
        {
            if (NowPlaying == null && !frompl || !this.PlayList.Contains(obj))
            {
                if(CurrentPlaylist != null && HasChanges) { SavePlaylistAction(); }
                CurrentPlaylist = null;

                var fileexpr = Shell.FileView.FileExplorer;
                CollectionView view = (CollectionView)CollectionViewSource.
               GetDefaultView(fileexpr.FileExplorerListView.ItemsSource);
                var viewlist = view.OfType<VideoFolderChild>();

                this.NowPlaying = (VideoFolderChild)obj;
                PlayList = new ObservableCollection<VideoFolder>();
                _playlist.AddRange(viewlist);
                
            }
            else
            {
                this.NowPlaying = (VideoFolderChild)obj;
            }
        }

        public void Add(VideoFolder vfc)
        {
            if (!this.PlayList.Contains(vfc))
            {
                this.PlayList.Add(vfc);
                HasChanges = true;
            }
        }

        public void Remove(VideoFolder vfc)
        {
            if (this.PlayList.Contains(vfc))
            {
                this.PlayList.Remove(vfc);
                HasChanges = true;
            }
            
        }
        
        public string PlaylistName
        {
            get {
                if (CurrentPlaylist == null)
                {
                    return "UnSaved Playlist";
                }
                return CurrentPlaylist.PlaylistName; }
        }
        private string tempplaylistname;
        public string TempPlaylistName
        {
            get
            {
                return tempplaylistname;
            }
            set { tempplaylistname = value; 
                RaisePropertyChanged(() => this.TempPlaylistName); }
        }

        public PlaylistModel NewCreatePlaylist()
        {
            IPlaylistModel ipl = new PlaylistModel
            {
                PlaylistName = this.TempPlaylistName
            };
            foreach (var item in PlayList)
            {
                ipl.ItemsPaths.Add(item.Directory.FullName);
            }
            TempPlaylistName = null;
            return ipl as PlaylistModel;
        }

        public void UpdateList()
        {
            if(currentplaylist == null) return;
            currentplaylist.ItemsPaths.Clear();
            foreach (var item in PlayList)
            {
                currentplaylist.ItemsPaths.Add(item.Directory.FullName);
            }
            HasChanges = false;
        }

        public void SavePlaylistAction()
        {
            if (CurrentPlaylist == null)
            {
                CurrentPlaylist = NewCreatePlaylist();
                ((Shell.FileView.TreeViewer as ITreeViewer).
                    MoviesPLaylist as PlaylistTree).AddToPlayList(CurrentPlaylist);
            }
            else
            {
                UpdateList();
            }
            RaisePropertyChanged(() => this.PlaylistName);
        }

        private IShell Shell
        {
            get {
                return ServiceLocator.Current.GetInstance<IShell>(); }
        }
    }
}
