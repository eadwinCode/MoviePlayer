using Delimon.Win32.IO;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Enums;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviePlaylistManager.Views;
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

namespace Movies.MoviePlaylistManager
{
    public class MoviesPlaylistManager : Control, IPlaylistManager
    {
        private static RoutedCommand clearplaylist;
        private static RoutedCommand enablesavedialog;
        private static RoutedCommand cancelcommand;
        private static RoutedCommand okcommand;
        private static RoutedCommand extsortcommand;
        private static RoutedCommand datesortcommand;
        private static RoutedCommand closeplaylistdialog;
        private static RoutedCommand nameCommand; 

        private IPlaylistModel currentplaylist;
        private bool issavedialogenable;
        private ObservableCollection<IPlayable> playlistcollection;
        private int NowPlayingIndex;
        private RepeatMode MoviesRepeatMode;
        private IPlayable NowPlaying;
        private bool _canNext;
        private bool _canPrevious;
        private string sortedname;

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

        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }
        #endregion

        #region Propdp
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



        public SortType SortedType
        {
            get { return (SortType)GetValue(SortedTypeProperty); }
            internal set { SetValue(SortedTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortedType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortedTypeProperty =
            DependencyProperty.Register("SortedType", typeof(SortType), typeof(MoviesPlaylistManager), new PropertyMetadata(SortType.NoSort));



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
            DependencyProperty.Register("PlayListCollection", typeof(ObservableCollection<IPlayable>), typeof(MoviesPlaylistManager), new PropertyMetadata(null));


        public IPlaylistModel CurrentPlaylist
        {
            get { return (IPlaylistModel)GetValue(CurrentPlaylistProperty); }
            set { SetValue(CurrentPlaylistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPlaylist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPlaylistProperty =
            DependencyProperty.Register("CurrentPlaylist", typeof(IPlaylistModel), typeof(MoviesPlaylistManager), new PropertyMetadata(null));

        #endregion

        #region CommandProperties
        public static RoutedCommand ClearPlaylist
        {
            get
            {
                return closeplaylistdialog;
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

        public static RoutedCommand OkCommand
        {
            get
            {
                return okcommand;
            }
        }

        public static RoutedCommand CancelCommand
        {
            get
            {
                return cancelcommand;
            }
        } 
        #endregion

        static MoviesPlaylistManager()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MoviesPlaylistManager),new FrameworkPropertyMetadata(typeof(MoviesPlaylistManager)));
            InitializeCommands();
            RegisterControlCommands();
        }

        private static void InitializeCommands()
        {
            clearplaylist = new RoutedCommand("ClearPlaylist", typeof(MoviesPlaylistManager));
            enablesavedialog = new RoutedCommand("EnableSaveDialog", typeof(MoviesPlaylistManager));
            cancelcommand = new RoutedCommand("CancelCommand", typeof(MoviesPlaylistManager));
            okcommand = new RoutedCommand("OkCommand", typeof(MoviesPlaylistManager));
            extsortcommand = new RoutedCommand("ExtSortCommand", typeof(MoviesPlaylistManager));
            datesortcommand = new RoutedCommand("DateSortCommand", typeof(MoviesPlaylistManager));
            closeplaylistdialog = new RoutedCommand("ClosePlaylistDialog", typeof(MoviesPlaylistManager));
            nameCommand = new RoutedCommand("NameSortCommand", typeof(MoviesPlaylistManager));
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
            RegisterCommandBings(typeof(MoviesPlaylistManager), CancelCommand, new CommandBinding(CancelCommand, CancelCommand_Executed));
            RegisterCommandBings(typeof(MoviesPlaylistManager), OkCommand, new CommandBinding(OkCommand, OkCommand_Executed, OkCommand_Enabled));
        }

        private static void ClearPlaylist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if(moviesPlaylistManager != null)
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

        private static void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if (moviesPlaylistManager != null)
            {
                moviesPlaylistManager.CancelAction();
            }
        }
        private void CancelAction()
        {
            TempPlaylistName = string.Empty;
            //MediaControllerViewModel.IVideoElement.ContentDockRegion.Content = null;
            //(MediaControllerViewModel.IVideoElement.MediaController as UserControl).Focus();
        }

        private static void OkCommand_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OkCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MoviesPlaylistManager moviesPlaylistManager = sender as MoviesPlaylistManager;
            if (moviesPlaylistManager != null)
            {
                moviesPlaylistManager.OkCommandAction();
            }
        }
        private void OkCommandAction()
        {
            SavePlaylistAction();
            //MediaControllerViewModel.IVideoElement.ContentDockRegion.Content = null;
            //(MediaControllerViewModel.IVideoElement.MediaController as UserControl).Focus();
        }

        private void ShowSaveDialogAction()
        {
            if (CurrentPlaylist != null && HasChanges)
            {
                this.UpdateList();
            }
            else if (CurrentPlaylist == null && PlayListCollection.Count > 0)
            {
                SavePlaylistDialog savePlaylistDialog = new SavePlaylistDialog() { DataContext = this };
                savePlaylistDialog.ShowDialog();
            }
        }

        public bool CanNext
        {
            get
            {
                if (MoviesRepeatMode == RepeatMode.Repeat)
                {
                    _canNext = true;
                    return _canNext;
                }
                _canNext = playlistcollection.Count > 1 && NowPlayingIndex + 1 != playlistcollection.Count ? true : false;

                return _canNext;
            }
        }

        public bool CanPrevious
        {
            get
            {
                if (MoviesRepeatMode == RepeatMode.Repeat)
                {
                    _canPrevious = true;
                    return _canPrevious;
                }
                _canPrevious = (playlistcollection.Count > 1 && NowPlayingIndex - 1 >= 0) ? true : false;

                return _canPrevious;
            }
        }

        public string TempPlaylistName { get; private set; }
        
        public void Add(IPlayable vfc)
        {
            if (!this.PlayListCollection.Contains(vfc))
            {
                this.PlayListCollection.Add(vfc);
            }
            if (CurrentPlaylist != null)
                HasChanges = true;
        }

        public void AddRange(IEnumerable<IPlayable> EnumerableVfc)
        {
            this.PlayListCollection.AddRange(EnumerableVfc);
            if (CurrentPlaylist != null)
                HasChanges = true;
        }

        public IPlayable GetNextItem()
        {
            int curreentpos = NowPlayingIndex;
            if (MoviesRepeatMode == RepeatMode.RepeatOnce)
            {
                return NowPlaying;
            }
            if (CanNext)
            {
                int finalpos = curreentpos + 1;
                if (MoviesRepeatMode == RepeatMode.Repeat)
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
                if (MoviesRepeatMode == RepeatMode.Repeat)
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

        public IPlaylistModel NewCreatePlaylist()
        {
            PlaylistModel ipl = new PlaylistModel
            {
                PlaylistName = this.TempPlaylistName
            };
            foreach (var item in PlayListCollection)
            {
                ipl.Add(Pathlist.Parse(item));
            }
            TempPlaylistName = null;
            return ipl as PlaylistModel;
        }

        public void PlayFromAList(IPlaylistModel plm)
        {
            Clear();
            CurrentPlaylist = plm;
            IsLoadingPlaylist = true;
            currentplaylist.SetIsActive(true);
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
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }, 2000);
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
        }

        private void UpdateList()
        {
            if (currentplaylist == null) return;
            (currentplaylist as PlaylistModel).Clear();
            foreach (var item in PlayListCollection)
            {
                (currentplaylist as PlaylistModel).Add(Pathlist.Parse(item));
            }
            HasChanges = false;
        }

        public string WhatsNextItem()
        {
            if (CanNext)
            {
                int finalpos = NowPlayingIndex + 1;
                if (MoviesRepeatMode == RepeatMode.Repeat)
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
                if (MoviesRepeatMode == RepeatMode.Repeat)
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
        
        public void PlayFromTemperalList(IPlayable playFile, IEnumerable<IPlayable> TemperalList)
        {
            Clear();
            if (currentplaylist != null)
            {
                currentplaylist.SetIsActive(true);
                CurrentPlaylist = null;
            }
            PlayListCollection = new ObservableCollection<IPlayable>(TemperalList);
            //Come back to this
            //MediaControllerViewModel.GetVideoItem((VideoFolderChild)playFile, true);
        }

        private void Clear()
        {
            if (CurrentPlaylist != null)
            {
                if (HasChanges)
                {
                    if (MessageBox.Show("Do you wish to save changes in " + CurrentPlaylist.PlaylistName + " ?", CurrentPlaylist.PlaylistName, MessageBoxButton.OKCancel)== MessageBoxResult.OK)
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

            SortedType = sortType;
            //Task.Factory.StartNew(() => FileLoader.SortList(sortType, playlistcollection)).ContinueWith(t =>
            //{
            //    _playlist = t.Result;
            //    RaisePropertyChanged(() => this.PlayListCollection);
            //}, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
