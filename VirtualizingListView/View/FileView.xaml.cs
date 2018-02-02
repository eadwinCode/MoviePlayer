using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VirtualizingListView.Model;
using VirtualizingListView.ViewModel;

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for FileView.xaml
    /// </summary>
    public partial class FileView : UserControl,IFileViewer
    {
        //private static VirtualizingVM vm;
        private static string ParentDirectory;
        static DispatcherTimer timer;

        public UIElement TreeViewer { get => this.treeviewer; }

        public UIElement FileExplorer => this.fileexpr;

        public FileView()
        {
            InitializeComponent();
            this.DataContext = CollectionViewModel.Instance; 
            this.Loaded += VideoComponentList_Loaded;

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += timer_Tick;

            // LthisoadandSave ls = new LoadandSave();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //progressbar.Value = VideoDataAccessor.progresslevel;
            //pmtxt.Text = string.Format("{0:0.00} MB", GC.GetTotalMemory(true) / 1024.0 / 1024.0);
        }

        void VideoComponentList_Loaded(object sender, RoutedEventArgs e)
        {
            //vm.VideoComponentViewModel_Loaded(sender, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.MainViewWrapper.Visibility = System.Windows.Visibility.Collapsed;

            //this.ListPanel.Visibility = System.Windows.Visibility.Collapsed;
            //this.ListPanelspliter.Visibility = System.Windows.Visibility.Collapsed;
            //Grid gd = VideoHolder as Grid;
            //Grid.SetRowSpan(gd, 3);
        }


        public static void TreeviewChanged(string s)
        {
            ParentDirectory = s;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(Bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bw_RunWorkerCompleted);


            bw.RunWorkerAsync();
            //  Task.Factory.StartNew(() => );
            //  Task.WaitAll();
            // vm.StartLoadingProcedure(s);
        }

        static void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        static void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(10);
            //vm.StartLoadingProcedure(ParentDirectory);
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            ScrollViewer scrollViewer = GetScrollViewer(listView);
            if (scrollViewer != null)
            {
                ScrollBar scrollBar = scrollViewer.Template.FindName("PART_VerticalScrollBar", scrollViewer) as ScrollBar;
                if (scrollBar != null)
                {
                    scrollBar.ValueChanged += delegate
                    {
                        //VerticalOffset and ViweportHeight is actually what you want if UI virtualization is turned on.
                        Console.WriteLine("Visible Item Start Index:{0}", scrollViewer.VerticalOffset);
                        Console.WriteLine("Visible Item Count:{0}", scrollViewer.ViewportHeight);
                    };
                }
            }
        }

        public static void Complete()
        {
            timer.Stop();
        }

        public static void StartedHiddenLoad()
        {
            timer.Start();
        }
        public static ScrollViewer GetScrollViewer(DependencyObject depobj)
        {
            var obj = depobj as ScrollViewer;
            if (obj != null) return obj;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depobj); i++)
            {
                var child = VisualTreeHelper.GetChild(depobj, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }

            return null;
        }

        private void ListView_CleanUpVirtualizedItem(object sender, CleanUpVirtualizedItemEventArgs e)
        {

        }

        private void ListView_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {

        }
    }
}
