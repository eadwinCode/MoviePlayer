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
using System.Windows.Controls;

namespace Movies.MoviePlaylistManager.ViewModel
{
    public partial class PlaylistManager
    {
        private bool issavedialogenable = false;
        private bool _canNext;
        private bool _canPrevious;
        ObservableCollection<IPlayable> _playlist = new ObservableCollection<IPlayable>();

        private bool isloading;
        private string sortedname;
        private bool haschanges = false;

        private string tempplaylistname;
        private bool hasSubcribed = false;
        private PlaylistModel currentplaylist;
        private IPlayable NowPlaying;

        private DelegateCommand clearplaylist;
        private DelegateCommand cancelcommand;
        private DelegateCommand okcommand;
        private DelegateCommand extsortcommand;
        private DelegateCommand datesortcommand;
        private DelegateCommand nameCommand;
        private DelegateCommand enablesavedialog;

        public bool IsSaveDialogEnable
        {
            get { return issavedialogenable; }
            set { issavedialogenable = value; RaisePropertyChanged(() => this.IsSaveDialogEnable); }
        }
        
        public ObservableCollection<IPlayable> PlayListCollection
        {
            get { return _playlist; }
            set
            {
                _playlist = value;
                if (value != null && CurrentPlaylist != null)
                {
                    //MediaControllerViewModel.GetVideoItem((IPlayable)value.First(), true);
                }
                this.RaisePropertyChanged(() => this.PlayListCollection);
            }
        }

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
                            SavePlaylistDialog savePlaylistDialog = new SavePlaylistDialog();
                            savePlaylistDialog.ShowDialog();
                        }
                    });
                }
                return enablesavedialog;
            }
        }

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
        
        public DelegateCommand NameSortCommand
        {
            get
            {
                if (nameCommand == null)
                    nameCommand = new DelegateCommand(() => {
                        SortFunction(SortType.Name);
                        (MediaControllerViewModel.IVideoElement as Window).Focus();
                    });
                return nameCommand;
            }
        }

        public DelegateCommand DateSortCommand
        {
            get
            {
                if (datesortcommand == null)
                    datesortcommand = new DelegateCommand(() => 
                    {
                        SortFunction(SortType.Date);
                        (MediaControllerViewModel.IVideoElement as Window).Focus();
                    });
                return datesortcommand;
            }
        }

        public DelegateCommand ExtSortCommand
        {
            get
            {
                if (extsortcommand == null)
                    extsortcommand = new DelegateCommand(() =>
                    {
                        SortFunction(SortType.Extension);
                        (MediaControllerViewModel.IVideoElement as Window).Focus();
                    });
                return extsortcommand;
            }
        }

        public DelegateCommand OkCommand
        {
            get
            {
                if (okcommand == null)
                    okcommand = new DelegateCommand(() => {
                        SavePlaylistAction();
                        MediaControllerViewModel.IVideoElement.ContentDockRegion.Content = null;
                        (MediaControllerViewModel.IVideoElement.MediaController as UserControl).Focus();
                    },CanOkAction);
                return okcommand;
            }
        }

        public DelegateCommand CancelCommand
        {
            get
            {
                if (cancelcommand == null)
                    cancelcommand = new DelegateCommand(() => 
                    {
                        TempPlaylistName = string.Empty;
                        MediaControllerViewModel.IVideoElement.ContentDockRegion.Content = null;
                        (MediaControllerViewModel.IVideoElement.MediaController as UserControl).Focus();
                    });
                return cancelcommand;
            }
        }

        public bool CanNext
        {
            get
            {
                if (MediaControllerViewModel.RepeatMode == RepeatMode.Repeat)
                {
                    _canNext = true;
                    return _canNext;
                }
                _canNext = _playlist.Count > 1 && NowPlayingIndex + 1 != _playlist.Count ? true : false;

                return _canNext;
            }
        }

        public bool IsLoading
        {
            get { return isloading; }
            set
            {
                isloading = value;
                IEventManager.GetEvent<IsPlaylistManagerBusy>().Publish(value);
                RaisePropertyChanged(() => this.IsLoading);
            }
        }
        
        public bool CanPrevious
        {
            get
            {
                if (MediaControllerViewModel.RepeatMode == RepeatMode.Repeat)
                {
                    _canPrevious = true;
                    return _canPrevious;
                }
                _canPrevious = (_playlist.Count > 1 && NowPlayingIndex - 1 >= 0) ? true : false;

                return _canPrevious;
            }
        }

        public bool HasChanges
        {
            get { return haschanges; }
            set { haschanges = value; RaisePropertyChanged(() => this.HasChanges); }
        }

        public int NowPlayingIndex
        {
            get
            {
                return _playlist.IndexOf(NowPlaying);
            }
        }
        
        public string PlaylistName
        {
            get
            {
                if (CurrentPlaylist == null)
                {
                    return "UnSaved Playlist";
                }
                return CurrentPlaylist.PlaylistName;
            }
        }

        public string TempPlaylistName
        {
            get
            {
                return tempplaylistname;
            }
            set
            {
                tempplaylistname = value;
                OkCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => this.TempPlaylistName);
            }
        }

        public string SortedName
        {
            get { return sortedname; }
            set { sortedname = value; RaisePropertyChanged(() => this.SortedName); }
        }

        public PlaylistModel CurrentPlaylist
        {
            get { return currentplaylist; }
            set
            {
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

        public IFileLoader FileLoader
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoader>();
            }
        }

        IFileExplorerCommonHelper FileExplorerCommonHelper
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileExplorerCommonHelper>();
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
        IFileLoaderCompletion LoaderCompletion
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoaderCompletion>();
            }
        }

       

        IMediaControllerViewModel MediaControllerViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>().MediaControllerViewModel;
            }
        }

        IEventManager IEventManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventManager>();
            }
        }
        
    }

}
