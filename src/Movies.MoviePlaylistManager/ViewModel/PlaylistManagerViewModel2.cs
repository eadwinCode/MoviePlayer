using Delimon.Win32.IO;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Movies.MoviePlaylistManager.Views;
using PresentationExtension.InterFaces;
using PresentationExtension.CommonEvent;
using System.Windows.Threading;

namespace Movies.MoviePlaylistManager.ViewModel
{
    public partial class PlaylistManager : NotificationObject, IHasChanges, IPlaylistManager
    {
        private ObservableCollection<VideoFolder> GetObservableCollection(PlaylistModel plm)
        {
            var padlock = new object();
            ObservableCollection<VideoFolder> list = new ObservableCollection<VideoFolder>();
            List<Task> Tasks = new List<Task>();
            foreach (var s in plm.GetEnumerator)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(s.FilePath);
                var task = Task.Factory.StartNew(() =>
                   FileLoader.LoadChildrenFiles(directoryInfo)
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

        private bool CanOkAction()
        {
            return TempPlaylistName != string.Empty;
        }

        public IPlayable GetNextItem()
        {
            int curreentpos = NowPlayingIndex;
            if (MediaControllerViewModel.RepeatMode == RepeatMode.RepeatOnce)
            {
                return NowPlaying;
            }
            if (CanNext)
            {
                int finalpos = curreentpos + 1;
                if (MediaControllerViewModel.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos > PlayListCollection.Count - 1)
                    {
                        finalpos = 0;
                    }
                }
                var NowPlaying = PlayListCollection[finalpos];
                return NowPlaying;
            }
            return null;
        }

        public IPlayable GetPreviousItem()
        {
            int curreentpos = NowPlayingIndex;
            if (CanPrevious)
            {
                int finalpos = curreentpos - 1;
                if (MediaControllerViewModel.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos < 0)
                    {
                        finalpos = PlayListCollection.Count - 1;
                    }
                }
                var NowPlaying = PlayListCollection[finalpos];
                return NowPlaying;
            }
            return null;
        }

        public PlaylistModel NewCreatePlaylist()
        {
            PlaylistModel ipl = new PlaylistModel
            {
                PlaylistName = this.TempPlaylistName
            };
            foreach (var item in PlayListCollection)
            {
                //ipl.Add(item.Directory.FullName);
            }
            TempPlaylistName = null;
            return ipl as PlaylistModel;
        }

        public object GetPlaylistView(IMediaControllerViewModel mediaControllerViewModel)
        {
            return new PlaylistView(mediaControllerViewModel, this);
        }

        public string WhatsPreviousItem()
        {
            if (CanPrevious)
            {
                int finalpos = NowPlayingIndex - 1;
                if (MediaControllerViewModel.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos < 0)
                    {
                        finalpos = PlayListCollection.Count - 1;
                    }
                }
                return (PlayListCollection[finalpos] as VideoFolderChild).MediaTitle;
            }
            return null;
        }

        public string WhatsNextItem()
        {
            if (CanNext)
            {
                int finalpos = NowPlayingIndex + 1;
                if (MediaControllerViewModel.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos > PlayListCollection.Count - 1)
                    {
                        finalpos = 0;
                    }
                }
              //  return (PlayListCollection[finalpos]).MediaTitle;
            }
            return null;
        }

        public PlaylistManager()
        {
            this.SortedName = "--No Sort--";
            //WindowsBindingCollection.Add(new CommandBinding(VideoPlayerCommands.PlayList,
            //    PlayList_executed));

            //WindowsBindingCollection.Add(new CommandBinding(VideoPlayerCommands.RemovefromPlayList,
            //   RemovefromPlayList_executed));
            //WindowsBindingCollection.Add(new CommandBinding(VideoPlayerCommands.Play, Play_executed));

        }

        public void PlaylistViewLoaded()
        {
            if (MediaControllerViewModel != null)
            {
                MediaControllerViewModel.CurrentVideoItemChangedEvent += UpdateNowPlaying;
            }
        }
        
        private void SortFunction(SortType sortType)
        {
            if (IsLoading) return;

            SortedName = sortType.ToString();
            //Task.Factory.StartNew(() => FileLoader.SortList(sortType, _playlist)).ContinueWith(t => {
            //    _playlist = t.Result;
            //    RaisePropertyChanged(() => this.PlayListCollection);
            //}, TaskScheduler.FromCurrentSynchronizationContext());
        }
        
        private void Currentplaylist_PropertyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(() => this.PlaylistName);
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

        public void UpdateNowPlaying(object obj, bool frompl)
        {
            //if (NowPlaying == null && !frompl || !this.PlayListCollection.Contains(obj))
            //{
            //    if (CurrentPlaylist != null && HasChanges) { SavePlaylistAction(); }
            //    CurrentPlaylist = null;
            //    this.NowPlaying = (VideoFolderChild)obj;
            //    VideoFolder parent = (VideoFolder)NowPlaying.ParentDirectory;
            //    PlayListCollection = new ObservableCollection<VideoFolder>();
            //    _playlist.AddRange(parent.OtherFiles.Where(x => x.FileType == FileType.File).ToList());

            //}
            //else
            //{
            //    this.NowPlaying = (VideoFolderChild)obj;
            //}
        }

        public void Add(VideoFolder vfc)
        {
            //if (!this.PlayListCollection.Contains(vfc))
            //{
            //    this.PlayListCollection.Add(vfc);
            //    HasChanges = true;
            //}
        }

        public void Add(IEnumerable<VideoFolder> EnumerableVfc)
        {
            IsLoading = true;
            //this.PlayListCollection.AddRange(EnumerableVfc);
            InitFinishCollection();
        }

        public void Remove(VideoFolder vfc)
        {
            //if (this.PlayListCollection.Contains(vfc))
            //{
            //    this.PlayListCollection.Remove(vfc);
            //    HasChanges = true;
            //}
        }
        
        public void UpdateList()
        {
            if (currentplaylist == null) return;
            currentplaylist.Clear();
            //foreach (var item in PlayListCollection)
            //{
            //    currentplaylist.Add(item.Directory.FullName);
            //}
            HasChanges = false;
        }

        public void SavePlaylistAction()
        {
            if (CurrentPlaylist == null)
            {
                CurrentPlaylist = NewCreatePlaylist();
                IEventManager.GetEvent<PlaylistCollectionChangedEventToken>().Publish(CurrentPlaylist);
            }
            else
            {
                UpdateList();
            }
            RaisePropertyChanged(() => this.PlaylistName);
        }
        
        public void PlayFromTemperalList(IVideoData playFile, IEnumerable<VideoFolderChild> TemperalList)
        {
            Clear();
            if (currentplaylist != null)
            {
                currentplaylist.SetIsActive(true);
                CurrentPlaylist = null;
            }
            //PlayListCollection = new ObservableCollection<VideoFolder>(TemperalList);
            //MediaControllerViewModel.GetVideoItem((VideoFolderChild)playFile, true);
        }

        public void PlayFromAList(PlaylistModel plm)
        {
            //Clear();
            //CurrentPlaylist = plm;
            //IsLoading = true;
            //currentplaylist.SetIsActive(true);
            //DispatcherService.ExecuteTimerAction(() => {
            //    Task.Factory.StartNew(() =>
            //    {
            //        return GetObservableCollection(plm);
            //        //list = FileLoader.FileLoaderInstance.SortList(SortType.Name, list);
            //    }).ContinueWith(t => {
            //        this.PlayListCollection = t.Result;
            //        IsLoading = false;
            //    }, TaskScheduler.FromCurrentSynchronizationContext());
            //}, 2000);

        }

        private void InitFinishCollection()
        {
            DispatcherService.ExecuteTimerAction(() => {
                BackgroundService.Execute(() => {
                   // LoaderCompletion.FinishCollectionLoadProcess(PlayListCollection, null);
                }, "Getting playlist metadata...", () => { });
            }, 5000);
        }
    }
}
