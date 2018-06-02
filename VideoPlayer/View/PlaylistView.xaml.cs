using Common.Interfaces;
using Common.Util;
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
using VideoComponent.BaseClass;
using VideoPlayer.PlayList;
using VideoPlayer.ViewModel;
using VirtualizingListView.ViewModel;

namespace VideoPlayer
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView : UserControl,IPlayListClose
    {
        public static RoutedCommand OkCommand = new RoutedCommand();
        public static RoutedCommand CancelCommand = new RoutedCommand();

        public PlaylistView()
        {
            InitializeComponent();
            this.DataContext = new PlayListManager();
            this.CommandBindings.Add(new CommandBinding(OkCommand, 
                OkCommand_Execute, OkCommand_Enabled));
            this.CommandBindings.Add(new CommandBinding(CancelCommand,
                CancelCommand_Execute));
        }

        private void CancelCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var vm = this.DataContext as PlayListManager;
            NewPlaylistName.Text = string.Empty;
            vm.IsSaveDialogEnable = false;
        }

        private void OkCommand_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute =  NewPlaylistName.Text != string.Empty;
        }

        private void OkCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var vm = this.DataContext as PlayListManager;
            vm.SavePlaylistAction();
            vm.IsSaveDialogEnable = false;
        }

        public event EventHandler OnPlaylistClose;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as PlayListManager;
            if (OnPlaylistClose != null)
            {
                OnPlaylistClose.Invoke(sender, null);
            }
            (MediaControllerVM.MediaControllerInstance.IVideoElement as Window).Focus();
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
                            (this.DataContext as PlayListManager).Add(item);
                            if (!hasfiles)
                            {
                                hasfiles = true;
                            }
                        }
                       
                    }
                    if (hasfiles)
                    {
                        CollectionViewModel.Instance.ItemProvider.CompleteLoad(vf.OtherFiles);
                    }
                }
                else
                {
                    (this.DataContext as PlayListManager).Add(vf);
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
           
            if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightCtrl))
            {
                (MediaControllerVM.MediaControllerInstance.IVideoPlayer as UserControl).Focus();
                return;
            }
             (MediaControllerVM.MediaControllerInstance.IVideoElement as Window).Focus();
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

        internal void OnPlaylistCloseExecute(object sender)
        {
            if (OnPlaylistClose != null)
            {
                OnPlaylistClose.Invoke(sender, null);
            }
        }
        
    }
}
