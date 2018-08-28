using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using Movies.Models.Interfaces;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.PlaylistCollectionView.RenameDialog;
using Movies.PlaylistCollectionView.Views;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Movies.PlaylistCollectionView.ViewModels
{
    public class HomePlaylistViewModel : NotificationObject,IHomePlaylist
    {
        IApplicationService ApplicationService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IApplicationService>();
            }
        }

        IFileLoader FileLoader
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFileLoader>();
            }
        }

        IEventManager IEventManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventManager>();
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

        private Point DialogLocation;
        HomePlaylistView HomePlaylistView = null;

        public static RoutedUICommand OpenPlaylist { get; private set; } 
        public static RoutedUICommand RenamePlaylist { get; private set; }
        public static RoutedUICommand RemovePlaylist { get; private set; } 
        public static RoutedUICommand EditPlaylist { get; private set; } 
        public static AddPlaylistCollectionHandler AddToPlaylistCollection;
        
        public ObservableCollection<PlaylistModel> PlayListCollection
        {
            get
            {
                return ApplicationService.AppPlaylist.MoviePlayList;
            }
        }

        public HomePlaylistViewModel()
        {
            InitializeComponent();
            //GetHomePlaylistView();
            IEventManager.GetEvent<PlaylistCollectionChangedEventToken>().Subscribe((playlist) =>
            {
                AddToPlayList(playlist as IPlaylistModel);
            });
            
        }

        public void InitializeComponent()
        {
            OpenPlaylist = new RoutedUICommand();
            RenamePlaylist = new RoutedUICommand();
            RemovePlaylist = new RoutedUICommand();
            EditPlaylist = new RoutedUICommand();
        }

        public object GetHomePlaylistView()
        {
            if (HomePlaylistView != null) return HomePlaylistView;

            
            HomePlaylistView = new HomePlaylistView
            {
                DataContext = this
            };

            HomePlaylistView.CommandBindings.Add(new CommandBinding(OpenPlaylist,
               OpenPlaylist_executed,
               OpenPlaylist_enabled));
            HomePlaylistView.CommandBindings.Add(new CommandBinding(RenamePlaylist,
                RenamePlaylist_executed,
                RenamePlaylist_enabled));
            HomePlaylistView.CommandBindings.Add(new CommandBinding(RemovePlaylist,
                RemovePlaylist_executed,
                RemovePlaylist_enabled));

            HomePlaylistView.CommandBindings.Add(new CommandBinding(EditPlaylist, EditPlaylist_Executed, EditPlaylist_Enabled));

            return HomePlaylistView;
        }

        private void EditPlaylist_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            PlaylistModel pm = (PlaylistModel)e.Parameter;
            e.CanExecute = !pm.IsActive;
        }

        private void EditPlaylist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PlaylistModel playlistModel = e.Parameter as PlaylistModel;
            EditPlaylistView editPlaylistView = new EditPlaylistView(playlistModel);
            editPlaylistView.ShowDialog();
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
                pm.PlaylistName + " playlist", "Movie Playlist", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                RemoveMoviePlaylistItem(pm);
                RaisePropertyChanged(() => this.PlayListCollection);
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

            RaisePropertyChanged(() => this.PlayListCollection);
        }

        private void OpenPlaylist_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!FileLoader.HasDataSource)
            {
                e.CanExecute = false;
                return;
            }
            PlaylistModel pm;
            pm = e.Parameter is PlaylistModel == false ? (e.OriginalSource as FrameworkElement)
                    .DataContext as PlaylistModel : (PlaylistModel)e.Parameter;
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

        

        public void AddToPlayList(IPlaylistModel ipl, bool addtomovieplaylist = true)
        {
            if (addtomovieplaylist)
            {
                AddMoviePlaylistItem(ipl as PlaylistModel);
                RaisePropertyChanged(() => this.PlayListCollection);
                ApplicationService.SavePlaylistFiles();
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
            if (renameDialog.IsCancel)
                return;

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
           // DialogLocation = Mouse.GetPosition(this);
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

       
    }
}
