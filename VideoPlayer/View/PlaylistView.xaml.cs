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
using VideoPlayerControl.PlayList;
using VideoPlayerControl.ViewModel;
using VirtualizingListView.Util;
using VirtualizingListView.ViewModel;

namespace VideoPlayerControl
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView : UserControl,IPlayListClose
    {
        public static RoutedCommand OkCommand = new RoutedCommand();
        public static RoutedCommand CancelCommand = new RoutedCommand();

        public static RoutedCommand NameSort = new RoutedCommand();
        public static RoutedCommand DateSort = new RoutedCommand();
        public static RoutedCommand ExtensionSort = new RoutedCommand();

        public PlaylistView()
        {
            InitializeComponent();
            this.DataContext = new PlayListManager();
            this.CommandBindings.Add(new CommandBinding(OkCommand, 
                OkCommand_Execute, OkCommand_Enabled));
            this.CommandBindings.Add(new CommandBinding(CancelCommand,
                CancelCommand_Execute));
            this.CommandBindings.Add(new CommandBinding(NameSort,
               NameSort_Execute));
            this.CommandBindings.Add(new CommandBinding(DateSort,
              DateSort_Execute));
            this.CommandBindings.Add(new CommandBinding(ExtensionSort,
              ExtensionSort_Execute));

            this.Loaded += new RoutedEventHandler(PlaylistView_Loaded);
        }

        private void NameSort_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            SortFunction(SortType.Name,e);
        }

        private void SortFunction(SortType sortType, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is Button)
            {
                Button button = e.OriginalSource as Button;
                button.Content = sortType.ToString();
            }
            var datacontext = this.DataContext as PlayListManager;
            datacontext.PlayListCollection = FileLoader.FileLoaderInstance.SortList(sortType, datacontext.PlayListCollection);
        }

        private void DateSort_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            SortFunction(SortType.Date,e);
        }

        private void ExtensionSort_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            SortFunction(SortType.Extension,e);
        }

        void PlaylistView_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = this.DataContext as PlayListManager;
            datacontext.PlaylistViewLoaded();
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
