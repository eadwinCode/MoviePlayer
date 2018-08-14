using Common.Interfaces;
using Common.Util;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VideoComponent.BaseClass;
using VirtualizingListView.ViewModel;

namespace VirtualizingListView.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileExplorer : UserControl, IFileExplorer
    {
        public bool IsNavigationEnabled
        {
            get { return (bool)GetValue(IsNavigationEnabledProperty); }
            set { SetValue(IsNavigationEnabledProperty, value); }
        }

        public ListView FileExplorerListView { get { return collections; } }
        private VideoFolder VideoFolder = null;
        public object ContextMenuObject { get { return VideoFolder; } }

        // Using a DependencyProperty as the backing store for IsNavigationEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNavigationEnabledProperty =
            DependencyProperty.Register("IsNavigationEnabled", typeof(bool), typeof(FileExplorer), new FrameworkPropertyMetadata { DefaultValue = true});

        
        
        public FileExplorer()
        {
            InitializeComponent();
            DataContext = CollectionViewModel.Instance;
            CollectionViewModel.Instance.GetFileExplorerInstance(this);
            this.Loaded += CollectionViewModel.Instance.VideoComponentViewModel_Loaded;
            
            //this.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.Sort, 
            //    Sort_executed, View_enabled));
            //this.CommandBindings.Add(new CommandBinding(VideoPlayerCommands.View,
            //    View_executed, View_enabled));
        }

        private ScrollViewer scrollviewer;
        public ScrollViewer ScrollViewer
        {
            get
            {
                if (scrollviewer == null)
                {
                    Border border = (Border)VisualTreeHelper.GetChild(collections, 0);
                    scrollviewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                }
                
                return scrollviewer;
            }
        }

        public void ResetScrollBar()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.ScrollViewer.ScrollToVerticalOffset(-1);
                this.ScrollViewer.ScrollToHorizontalOffset(-1);
            }));
        }

        private void View_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.collections.Items.Count > 0;
        }

        private void View_executed(object sender, ExecutedRoutedEventArgs e)
        {
            var viewType = (ViewType)e.Parameter;
            CollectionViewModel.Instance.ActiveViewType = viewType;
        }

        private void Sort_executed(object sender, ExecutedRoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            //this.Focus();
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);
            FrameworkElement uIElement = e.OriginalSource as FrameworkElement;
            this.VideoFolder = uIElement.DataContext as VideoFolder;
        }
        
    }
    
}
