using Delimon.Win32.IO;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using MovieHub.MediaPlayerElement.Views;
using Movies.Enums;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovieHub.MediaPlayerElement
{
    public class MoviesPlaylistManager : Control, IPlaylistManager
    {
        private static RoutedCommand clearplaylist;
        private static RoutedCommand enablesavedialog;
        private static RoutedCommand extsortcommand;
        private static RoutedCommand datesortcommand;
        private static RoutedCommand closeplaylistdialog;
        private static RoutedCommand nameCommand;
        private static RoutedCommand removeItemCommand;
        private static RoutedCommand playitemcommand;

        internal MediaPlayerElement mediaPlayer;
        private  IPlayable _nowplaying;
        private bool _canNext;
        private bool _canPrevious;
        private int NowPlayingIndex
        {
            get
            {
                return PlayListCollection.IndexOf(NowPlaying);
            }
        }
        internal IPlayable NowPlaying
        {
            get
            {
                if (_nowplaying == null)
                    _nowplaying = PlayListCollection.FirstOrDefault(x => x.IsActive);
                return _nowplaying;
            }
            set
            {
                _nowplaying = value;
            }
        }

        #region Services

        IEventManager IEventManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventManager>();
            }
        }

        IBackgroundService BackgroundService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IBackgroundService>();
            }
        }

        IDispatcherService DispatcherService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDispatcherService>();
            }
        }

        IFileLoader FileLoader
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoader>();
            }
        }

        ISortService SortService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ISortService>();
            }
        }
        
        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }

        #endregion

        #region Propdp

        // Using a DependencyProperty as the backing store for FileexpVisiblityProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowPlaylistViewPropertyProperty = 
            DependencyProperty.RegisterAttached("ShowPlaylistView", typeof(Visibility),typeof(MoviesPlaylistManager),new FrameworkPropertyMetadata(Visibility.Collapsed)
        { DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged });

        public static void SetShowPlaylistView(UIElement element, Visibility value)
        {
            element.SetValue(ShowPlaylistViewPropertyProperty, value);
        }
        public static Visibility GetShowPlaylistView(UIElement element)
        {
            return (Visibility)element.GetValue(ShowPlaylistViewPropertyProperty);
        }

        public bool HasChanges
        {
            get { return (bool)GetValue(HasChangesProperty); }
            private set { SetValue(HasChangesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasChanges.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasChangesProperty =
            DependencyProperty.Register("HasChanges", typeof(bool), typeof(MoviesPlaylistManager), new PropertyMetadata(false));

        public bool IsLoadingPlaylist
        {
            get { return (bool)GetValue(IsLoadingPlaylistProperty); }
            internal set { SetValue(IsLoadingPlaylistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoadingPlaylist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingPlaylistProperty =
            DependencyProperty.Register("IsLoadingPlaylist", typeof(bool), typeof(MoviesPlaylistManager), new PropertyMetadata(false));
        
        public string SortedType
        {
            get { return (string)GetValue(SortedTypeProperty); }
            internal set { SetValue(SortedTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortedType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortedTypeProperty =
            DependencyProperty.Register("SortedType", typeof(string), typeof(MoviesPlaylistManager), new PropertyMetadata("-Custom-"));
        
        public string PlayListName
        {
            get { return (string)GetValue(PlayListNameProperty); }
            internal set { SetValue(PlayListNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayListName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayListNameProperty =
            DependencyProperty.Register("PlayListName", typeof(string), typeof(MoviesPlaylistManager), new PropertyMetadata("UnSaved Playlist"));

        public ObservableCollection<IPlayable> PlayListCollection
        {
            get { return (ObservableCollection<IPlayable>)GetValue(PlayListCollectionProperty); }
            internal set { SetValue(PlayListCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayListCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayListCollectionProperty =
            DependencyProperty.Register("PlayListCollection", typeof(ObservableCollection<IPlayable>), typeof(MoviesPlaylistManager), new PropertyMetadata(new ObservableCollection<IPlayable>()));


        public IPlaylistModel CurrentPlaylist
        {
            get { return (IPlaylistModel)GetValue(CurrentPlaylistProperty); }
            internal set { SetValue(CurrentPlaylistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPlaylist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPlaylistProperty =
            DependencyProperty.Register("CurrentPlaylist", typeof(IPlaylistModel), typeof(MoviesPlaylistManager), new PropertyMetadata(null, OnCurrentPlaylistChanged));

        private static void OnCurrentPlaylistChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = d as MoviesPlaylistManager;
            if(moviesPlaylistManager != null)
            {
                var oldPlaylist = e.OldValue as IPlaylistModel;
                if (oldPlaylist != null)
                    oldPlaylist.SetIsActive(false);
                if(e.NewValue != null)  
                    moviesPlaylistManager.PlayListName = moviesPlaylistManager.CurrentPlaylist.PlaylistName;
            }
        }

        #endregion

        static MoviesPlaylistManager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MoviesPlaylistManager), new FrameworkPropertyMetadata(typeof(MoviesPlaylistManager)));
            InitializeCommands();
            RegisterControlCommands();
        }
        
        #region CommandProperties

        public static RoutedCommand ClearPlaylist
        {
            get
            {
                return closeplaylistdialog;
            }
        }

        public static RoutedCommand RemoveItemCommand
        {
            get
            {
                return removeItemCommand;
            }
        }

        public static RoutedCommand PlayItemCommand
        {
            get
            {
                return playitemcommand;
            }
        }

        public static RoutedCommand ClosePlaylistDialog
        {
            get
            {
                return clearplaylist;
            }
        }

        public static RoutedCommand EnableSaveDialog
        {
            get
            {
                return enablesavedialog;
            }
        }

        public static RoutedCommand NameSortCommand
        {
            get
            {
                return nameCommand;
            }
        }

        public static RoutedCommand DateSortCommand
        {
            get
            {
                return datesortcommand;
            }
        }

        public static RoutedCommand ExtSortCommand
        {
            get
            {
                return extsortcommand;
            }
        }

        private static void InitializeCommands()
        {
            clearplaylist = new RoutedCommand("ClearPlaylist", typeof(MoviesPlaylistManager));
            enablesavedialog = new RoutedCommand("EnableSaveDialog", typeof(MoviesPlaylistManager));
            extsortcommand = new RoutedCommand("ExtSortCommand", typeof(MoviesPlaylistManager));
            datesortcommand = new RoutedCommand("DateSortCommand", typeof(MoviesPlaylistManager));
            closeplaylistdialog = new RoutedCommand("ClosePlaylistDialog", typeof(MoviesPlaylistManager));
            nameCommand = new RoutedCommand("NameSortCommand", typeof(MoviesPlaylistManager));
            removeItemCommand = new RoutedCommand("RemoveItemCommand", typeof(MoviesPlaylistManager));
            playitemcommand = new RoutedCommand("PlayItemCommand", typeof(MoviesPlaylistManager));
        }

        private static void RegisterCommandBings(Type type, ICommand command, CommandBinding commandBinding, params InputGesture[] inputBinding)
        {
            CommandManager.RegisterClassCommandBinding(type, commandBinding);
            if (inputBinding.Length > 0)
            {
                foreach (var ipbing in inputBinding)
                {
                    CommandManager.RegisterClassInputBinding(type, new InputBinding(command, ipbing));
                }
            }

        }

        private static void RegisterControlCommands()
        {
            RegisterCommandBings(typeof(MoviesPlaylistManager), ClearPlaylist, new CommandBinding(ClearPlaylist, ClearPlaylist_Executed));
            RegisterCommandBings(typeof(MoviesPlaylistManager), EnableSaveDialog, new CommandBinding(EnableSaveDialog, EnableSaveDialog_Executed));
            RegisterCommandBings(typeof(MoviesPlaylistManager), NameSortCommand, new CommandBinding(NameSortCommand, NameSortCommand_Executed));
            RegisterCommandBings(typeof(MoviesPlaylistManager), DateSortCommand, new CommandBinding(DateSortCommand, DateSortCommand_Executed));
            RegisterCommandBings(typeof(MoviesPlaylistManager), ExtSortCommand, new CommandBinding(ExtSortCommand, ExtSortCommand_Executed));

            RegisterCommandBings(typeof(MoviesPlaylistManager), PlayItemCommand, new CommandBinding(PlayItemCommand, PlayItemCommand_Executed));
            RegisterCommandBings(typeof(MoviesPlaylistManager), RemoveItemCommand, new CommandBinding(RemoveItemCommand, RemoveItemCommand_Executed));
            RegisterCommandBings(typeof(MoviesPlaylistManager), ClosePlaylistDialog, new CommandBinding(ClosePlaylistDialog, ClosePlaylistDialog_Executed));

        }

        private static void ClosePlaylistDialog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if (moviesPlaylistManager != null)
            {
                moviesPlaylistManager.mediaPlayer.TogglePlaylistView();
            }
        }

        private static void RemoveItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if (moviesPlaylistManager != null)
            {
                moviesPlaylistManager.Remove(e.Parameter as IPlayable);
            }
        }

        private static void PlayItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if (moviesPlaylistManager != null)
            {
                moviesPlaylistManager.PlayItemFromList(e.Parameter as IPlayable);
            }
        }

        private void PlayItemFromList(IPlayable playable)
        {
            mediaPlayer.SourceFromPlaylist(playable);
        }

        private static void ClearPlaylist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if (moviesPlaylistManager != null)
            {
                moviesPlaylistManager.Clear();
            }
        }

        private static void EnableSaveDialog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if (moviesPlaylistManager != null)
            {
                moviesPlaylistManager.ShowSaveDialogAction();
            }
        }

        private static void NameSortCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if (moviesPlaylistManager != null)
            {
                moviesPlaylistManager.SortFunction(SortType.Name);
            }
        }

        private static void DateSortCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if (moviesPlaylistManager != null)
            {
                moviesPlaylistManager.SortFunction(SortType.Date);
            }
        }

        private static void ExtSortCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if (moviesPlaylistManager != null)
            {
                moviesPlaylistManager.SortFunction(SortType.Extension);
            }
        }
        #endregion
        
        private void MoviesPlaylistManager_Unloaded(object sender, RoutedEventArgs e)
        {
            if (CurrentPlaylist != null && mediaPlayer.AllowMediaPlayerAutoDispose)
                CurrentPlaylist.SetIsActive(false);
        }

        private void CurrentlyStreamingChangedEventDelegate(IPlayable playable)
        {
            NowPlaying = playable;
        }
        
        private void ShowSaveDialogAction()
        {
            if (CurrentPlaylist != null && HasChanges)
            {
                this.UpdateList();
            }
            else if (CurrentPlaylist == null && PlayListCollection.Count > 0)
            {
                SavePlaylistDialog savePlaylistDialog = new SavePlaylistDialog(this) { DataContext = this };
                savePlaylistDialog.ShowDialog();
                savePlaylistDialog.OnDialogFinished += (s, e) =>
                {
                    SavePlaylistDialog savePlaylistdialog = s as SavePlaylistDialog;
                    if(savePlaylistdialog != null)
                        SavePlaylistAction(savePlaylistdialog.PlaylistName);

                    savePlaylistDialog = null;
                };
            }
        }

        private void UpdateList()
        {
            if (CurrentPlaylist == null) return;
            (CurrentPlaylist as PlaylistModel).Clear();
            foreach (var item in PlayListCollection)
            {
                (CurrentPlaylist as PlaylistModel).Add(Pathlist.Parse(item));
            }
            HasChanges = false;
        }

        private void Clear()
        {
            if (CurrentPlaylist != null)
            {
                if (HasChanges)
                {
                    if (MessageBox.Show("Do you wish to save changes in " + CurrentPlaylist.PlaylistName + " ?", CurrentPlaylist.PlaylistName, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        this.UpdateList();
                    }
                }
            }
            CurrentPlaylist = null;
            PlayListCollection.Clear();
        }

        private void InitFinishCollection()
        {
            DispatcherService.ExecuteTimerAction(() => {
                BackgroundService.Execute(() => {
                    //   LoaderCompletion.FinishCollectionLoadProcess(PlayListCollection, null);
                }, "Getting playlist metadata...", () => { });
            }, 5000);
        }

        private ObservableCollection<IPlayable> GetObservableCollection(IPlaylistModel plm)
        {
            var padlock = new object();
            ObservableCollection<IPlayable> list = new ObservableCollection<IPlayable>();
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

        private void SortFunction(SortType sortType)
        {
            if (IsLoadingPlaylist) return;

            SortedType = String.Format("{0}", sortType.ToString());
            var collection = PlayListCollection.OfType<IItemSort>();
            Task.Factory.StartNew(() => SortService.SortList(sortType, collection)).
                ContinueWith(t =>
                {
                    PlayListCollection = new ObservableCollection<IPlayable>(t.Result.OfType<IPlayable>());
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        
        private void AdjustHeight()
        {
            var playerHeight = mediaPlayer.ActualHeight;
            if(playerHeight <= 400)
            {
                this.MinHeight = this.Height = mediaPlayer.ActualHeight / 2;
                return;
            }
            this.Height = (mediaPlayer.ActualHeight / 3) * 2;
            
        }

        internal MoviesPlaylistManager(MediaPlayerElement mediaPlayerElement)
        {
            this.mediaPlayer = mediaPlayerElement;
            mediaPlayer.CurrentlyStreamingChangedEvent += CurrentlyStreamingChangedEventDelegate;
            this.Unloaded += MoviesPlaylistManager_Unloaded;
            mediaPlayer.SizeChanged += MediaPlayer_SizeChanged;
            this.Height = 0;
        }

        private void MediaPlayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(mediaPlayer.IsLoaded && GetShowPlaylistView(this) == Visibility.Visible)
                AdjustHeight();
        }

        internal void Add(IPlayable vfc)
        {
            if (!this.PlayListCollection.Contains(vfc))
            {
                this.PlayListCollection.Add(vfc);
            }
            if (CurrentPlaylist != null)
                HasChanges = true;
        }

        internal void AddRange(IEnumerable<IPlayable> EnumerableVfc)
        {
            this.PlayListCollection.AddRange(EnumerableVfc);
            if (CurrentPlaylist != null)
                HasChanges = true;
        }

        internal bool TogglePlayVisibility()
        {
            if (GetShowPlaylistView(this) == Visibility.Collapsed)
            {
                AdjustHeight();
                SetShowPlaylistView(this, Visibility.Visible);
                return true;
            }
            else
                SetShowPlaylistView(this, Visibility.Collapsed);
            mediaPlayer.Focus();
            return false;
        }

        internal void PlayFromAList(IPlaylistModel plm)
        {
            Clear();
            CurrentPlaylist = plm;
            IsLoadingPlaylist = true;
            CurrentPlaylist.SetIsActive(true);
            DispatcherService.ExecuteTimerAction(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    return GetObservableCollection(plm);
                    //list = FileLoader.FileLoaderInstance.SortList(SortType.Name, list);
                }).ContinueWith(t =>
                {
                    this.PlayListCollection = t.Result;
                    IsLoadingPlaylist = false;
                    //Play First item
                    NowPlaying = PlayListCollection[0];
                    mediaPlayer.SourceFromPlaylist(NowPlaying);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }, 2000);
        }

        internal void PlayFromTemperalList(IEnumerable<IPlayable> TemperalList)
        {
            Clear();
            if (CurrentPlaylist != null)
            {
                CurrentPlaylist.SetIsActive(false);
                CurrentPlaylist = null;
            }
            PlayListCollection = new ObservableCollection<IPlayable>(TemperalList);
        }
        
        public bool CanNext
        {
            get
            {
                if (mediaPlayer.MovieControl.RepeatMode == RepeatMode.Repeat)
                {
                    _canNext = true;
                    return _canNext;
                }
                _canNext = PlayListCollection.Count > 1 && NowPlayingIndex + 1 != PlayListCollection.Count ? true : false;

                return _canNext;
            }
        }

        public bool CanPrevious
        {
            get
            {
                if (mediaPlayer.MovieControl.RepeatMode == RepeatMode.Repeat)
                {
                    _canPrevious = true;
                    return _canPrevious;
                }
                _canPrevious = (PlayListCollection.Count > 1 && NowPlayingIndex - 1 >= 0) ? true : false;

                return _canPrevious;
            }
        }
        
        public IPlayable GetNextItem()
        {
            int curreentpos = NowPlayingIndex;
            if (mediaPlayer.MovieControl.RepeatMode == RepeatMode.RepeatOnce)
            {
                return NowPlaying;
            }
            if (CanNext)
            {
                int finalpos = curreentpos + 1;
                if (mediaPlayer.MovieControl.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos > PlayListCollection.Count - 1)
                    {
                        finalpos = 0;
                    }
                }
                return PlayListCollection[finalpos];
            }
            return null;
        }

        public IPlayable GetPreviousItem()
        {
            int curreentpos = NowPlayingIndex;
            if (CanPrevious)
            {
                int finalpos = curreentpos - 1;
                if (mediaPlayer.MovieControl.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos < 0)
                    {
                        finalpos = PlayListCollection.Count - 1;
                    }
                } 
                return PlayListCollection[finalpos];
            }
            return null;
        }

        public IPlaylistModel NewCreatePlaylist(string playlistName)
        {
            PlaylistModel ipl = new PlaylistModel
            {
                PlaylistName = playlistName
            };
            foreach (var item in PlayListCollection)
            {
                ipl.Add(Pathlist.Parse(item));
            }
            return ipl as PlaylistModel;
        }
        
        public void Remove(IPlayable vfc)
        {
            if (this.PlayListCollection.Contains(vfc))
            {
                this.PlayListCollection.Remove(vfc);
            }
            if(CurrentPlaylist != null)
                HasChanges = true;
        }

        public void SavePlaylistAction(string playlisName = null)
        {
            if (CurrentPlaylist == null)
            {
                CurrentPlaylist = NewCreatePlaylist(playlisName);
                IEventManager.GetEvent<PlaylistCollectionChangedEventToken>().Publish(CurrentPlaylist);
            }
            else
            {
                UpdateList();
            }
        }
        
        public string WhatsNextItem()
        {
            if (CanNext)
            {
                int finalpos = NowPlayingIndex + 1;
                if (mediaPlayer.MovieControl.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos > PlayListCollection.Count - 1)
                    {
                        finalpos = 0;
                    }
                }
               return (PlayListCollection[finalpos]).MediaTitle;
            }
            return null;
        }

        public string WhatsPreviousItem()
        {
            if (CanPrevious)
            {
                int finalpos = NowPlayingIndex - 1;
                if (mediaPlayer.MovieControl.RepeatMode == RepeatMode.Repeat)
                {
                    if (finalpos < 0)
                    {
                        finalpos = PlayListCollection.Count - 1;
                    }
                }
                return (PlayListCollection[finalpos]).MediaTitle;
            }
            return null;
        }
        
    }
}
