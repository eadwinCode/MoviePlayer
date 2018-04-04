using Common.ApplicationCommands;
using Common.FileHelper;
using Common.Interfaces;
using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using VirtualizingListView.ViewModel;

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for MyTreeView.xaml
    /// </summary>
    public partial class MyTreeView : UserControl, ITreeViewer
    {
        private object dummyNode = null;
        private TreeViewItem SelectedTreeViewItem;

        public UserControl MoviesFolder { get { return this; } }

        public UserControl MoviesPLaylist { get { return this.PlaylistView; } }

        public MyTreeView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //const int MaxPath = 260;
            //StringBuilder SB = new StringBuilder(MaxPath);
            //SHGetFolderPath(IntPtr.Zero, 0x0003, IntPtr.Zero, 0x0000, SB);
            //string  DesktopPath = SB.ToString();

            AddToTreeView( Environment.GetFolderPath(Environment.SpecialFolder.Desktop),false);
            AddToTreeView(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),false);

            foreach (string item in ApplicationService.AppSettings.TreeViewItems)
            {
                AddToTreeView(item);
            }

            
            //PlaylistChildren.ItemsSource = CreateHelper.AppSettings.MoviePlayList;


            //foreach (IPlaylistModel ipl in CreateHelper.AppSettings.MoviePlayList)
            //{
            //    AddToPlayList(ipl,false);
            //}

            //load saved lists

            //foreach (var s in Environment.GetLogicalDrives())
            //{
            //    DirectoryInfo dir = new DirectoryInfo(s);
            //    TreeViewItem item = new TreeViewItem();
            //    item.Header = s;
            //    item.Tag = dir.Name;
            //    item.FontWeight = FontWeights.Normal;
            //    item.Items.Add(dummyNode);
            //    item.Expanded += new RoutedEventHandler(folder_Expanded);
            //    FolderList.Items.Add(item);
            //}
        }

        void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
              
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        DirectoryInfo dir = new DirectoryInfo(s);
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = dir.ToString();
                        subitem.FontWeight = FontWeights.Normal;
                        
                        try
                        {
                        if (dir.GetDirectories().Count() > 0)
                        {
                            subitem.Items.Add(dummyNode);
                        }
                        }
                        catch (Exception) { }
                        subitem.Expanded += new RoutedEventHandler(Folder_Expanded);
                        
                        item.Items.Add(subitem);
                    }
            }
        }

        private void FolderList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)tree.SelectedItem;
            CollectionViewModel.Instance.TreeViewUpdate(item.Tag.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string dir = folderDialog.SelectedPath;

                AddToTreeView(dir);
                AddTreeViewItem(dir);
            }
        }

        private void AddToTreeView(string dir,bool applyContextment = true)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(dir);

            TreeViewItem item = new TreeViewItem
            {
                Tag = dir,
                Header = dirinfo.Name,
                FontWeight = FontWeights.Normal
            };
            if (applyContextment)
            {
                item.ContextMenu = FindResource("TreeViewContext") as ContextMenu;
                item.ContextMenuOpening += new ContextMenuEventHandler(FolderList_ContextMenuOpening);
            }
           
            item.Items.Add(dummyNode);

            item.Expanded += new RoutedEventHandler(Folder_Expanded);
            FolderList.Items.Add(item);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTreeViewItem == null) return;

           if( MessageBox.Show("Are sure you want to remove " + SelectedTreeViewItem.Header,"Movie FolderList",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
           {
               FolderList.Items.Remove(SelectedTreeViewItem);
                RemoveTreeViewItem(SelectedTreeViewItem.Tag.ToString());
            }
        }

        private void FolderList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            SelectedTreeViewItem = (TreeViewItem)sender;
        }

        private void FolderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem item = (TreeViewItem)tree.SelectedItem;
            if (item != null)
            {
                CollectionViewModel.Instance.TreeViewUpdate(item.Tag.ToString());
            }

        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
        //    base.OnPreviewKeyDown(e);
            ((IShell.FileView as IFileViewer).FileExplorer as UIElement).Focus();
            e.Handled = true;
        }

        private void DataTemplate_Opened(object sender, RoutedEventArgs e)
        {

        }


        public void AddTreeViewItem(string dir)
        {
            if (!ApplicationService.AppSettings.TreeViewItems.Contains(dir))
            {
                ApplicationService.AppSettings.TreeViewItems.Add(dir);
            }
        }

        public void RemoveTreeViewItem(string dir)
        {
            if (ApplicationService.AppSettings.TreeViewItems.Contains(dir))
            {
                ApplicationService.AppSettings.TreeViewItems.Remove(dir);
            }
        }

        private IShell IShell
        {
            get { return ServiceLocator.Current.GetInstance<IShell>(); }
        }
    }
}
