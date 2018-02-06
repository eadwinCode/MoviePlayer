using Common.ApplicationCommands;
using Common.Interfaces;
using Common.Util;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VideoComponent.BaseClass;
using VideoComponent.Command;
using VideoComponent.Events;
using VirtualizingListView.Events;
using VirtualizingListView.Model;
using VirtualizingListView.OtherFiles;
using VirtualizingListView.Util;
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

        private void View_enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.collections.Items.Count > 0;
        }

        private void View_executed(object sender, ExecutedRoutedEventArgs e)
        {
            var viewType = (ViewType)e.Parameter ;
            if (viewType == ViewType.None) return;
            CollectionViewModel.Instance.ViewType = viewType;
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
