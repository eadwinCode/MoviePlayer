using Common.Interfaces;
using Common.Model;
using Common.Util;
using Delimon.Win32.IO;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VideoComponent.BaseClass;
using VideoPlayerControl.ViewModel;
using VirtualizingListView.Pages.Util;
using VirtualizingListView.Util;
using VirtualizingListView.View;

namespace VideoPlayerControl.PlayList
{
    public class PlayListManager: NotificationObject,IHasChanges
    {
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
        public ObservableCollection<VideoFolder> PlayListCollection
        {
            get { return _playlist; }
            set {
                _playlist = value;
                if (value != null && CurrentPlaylist != null)
                {
                    MediaControllerVM.MediaControllerInstance.
                        GetVideoItem((VideoFolderChild)value.First(), true);
                }
                this.RaisePropertyChanged(() => this.PlayListCollection);
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
            PlayListCollection.Clear();
            //foreach (var item in PlayList)
            //{
            //    if (item == MediaControllerVM.MediaControllerInstance.CurrentVideoItem) continue;
            //    PlayList.Remove(item);
            //}
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
                        else if (CurrentPlaylist == null && PlayListCollection.Count > 0)
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
            FileLoaderCompletion fileLoaderCompletion = new FileLoaderCompletion();
            Task.Factory.StartNew(() =>
            {
                var list = GetObservableCollection(plm);
                fileLoaderCompletion.FinishCollectionLoadProcess(list);
                //list = FileLoader.FileLoaderInstance.SortList(SortType.Name, list);
                return list;
            }).ContinueWith(t =>this.PlayListCollection = t.Result,TaskScheduler.FromCurrentSynchronizationContext());
        }

        private ObservableCollection<VideoFolder> GetObservableCollection(PlaylistModel plm)
        {
            var padlock = new object();
            ObservableCollection<VideoFolder> list = new ObservableCollection<VideoFolder>();
            List<Task> Tasks = new List<Task>();
            foreach (var s in plm.ItemsPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(s);
                var task = Task.Factory.StartNew(() =>
                   FileLoader.FileLoaderInstance.LoadChildrenFiles(directoryInfo)
                ).ContinueWith(t =>
                {
                    lock (padlock)
                    {
                        if (t.Result != null)
                            list.Add(t.Result);
                    }
                  
                }, TaskScheduler.Current);
                Tasks.Add(task);
            }
            Task.WaitAll(Tasks.ToArray());
           
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
            
        }

        public void PlaylistViewLoaded()
        {
            if (MediaControllerVM.MediaControllerInstance != null)
            {
                MediaControllerVM.MediaControllerInstance.CurrentVideoItemChangedEvent += UpdateNowPlaying;
            }
        }

        public bool CanNext
        {
            get
            {
                if (MediaControllerVM.MediaControllerInstance.RepeatMode == RepeatMode.Repeat)
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
                if (MediaControllerVM.MediaControllerInstance.RepeatMode == RepeatMode.Repeat)
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
            if (MediaControllerVM.MediaControllerInstance.RepeatMode == RepeatMode.RepeatOnce)
            {
                return NowPlaying;
            }
            if (CanNext)
            {
                int finalpos = curreentpos + 1;
                if (MediaControllerVM.MediaControllerInstance.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos > PlayListCollection.Count - 1)
                    {
                        finalpos = 0;
                    }
                }
                var NowPlaying = (VideoFolderChild)PlayListCollection[finalpos];
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
                if (MediaControllerVM.MediaControllerInstance.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos < 0)
                    {
                        finalpos = PlayListCollection.Count - 1;
                    }
                }
                var NowPlaying = (VideoFolderChild)PlayListCollection[finalpos];
                return NowPlaying;
            }
            return null;
        }

        public string WhatsPreviousItem()
        {
            if (CanPrevious)
            {
                int finalpos = NowPlayingIndex - 1;
                if (MediaControllerVM.MediaControllerInstance.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos < 0)
                    {
                        finalpos = PlayListCollection.Count - 1;
                    }
                }
                return (PlayListCollection[finalpos]as VideoFolderChild).MediaTitle;
            }
            return null;
        }

        public string WhatsNextItem()
        {
            if (CanNext)
            {
                int finalpos = NowPlayingIndex + 1;
                if (MediaControllerVM.MediaControllerInstance.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos > PlayListCollection.Count - 1)
                    {
                        finalpos = 0;
                    }
                }
                return (PlayListCollection[finalpos] as VideoFolderChild).MediaTitle;
            }
            return null;
        }


        //public void PlayIndexChanged(ref VideoFolderChild obj)
        //{
        //    this.NowPlaying = obj;
        //}

        public void UpdateNowPlaying(object obj,bool frompl)
        {
            if (NowPlaying == null && !frompl || !this.PlayListCollection.Contains(obj))
            {
                if(CurrentPlaylist != null && HasChanges) { SavePlaylistAction(); }
                CurrentPlaylist = null;
                this.NowPlaying = (VideoFolderChild)obj;
                VideoFolder parent = (VideoFolder)NowPlaying.ParentDirectory;
                PlayListCollection = new ObservableCollection<VideoFolder>();
                _playlist.AddRange(parent.OtherFiles.Where(x => x.FileType == FileType.File).ToList());
                
            }
            else
            {
                this.NowPlaying = (VideoFolderChild)obj;
            }
        }

        public void Add(VideoFolder vfc)
        {
            if (!this.PlayListCollection.Contains(vfc))
            {
                this.PlayListCollection.Add(vfc);
                HasChanges = true;
            }
        }

        public void Remove(VideoFolder vfc)
        {
            if (this.PlayListCollection.Contains(vfc))
            {
                this.PlayListCollection.Remove(vfc);
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
            foreach (var item in PlayListCollection)
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
            foreach (var item in PlayListCollection)
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
                HomePlaylistView.AddToPlaylistCollection.Invoke(CurrentPlaylist as IPlaylistModel);
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
