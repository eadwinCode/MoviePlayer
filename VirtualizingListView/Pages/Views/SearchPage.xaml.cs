using Common.Interfaces;
using Microsoft.Practices.ServiceLocation;
using SearchComponent;
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
using VirtualizingListView.Pages.Model;
using VirtualizingListView.Util.AddFolderDialogWindow;

namespace VirtualizingListView.Pages.Views
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : UserControl,IAddFolderDialog
    {
        static SearchPage Instance = null;
        public SearchPage()
        {
            InitializeComponent();
            (INavigatorHost.GetSearchControl as ISearchControl<VideoFolder>).UpdateEvent += SearchControl_UpdateEvent;

            (INavigatorHost.GetSearchControl as ISearchControl<VideoFolder>).OnSearchStarted += SearchControl_OnSearchStarted;
            INavigatorHost.GetSearchControl.UseExtendedSearchTextbox(this.searchtbx);
            this.Loaded += SearchPage_Loaded;
        }

        private void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.searchtbx.Focus();
        }

        private void SearchControl_OnSearchStarted(object sender)
        {
            this.IsEnabled = false;
        }

        private void SearchControl_UpdateEvent(ICollection<VideoFolder> arrayArgs, string searchquery)
        {
            this.IsEnabled = true;
            if (OnFinished != null)
                OnFinished.Invoke(new SearchModel() { Results = arrayArgs, SearchQuery = searchquery }, null);
            INavigatorService.DockControl.Content = null;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(e.Key == Key.Enter)
            {
                INavigatorHost.GetSearchControl.QuickSearch(this.searchtbx.Text);
            }
        }

        public event EventHandler OnFinished;

        public void ShowDialog()
        {
            INavigatorService.DockControl.Content = this;
        }

        private INavigatorService INavigatorService
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShell>().PageNavigatorHost.PageNavigator;
            }
        }

        private IPageNavigatorHost INavigatorHost
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IShell>().PageNavigatorHost;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OnFinished != null)
                OnFinished.Invoke(null, null);
            INavigatorService.DockControl.Content = null;
        }

        public static SearchPage GetSearchPage()
        {
            if (Instance == null)
                Instance = new SearchPage();

            return Instance;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
