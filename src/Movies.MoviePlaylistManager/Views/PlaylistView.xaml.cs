
using Movies.Models.Model;
using Movies.MoviePlaylistManager.ViewModel;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Movies.MoviePlaylistManager.Views
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView : UserControl,IPlaylistViewMediaPlayerView
    {       
        IMediaControllerViewModel MediaControllerViewModel;
        private bool IsRegisteredCommand;

        public PlaylistView(IMediaControllerViewModel IMediaControllerViewModel, IPlaylistManager playlistManagerViewModel)
        {
            InitializeComponent();
            this.DataContext = playlistManagerViewModel;
            this.MediaControllerViewModel = IMediaControllerViewModel;
            this.Loaded += new RoutedEventHandler(PlaylistView_Loaded);
        }

        void PlaylistView_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = this.DataContext as PlaylistManager;
            datacontext.PlaylistViewLoaded();
        }

        public event EventHandler OnPlaylistClose;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as PlaylistManager;
            if (OnPlaylistClose != null)
            {
                OnPlaylistClose.Invoke(sender, null);
            }
            (MediaControllerViewModel.IVideoElement as Window).Focus();
            vm.IsSaveDialogEnable = false;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            VideoFolder vf = (VideoFolder)e.Data.GetData(typeof(VideoFolder));
            if (vf == null)
            {
                vf = (VideoFolder)e.Data.GetData(typeof(VideoFolderChild));
            }
            if (vf != null)
            {
                if (vf.FileType == FileType.Folder)
                {
                    bool hasfiles = false;
                    
                    foreach (VideoFolder item in vf.OtherFiles)
                    {
                        if (item.FileType == FileType.File)
                        {
                            (this.DataContext as PlaylistManager).Add(item);
                            if (!hasfiles)
                            {
                                hasfiles = true;
                            }
                        }
                       
                    }
                    if (hasfiles)
                    {
                        //CollectionViewModel.Instance.ItemProvider.CompleteLoad(vf.OtherFiles);
                    }
                }
                else
                {
                    (this.DataContext as PlaylistManager).Add(vf);
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
           
            if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightCtrl))
            {
                (MediaControllerViewModel.IVideoElement.MediaController as UserControl).Focus();
                return;
            }
           (MediaControllerViewModel.IVideoElement as Window).Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            e.Handled = false;
        }

        public void OnPlaylistCloseExecute(object sender)
        {
            if (OnPlaylistClose != null)
            {
                OnPlaylistClose.Invoke(sender, null);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ContextMenu contextMenu = btn.ContextMenu;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            contextMenu.PlacementTarget = btn;
            contextMenu.IsOpen = true;
        }

        
    }
}
