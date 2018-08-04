using Common.ApplicationCommands;
using Common.FileHelper;
using Common.Interfaces;
using Common.Model;
using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using VirtualizingListView.Util;
using VirtualizingListView.ViewModel;

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for MyTreeView.xaml
    /// </summary>
    public partial class MyTreeView : UserControl, ITreeViewer,INotifyPropertyChanged
    {
        private object dummyNode = null;
        private TreeViewItem selectedtreeviewitem;
        public TreeViewItem SelectedTreeViewItem
        {
            get { return selectedtreeviewitem; }
            set { selectedtreeviewitem = value; }
        }
        public UserControl MoviesFolder { get { return this; } }

        public UserControl MoviesPLaylist { get { return this.PlaylistView; } }

        private ObservableCollection<TreeViewItem> moviefolders;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TreeViewItem> MovieFolders
        {
            get { return moviefolders; }
            set { moviefolders = value; OnPropertyChanged("MovieFolders"); }
        }


        public MyTreeView()
        {
            InitializeComponent();
            this.DataContext = this;
            MovieFolders = new ObservableCollection<TreeViewItem>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //const int MaxPath = 260;
            //StringBuilder SB = new StringBuilder(MaxPath);
            //SHGetFolderPath(IntPtr.Zero, 0x0003, IntPtr.Zero, 0x0000, SB);
            //string  DesktopPath = SB.ToString();

            AddToTreeView(new MovieFolderModel( Environment.GetFolderPath(Environment.SpecialFolder.Desktop)), MovieFolders, false);
            AddToTreeView(new MovieFolderModel( Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)),MovieFolders,false);

            foreach (MovieFolderModel item in ApplicationService.AppSettings.MovieFolders)
            {
                AddToTreeView(item, MovieFolders);
            }

            //MovieFolders = new ObservableCollection<TreeViewItem>(
            //MovieFolders.OrderByDescending(x => x.Header.ToString() == "Videos" || x.Header.ToString() == "Desktop")
            //.GroupBy(x => x.Tag.ToString().Length == 3)
            //.OrderByDescending(g => g.First().Header)
            //.SelectMany(g => g));

           // MovieFolders = new ObservableCollection<TreeViewItem>(
         //     MovieFolders.OrderByDescending(x => x.Tag.ToString().Length == 3)
         //.GroupBy(x => x.Header.ToString() == "Videos" || x.Header.ToString() == "Desktop")
         //.OrderByDescending(g => g.First().Header)
         //.SelectMany(g => g));
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
                MovieFolderModel moviePath = new MovieFolderModel(dir);
                AddToTreeView(moviePath, MovieFolders);
                AddTreeViewItem(moviePath);
            }
           // MovieFolders = new ObservableCollection<TreeViewItem>(
           //     MovieFolders.OrderByDescending(x => x.Tag.ToString().Length == 3)
           //.GroupBy(x => x.Header.ToString() == "Videos" || x.Header.ToString() == "Desktop")
           //.OrderByDescending(g => g.First().Header)
           //.SelectMany(g => g));
        }

        private void AddToTreeView(MovieFolderModel dir,IList collections,bool applyContextment = true)
        {
            DirectoryInfo dirinfo = dir.DirectoryInfo;

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
            collections.Add(item);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTreeViewItem == null) return;

           if( MessageBox.Show("Are sure you want to remove " + SelectedTreeViewItem.Header,"Movie FolderList",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
           {
               MovieFolders.Remove(SelectedTreeViewItem);
                RemoveTreeViewItem((MovieFolderModel)SelectedTreeViewItem.Tag);
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
            (((IPageNavigatorHost)IShell.PageNavigatorHost).PageNavigator as UIElement).Focus();
            e.Handled = true;
        }

        private void DataTemplate_Opened(object sender, RoutedEventArgs e)
        {

        }


        public void AddTreeViewItem(MovieFolderModel dir)
        {
            if (!ApplicationService.AppSettings.MovieFolders.Contains(dir))
            {
                ApplicationService.AppSettings.MovieFolders.Add(dir);
            }
        }

        public void RemoveTreeViewItem(MovieFolderModel dir)
        {
            if (ApplicationService.AppSettings.MovieFolders.Contains(dir))
            {
                ApplicationService.AppSettings.MovieFolders.Remove(dir);
            }
        }

        private IShell IShell
        {
            get { return ServiceLocator.Current.GetInstance<IShell>(); }
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
