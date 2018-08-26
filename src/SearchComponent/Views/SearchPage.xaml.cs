using Microsoft.Practices.ServiceLocation;
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using PresentationExtension.CommonEvent;
using PresentationExtension.InterFaces;
using SearchComponent;
using SearchComponent.Model;
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

namespace SearchComponent.Views
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        static SearchPage Instance = null;
        public SearchPage()
        {
            InitializeComponent();
            (ISearchControl).UpdateEvent += SearchControl_UpdateEvent;
            ISearchControl.SearchPattern = SearchMode.Object;
            ISearchControl.IsSearchButtonEnabled = true;
            ISearchControl.OnSearchStarted += SearchControl_OnSearchStarted;
            ISearchControl.UseExtendedSearchTextbox(this.searchtbx);
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
            //INavigatorService.DockControl.Content = null;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(e.Key == Key.Enter)
            {
                ISearchControl.QuickSearch(this.searchtbx.Text);
            }
        }

        public event EventHandler OnFinished;

        public void ShowDialog()
        {
            EventManager.GetEvent<NavigateNewPage>().Publish(GetSearchPage());
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OnFinished != null)
                OnFinished.Invoke(null, null);

            if (this.NavigationService.CanGoBack)
                NavigationService.GoBack();
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

        private ISearchControl<VideoFolder> ISearchControl
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ISearchControl<VideoFolder>>();
            }
        }

        private IEventManager EventManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IEventManager>();
            }
        }
    }
}
