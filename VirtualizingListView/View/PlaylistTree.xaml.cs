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

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for PlaylistTree.xaml
    /// </summary>
    public partial class PlaylistTree : UserControl, INotifyPropertyChanged
    {
        private Point DialogLocation;
        public event PropertyChangedEventHandler PropertyChanged;
        public static RoutedCommand OpenPlaylist= new RoutedCommand();
        public static RoutedCommand RenamePlaylist = new RoutedCommand();
        public static RoutedCommand RemovePlaylist = new RoutedCommand();

        public ObservableCollection<PlaylistModel> PlayListCollection
        {
            get { return CreateHelper.AppPlaylist.MoviePlayList; }
        }
        

        public PlaylistTree()
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

            RenameDialog renameDialog = new RenameDialog
            {
                PlaylistModel = pm,
                Owner = (Shell as Window),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            renameDialog.ShowDialog();
            if (renameDialog.DialogResult == false) return;

            pm.PlaylistName = renameDialog.RenameText.Text == (string.Empty) ? pm.PlaylistName: renameDialog.RenameText.Text;
            Playlist.Items.Refresh();
        }

        private void OpenPlaylist_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
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
                UnSelectItem(Playlist);
            }
        }

        private void PlaylistTree_Loaded(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("PlayListCollection");
        }
        
        private void PlayList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            PlaylistModel pm = (PlaylistModel)tree.SelectedItem;
            PlayFile.PlayFileFromPlayList(pm);
            UnSelectItem(tree);
        }

        private void UnSelectItem(TreeView treeview)
        {
            var container = FindTreeViewSelectedItemContainer(treeview, treeview.SelectedItem);
            if (container != null)
            {
                container.IsSelected = false;
            }
        }

        private static TreeViewItem FindTreeViewSelectedItemContainer(ItemsControl root, object selection)
        {
            var item = root.ItemContainerGenerator.ContainerFromItem(selection) as TreeViewItem;
            if (item == null)
            {
                foreach (var subItem in root.Items)
                {
                    item = FindTreeViewSelectedItemContainer((TreeViewItem)root.ItemContainerGenerator.ContainerFromItem(subItem), selection);
                    if (item != null)
                    {
                        break;
                    }
                }
            }

            return item;
        }

        public void AddToPlayList(IPlaylistModel ipl, bool addtomovieplaylist = true)
        {
            //TreeViewItem item = new TreeViewItem
            //{
            //    Tag = ipl,
            //    Header = ipl.PlaylistName,
            //    FontWeight = FontWeights.Normal
            //};
            //item.ContextMenu = FindResource("PlaylistContextMenu") as ContextMenu;
            //    item.ContextMenuOpening += new ContextMenuEventHandler(PlayList_ContextMenuOpening);
            //PlaylistChildren.Items.Add(item);
            if (addtomovieplaylist)
            {
                AddMoviePlaylistItem(ipl as PlaylistModel);
                Playlist.Items.Refresh();
            }

        }

        public void CreateNewPlayList(string ItemPath)
        {
            RenameDialog renameDialog = new RenameDialog
            {
                Owner = (Shell as Window),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            renameDialog.ShowDialog();
            if (renameDialog.DialogResult == false) return;
            string PlaylistName = renameDialog.RenameText.Text;

            PlaylistModel plm = new PlaylistModel
            {
                PlaylistName = PlaylistName
            };
            plm.Add(ItemPath);

            AddToPlayList(plm);
        }

        private void PlayList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Panel control = (sender as Panel);
            DialogLocation = Mouse.GetPosition(this);
        }

        public void AddMoviePlaylistItem(PlaylistModel plm)
        {
            if (!CreateHelper.AppPlaylist.MoviePlayList.Contains(plm))
            {
                CreateHelper.AppPlaylist.MoviePlayList.Add(plm);
            }
        }

        public void RemoveMoviePlaylistItem(PlaylistModel plm)
        {
            if (CreateHelper.AppPlaylist.MoviePlayList.Contains(plm))
            {
                CreateHelper.AppPlaylist.MoviePlayList.Remove(plm);
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
}
