using Common.ApplicationCommands;
using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VirtualizingListView.Pages.ViewModel;
using VirtualizingListView.Util.RenameDialog;

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for HomePlaylistView.xaml
    /// </summary>
    public partial class HomePlaylistView : UserControl, INotifyPropertyChanged
    {
        private Point DialogLocation;
        public event PropertyChangedEventHandler PropertyChanged;
        public static RoutedCommand OpenPlaylist= new RoutedCommand();
        public static RoutedCommand RenamePlaylist = new RoutedCommand();
        public static RoutedCommand RemovePlaylist = new RoutedCommand();
        public static AddPlaylistCollectionHandler AddToPlaylistCollection;

        public ObservableCollection<PlaylistModel> PlayListCollection
        {
            get { return ApplicationService.AppPlaylist.MoviePlayList; }
        }
        

        public HomePlaylistView()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += PlaylistTree_Loaded;

            this.CommandBindings.Add(new CommandBinding(OpenPlaylist,
                OpenPlaylist_executed,
                OpenPlaylist_enabled));
            this.CommandBindings.Add(new CommandBinding(RenamePlaylist,
                RenamePlaylist_executed,
                RenamePlaylist_enabled));
            this.CommandBindings.Add(new CommandBinding(RemovePlaylist,
                RemovePlaylist_executed,
                RemovePlaylist_enabled));

            AddToPlaylistCollection += AddToPlayList;
        }

        private void RemovePlaylist_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            PlaylistModel pm = (PlaylistModel)e.Parameter;
            e.CanExecute = pm != null;
        }

        private void RemovePlaylist_executed(object sender, ExecutedRoutedEventArgs e)
        {
            PlaylistModel pm = (PlaylistModel)e.Parameter;

            if (MessageBox.Show("You wish to remove " +
                pm.PlaylistName + " playlist", "Movie Playlist", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                RemoveMoviePlaylistItem(pm);
                Playlist.Items.Refresh();
            }
        }

        private void RenamePlaylist_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            PlaylistModel pm = (PlaylistModel)e.Parameter;
            e.CanExecute = pm != null;
        }

        private void RenamePlaylist_executed(object sender, ExecutedRoutedEventArgs e)
        {
            PlaylistModel pm = (PlaylistModel)e.Parameter;

            RenameDialogControl renameDialog = new RenameDialogControl()
            {
                PlaylistModel = pm
            };
            renameDialog.OnFinished += RenameDialog_OnFinished;
            renameDialog.ShowDialog();
        }

        private void RenameDialog_OnFinished(object sender, EventArgs e)
        {
            RenameDialogControl renameDialog = (RenameDialogControl)sender;
            PlaylistModel playlistModel = renameDialog.PlaylistModel;
            playlistModel.PlaylistName = renameDialog.RenameText.Text == (string.Empty) ?
                playlistModel.PlaylistName : renameDialog.RenameText.Text;
            Playlist.Items.Refresh();
        }

        private void OpenPlaylist_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            if (HomePageLocalViewModel.HasSearchData())
            {
                e.CanExecute = false;
                return;
            }
            PlaylistModel pm;
            pm = e.Parameter is PlaylistModel == false ? 
                (e.OriginalSource as FrameworkElement).DataContext as PlaylistModel : 
                (PlaylistModel)e.Parameter;
            e.CanExecute = !pm.IsActive;
        }

        private void OpenPlaylist_executed(object sender, ExecutedRoutedEventArgs e)
        {
            PlaylistModel pm = (PlaylistModel)e.Parameter;

            if (pm != null)
            {
                PlayFile.PlayFileFromPlayList(pm);
            }
        }

        private void PlaylistTree_Loaded(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("PlayListCollection");
        }

        public void AddToPlayList(IPlaylistModel ipl, bool addtomovieplaylist = true)
        {
            if (addtomovieplaylist)
            {
                AddMoviePlaylistItem(ipl as PlaylistModel);
                Playlist.Items.Refresh();
            }
        }

        public void CreateNewPlayList(string ItemPath)
        {
            RenameDialogControl renameDialog = new RenameDialogControl()
            {
                ItemPath = ItemPath
            };
            renameDialog.OnFinished += RenameDia_OnFinished;
            renameDialog.ShowDialog();
           
        }

        public void CreateNewPlayList(PlaylistModel playlistModel)
        {
            RenameDialogControl renameDialog = new RenameDialogControl()
            {
                PlaylistModel = playlistModel
            };
            renameDialog.OnFinished += RenameDia_OnFinished;
            renameDialog.ShowDialog();
        }

        private void RenameDia_OnFinished(object sender, EventArgs e)
        {
            RenameDialogControl renameDialog = (RenameDialogControl)sender;
            PlaylistModel playlistModel = renameDialog.PlaylistModel;
            if (renameDialog.IsCancel) return;

            string PlaylistName = renameDialog.RenameText.Text;
            if (playlistModel == null)
            {
                PlaylistModel plm = new PlaylistModel
                {
                    PlaylistName = PlaylistName
                };
                plm.Add(renameDialog.ItemPath);
                AddToPlayList(plm);
                return;
            }
            playlistModel.PlaylistName = PlaylistName;
            AddToPlayList(playlistModel);
        }

        private void PlayList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Panel control = (sender as Panel);
            DialogLocation = Mouse.GetPosition(this);
        }

        public void AddMoviePlaylistItem(PlaylistModel plm)
        {
            if (!ApplicationService.AppPlaylist.MoviePlayList.Contains(plm))
            {
                ApplicationService.AppPlaylist.MoviePlayList.Add(plm);
            }
        }

        public void RemoveMoviePlaylistItem(PlaylistModel plm)
        {
            if (ApplicationService.AppPlaylist.MoviePlayList.Contains(plm))
            {
                ApplicationService.AppPlaylist.MoviePlayList.Remove(plm);
            }
        }

        private IShell Shell
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShell>();
            }
        }

        private IPlayFile PlayFile
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayFile>();
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
    }

    public delegate void AddPlaylistCollectionHandler(IPlaylistModel ipl, bool addtomovieplaylist = true);
}
